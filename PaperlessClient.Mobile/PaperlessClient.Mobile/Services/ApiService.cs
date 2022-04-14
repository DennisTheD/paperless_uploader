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
        private static readonly string UPLOAD_ENDPOINT = "api/documents/post_document/";
        private ITenantService tenantService;


        public ApiService(
            ITenantService tennantService)
        {
            this.tenantService = tennantService;
        }


        public async Task UploadInForeground(
            Uri fileUri
            , string documentTitle = null) {
            if (!fileUri.IsFile)
                throw new InvalidOperationException("Document to upload should be a file");

            var fileName = Path.GetFileName(fileUri.LocalPath);
            using(var httpClient = new HttpClient())
            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            using(var fs = new FileStream(path: fileUri.LocalPath, mode: FileMode.Open)) {
                var currentTennant = tenantService.GetCurrentTennant();
                httpClient.BaseAddress = new Uri(currentTennant.Endpoint);
                httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Token", currentTennant.Token);

                content.Add(new StreamContent(fs), "document", fileName);
                content.Add(new StringContent(documentTitle ?? fileName), "title");
                var result = await httpClient.PostAsync(UPLOAD_ENDPOINT, content);
                result.EnsureSuccessStatusCode();
                var response = await result.Content.ReadAsStringAsync();
            }            
        }
    }
}
