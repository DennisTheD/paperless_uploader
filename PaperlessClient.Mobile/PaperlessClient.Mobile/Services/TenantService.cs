using Newtonsoft.Json;
using PaperlessClient.Mobile.Events;
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
        private string defaultTennantKey = string.Empty;

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

                // notify other components about the new tenant
                MessagingCenter.Send(
                    new TenantListChangedEvent() { NewTenant = apiSetup }
                    , nameof(TenantListChangedEvent));

                if (activeTennant == null || setAsDefault) {
                    defaultTennantKey = $"{TENNANT_SETUP_PREFIX}{endpoint}";
                    await persistenceService.PersistSecureAsync(DEFAULT_TENNANT_KEY, $"{TENNANT_SETUP_PREFIX}{endpoint}");
                    activeTennant = apiSetup;
                }                    

                return true;
            }

            return false;
        }

        public async Task InitializeAsync() {
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

        public void ChangeTenant(ApiSetup tenant) {
            activeTennant = tenant;
            // notify other components about the change
            MessagingCenter.Send(
                new TenantChangedEvent(activeTennant)
                , nameof(TenantChangedEvent));
        }

        public Task<List<ApiSetup>> GetTennants()
            => persistenceService.GetAllSecureAsync<ApiSetup>();

        public async Task DeleteTenant(ApiSetup tenant)
        {
            var targetTenantKey = $"{TENNANT_SETUP_PREFIX}{tenant.Endpoint}";

            var requireTenantChange = 
                tenant.Endpoint == activeTennant.Endpoint;
            var requireNewDefaultTenant = targetTenantKey == defaultTennantKey;

            // delete the configuration
            await persistenceService.DeleteSecureAsync(targetTenantKey);

            // notify other components about the deleted tenant
            MessagingCenter.Send(
                new TenantListChangedEvent() { DeletedTenant = tenant }
                , nameof(TenantListChangedEvent));


            if (requireTenantChange || requireNewDefaultTenant) {
                var availableTenants = await GetTennants();
                if (availableTenants == null || availableTenants.Count == 0) {
                    // require a new login
                    MessagingCenter.Send(
                        new LoginRequiredEvent()
                        , nameof(LoginRequiredEvent));
                    activeTennant = null;
                    defaultTennantKey = null;

                    return;
                }
                
                if (requireTenantChange) {
                    ChangeTenant(availableTenants[0]);                    
                } 
                
                if (requireNewDefaultTenant) {
                    await SetDefaultTenant(activeTennant);
                }
            }
        }

        public async Task SetDefaultTenant(ApiSetup tenant)
        {
            defaultTennantKey = $"{TENNANT_SETUP_PREFIX}{activeTennant.Endpoint}";
            await persistenceService.PersistSecureAsync(
                DEFAULT_TENNANT_KEY
                , defaultTennantKey);
        }
    }
}
