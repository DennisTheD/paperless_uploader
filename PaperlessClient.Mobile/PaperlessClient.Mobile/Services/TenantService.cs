using Newtonsoft.Json;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Services
{
    public class TenantService : ITenantService
    {
        private static readonly string LOGIN_ENDPOINT = "api/token/";
        private static readonly string TENNANT_SETUP_PREFIX = "tennantservice_";
        private static readonly string DEFAULT_TENNANT_KEY = "tennantservice_default_tennant";

        private IPersistenceService persistenceService;
        private ApiSetup activeTennant;

        public TenantService(
            IPersistenceService persistenceService)
        {
            this.persistenceService = persistenceService;
        }

        public async Task<bool> Login(
            Uri endpoint
            , string username
            , string password
            , string tennantName
            , bool setAsDefault = false)
        {
            // prepare the login request
            var loginRequest = new ApiLoginRequest(username, password);
            var requestText = JsonConvert.SerializeObject(loginRequest);
            var requestContent = new StringContent(requestText, Encoding.UTF8, "application/json");
            var loginClient = new HttpClient();
            loginClient.BaseAddress = endpoint;

            // send the login response
            ApiLoginResponse loginResponse = null;
            try
            {
                var response = await loginClient.PostAsync(LOGIN_ENDPOINT, requestContent);
                response.EnsureSuccessStatusCode();
                var responseText = await response.Content.ReadAsStringAsync();
                loginResponse = JsonConvert.DeserializeObject<ApiLoginResponse>(responseText); // parse the token
            }
            catch (Exception)
            {
                return false;
            }

            // check if we got a token
            if (!string.IsNullOrWhiteSpace(loginResponse?.Token))
            {               
                var apiSetup = new ApiSetup()
                {
                    Endpoint = endpoint.ToString(),
                    Token = loginResponse.Token,
                    Name = tennantName
                };
                await persistenceService.PersistSecureAsync($"{TENNANT_SETUP_PREFIX}{endpoint}", apiSetup); // persist the setup

                if (activeTennant == null || setAsDefault) {
                    await persistenceService.PersistSecureAsync(DEFAULT_TENNANT_KEY, $"{TENNANT_SETUP_PREFIX}{endpoint}");
                    activeTennant = apiSetup;
                }                    

                return true;
            }

            return false;
        }

        public async Task InitializeAsync() {
            string defaultTennantKey = null;
            try
            {
                defaultTennantKey = await persistenceService.GetSecureAsync<string>(DEFAULT_TENNANT_KEY);
                activeTennant = await persistenceService.GetSecureAsync<ApiSetup>(defaultTennantKey);
            }
            catch (Exception){
            }
        }

        public ApiSetup GetCurrentTennant()
            => activeTennant;

        public Task<List<ApiSetup>> GetTennants()
            => persistenceService.GetAllSecureAsync<ApiSetup>();
        
    }
}
