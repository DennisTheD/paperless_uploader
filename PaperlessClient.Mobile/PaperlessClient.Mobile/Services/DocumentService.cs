using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services
{
    public class DocumentService : BaseClient<Document>, IDocumentService
    {
        private static readonly string DOCUMENT_ENDPOINT = "api/documents/";
        private CancellationTokenSource lastRequestCancellationToken;
        private Task lastTask;

        public DocumentService(
            IApiService apiService
            , IPersistenceService persistenceService
            , ITenantService tenantService)
            :base(apiService, persistenceService, tenantService, DOCUMENT_ENDPOINT)
        {
        }

        public IObservable<ApiListResponse<Document>> GetAndFetchDocuments()
            => persistenceService.GetAndFetchObjectAsync(FormatPersistenceKey(DOCUMENT_ENDPOINT), () => GetDocuments(1));

        public Task<ApiListResponse<Document>> GetDocuments(int page)
        {
            CancelLastRequest();
            var response = apiService.Get<ApiListResponse<Document>>(
                DOCUMENT_ENDPOINT
                , new Dictionary<string, string>() { {"page", page.ToString() } }
                , lastRequestCancellationToken.Token);
            lastTask = response;
            return response;
        }

        public Task<ApiListResponse<Document>> SearchDocuments(string query, int page)
        {
            CancelLastRequest();
            var response = apiService.Get<ApiListResponse<Document>>(
                DOCUMENT_ENDPOINT
                , new Dictionary<string, string>() {
                    {"page", page.ToString() }
                    , { "query", query }
                }, lastRequestCancellationToken.Token);
            lastTask = response;
            return response;
        }

        private void CancelLastRequest() {
            // cancel the last request if its still running
            if (lastRequestCancellationToken != null
                && !lastTask.IsCompleted)
            {
                lastRequestCancellationToken.Cancel();
            }

            lastRequestCancellationToken = new CancellationTokenSource();
        }
    }
}
