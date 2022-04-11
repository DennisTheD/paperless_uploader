using Newtonsoft.Json;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services
{
    public class ApiService : IApiService
    {
        private static readonly string LOGIN_ENDPOINT = "api/token/";
        private static readonly string UPLOAD_ENDPOINT = "api/documents/post_document/";

        private static readonly string API_CONFIG_KEY = "api_config";
        private IPersistenceService persistenceService;
        private INotificationService notificationService;

        private ApiSetup apiSetup;
        private HttpClient httpClient;

        public ApiService(
            IPersistenceService persistenceService
            , INotificationService notificationService)
        {
            this.persistenceService = persistenceService;
            this.notificationService = notificationService;
            httpClient = new HttpClient();
        }

        public async Task<bool> IsSetupComplete() {
            if (apiSetup != null)
                return true;

            try
            {
                apiSetup = await persistenceService.GetSecureAsync<ApiSetup>(API_CONFIG_KEY);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            catch (Exception ex) { 
                System.Diagnostics.Debug.WriteLine($"Failed to get api setup from storage service: {ex.Message}");
                throw;
            }

            if (apiSetup != null) {
                httpClient.BaseAddress = new Uri(apiSetup.Endpoint);
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Token", apiSetup.Token);
                return true;
            }

            return false;
        }

        public async Task<bool> Login(Uri endpoint, string username, string password) {
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
            if (!string.IsNullOrWhiteSpace(loginResponse?.Token)) {
                httpClient.BaseAddress = endpoint;
                httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Token", loginResponse.Token);               

                apiSetup = new ApiSetup() { 
                    Endpoint = endpoint.ToString(),
                    Token = loginResponse.Token
                };
                await persistenceService.PersistSecureAsync(API_CONFIG_KEY, apiSetup); // persist the token

                return true;
            }

            return false;
        }

        public async Task UploadInForeground(
            Uri fileUri
            , string documentTitle = null) {
            if (!fileUri.IsFile)
                throw new InvalidOperationException("Document to upload should be a file");

            var fileName = Path.GetFileName(fileUri.LocalPath);
            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            using(var fs = new FileStream(path: fileUri.LocalPath, mode: FileMode.Open)) {
                content.Add(new StreamContent(fs), "document", fileName);
                content.Add(new StringContent(documentTitle ?? fileName), "title");
                var result = await httpClient.PostAsync(UPLOAD_ENDPOINT, content);
                result.EnsureSuccessStatusCode();
                var response = await result.Content.ReadAsStringAsync();
            }            
        }
    }
}
