using PaperlessClient.Mobile.Events;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Services
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private static readonly string DOCUMENT_TYPE_ENDPOINT = "api/document_types/";

        #region services
        private IApiService apiService;
        private IPersistenceService persistenceService;
        #endregion

        #region props
        private ApiSetup tenant;
        private int currentPage;
        private bool moreResultsAvailable;
        private string tenantPersistencePrefix => $"docservice_{tenant.Endpoint}";
        #endregion

        public DocumentTypeService(
            IApiService apiService
            , IPersistenceService persistenceService
            , ITenantService tenantService)
        {
            this.apiService = apiService;
            this.persistenceService = persistenceService;

            tenant = tenantService.GetCurrentTennant();
            MessagingCenter.Subscribe<TenantChangedEvent>(this, nameof(TenantChangedEvent), (e) => { tenant = e.NewTenant; });
        }                

        public async Task<List<DocumentType>> GetDocumentTypes(int page)
        {
            var documentTypeResponse = await apiService.Get<ApiListResponse<DocumentType>>(
                DOCUMENT_TYPE_ENDPOINT
                , new Dictionary<string, string>() { { "page", page.ToString() } });

            currentPage = page;
            moreResultsAvailable = !string.IsNullOrWhiteSpace(documentTypeResponse?.Next);
            return documentTypeResponse.Results;
        }

        public IObservable<List<DocumentType>> GetAndFetchAllDocumentTypes()
            => persistenceService.GetAndFetchObjectAsync(
                $"{tenantPersistencePrefix}_doctypes"
                , GetAllDocumentTypes);

        public async Task<List<DocumentType>> GetAllDocumentTypes()
        {
            var result = new List<DocumentType>();
            currentPage = 0;
            moreResultsAvailable = true;
            while (moreResultsAvailable)
            {
                var documentTypes = await GetDocumentTypes(currentPage + 1);
                result.AddRange(documentTypes);
            }
            return result;
        }        
    }
}
