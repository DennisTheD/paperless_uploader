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
    public class TagService : BaseClient<Tag>, ITagService
    {
        private static readonly string TAG_ENDPOINT = "api/tags/";

        public TagService(
            IApiService apiService
            , IPersistenceService persistenceService) 
            : base(apiService, persistenceService, TAG_ENDPOINT)
        {
        }

        public IObservable<List<Tag>> GetAndFetchAllTags()
            => InternalGetAndFetchAll(defaultEndpoint);

        public Task<List<Tag>> GetAllTags()
            => InternalFetchAll(defaultEndpoint);
    }
}
