using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services
{
    public class CorrespondentService : BaseClient<Correspondent>, ICorrespondentService
    {
        private static readonly string CORRESPONDENT_ENDPOINT = "api/correspondents/";

        public CorrespondentService(
            IApiService apiService
            , IPersistenceService persistenceService
            , ITenantService tenantService) 
            : base(apiService, persistenceService, tenantService, CORRESPONDENT_ENDPOINT)
        {
        }

        public IObservable<List<Correspondent>> GetAndFetchAllCorrespondents()
            => InternalGetAndFetchAll(defaultEndpoint);

        public Task<List<Correspondent>> GetAllCorrespondents()
            => InternalFetchAll(defaultEndpoint);
    }
}
