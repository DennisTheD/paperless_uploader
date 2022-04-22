﻿using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services
{
    public abstract class BaseClient<TEntityType>
        where TEntityType : ApiEntity
    {
        protected IApiService apiService;
        protected IPersistenceService persistenceService;
        protected string defaultEndpoint;

        protected BaseClient(
            IApiService apiService
            , IPersistenceService persistenceService
            , string defaultEndpoint)
        {
            this.apiService = apiService;
            this.persistenceService = persistenceService;
            this.defaultEndpoint = defaultEndpoint;
        }

        protected async Task<TFetchType> InternalFetch<TFetchType>(
            string endpoint
            , Dictionary<string,string> parameters = null)
        {
            return await apiService.Get<TFetchType>(endpoint, parameters);
        }

        protected IObservable<TFetchType> InternalGetAndFetch<TFetchType>(string endpoint)
        {
            return persistenceService.GetAndFetchObjectAsync<TFetchType>(
                $"baseapiclient.{endpoint}"
                , () => InternalFetch<TFetchType>(endpoint));
        }

        public IObservable<List<TEntityType>> InternalGetAndFetchAll(string endpoint)
            => persistenceService.GetAndFetchObjectAsync(
                $"baseapiclient.{endpoint}.all"
                , () => InternalFetchAll(endpoint));

        protected async Task<List<TEntityType>> InternalFetchAll(string endpoint) { 
            var result = new List<TEntityType>();
            var currentPage = 1;
            var moreResultsAvailable = true;
            while (moreResultsAvailable)
            {
                var apiListResponse = await InternalFetch<ApiListResponse<TEntityType>>(
                    endpoint
                    , new Dictionary<string, string>() { { "page", currentPage++.ToString() } });


                result.AddRange(apiListResponse.Results);
                moreResultsAvailable = !string.IsNullOrWhiteSpace(apiListResponse?.Next);
            }

            return result;
        }
    }
}
