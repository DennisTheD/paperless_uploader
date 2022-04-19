using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services
{
    public class DocumentService : IDocumentService
    {
        private static readonly string DOCUMENT_ENDPOINT = "api/documents/";

        private IApiService apiService;
        private IPersistenceService persistenceService;

        private int currentPage = 0;
        private bool moreResultsAvailable = false;

        public bool MoreResultsAvailable => moreResultsAvailable;

        public DocumentService(
            IApiService apiService
            , IPersistenceService persistenceService)
        {
            this.apiService = apiService;
            this.persistenceService = persistenceService;
        }

        public IObservable<List<Document>> GetAndFetchDocuments()
            => persistenceService.GetAndFetchObjectAsync(DOCUMENT_ENDPOINT, () => GetDocuments(1));

        public async Task<List<Document>> GetDocuments(int page)
        {
            var documentResponse = await apiService.Get<ApiListResponse<Document>>(
                DOCUMENT_ENDPOINT
                , new Dictionary<string, string>() { {"page", page.ToString() } });

            currentPage = 1;
            moreResultsAvailable = !string.IsNullOrWhiteSpace(documentResponse?.Next);
            return documentResponse.Results;
        }
    }
}
