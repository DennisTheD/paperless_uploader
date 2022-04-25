using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Resources;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public abstract class ApiListViewModelsBase<TListEntity> : ListViewModelBase<TListEntity>
        where TListEntity : ApiEntity
    {
        #region props
        protected int currentPage = 1;

        private bool isLoadingMoreItems;
        public bool IsLoadingMoreItems
        {
            get => isLoadingMoreItems;
            set
            {
                SetProperty(ref isLoadingMoreItems, value);
                LoadMoreItemsCommand.ChangeCanExecute();
            }
        }

        private bool moreItemsAvailable;
        public bool MoreItemsAvailable
        {
            get => moreItemsAvailable;
            set
            {
                SetProperty(ref moreItemsAvailable, value);
                LoadMoreItemsCommand.ChangeCanExecute();
            }
        }

        protected Func<int, Task<ApiListResponse<TListEntity>>> apiListFetchFunc;
        protected Func<IObservable<ApiListResponse<TListEntity>>> apiListGetAndFetchFunc;
        #endregion

        private Command loadMoreItemsCommand;
        public Command LoadMoreItemsCommand
        {
            get
            {
                if (loadMoreItemsCommand == null)
                {
                    loadMoreItemsCommand = new Command(async (o) => { await LoadMoreItems(); }
                    , (o) => !IsLoadingMoreItems && MoreItemsAvailable);
                }
                return loadMoreItemsCommand;
            }
        }

        

        protected ApiListViewModelsBase(
            INotificationService notificationService
            , ITenantService tenantService
            , Func<int, Task<ApiListResponse<TListEntity>>> apiListFetchFunc
            , Func<IObservable<ApiListResponse<TListEntity>>> apiListGetAndFetchFunc)             
            : base(notificationService, tenantService, null, null, null)
        {
            UpdateFetchFuncs(apiListFetchFunc, apiListGetAndFetchFunc);
            MoreItemsAvailable = true;
        }

        protected void UpdateFetchFuncs(
            Func<int, Task<ApiListResponse<TListEntity>>> apiListFetchFunc
            , Func<IObservable<ApiListResponse<TListEntity>>> apiListGetAndFetchFunc)
        {

            this.apiListFetchFunc = apiListFetchFunc; 
            this.apiListGetAndFetchFunc = apiListGetAndFetchFunc;

            RefreshCommand = new Command(async (commandParam) => {
                var forceReload = commandParam as bool? ?? true;
                // reset the current page
                currentPage = 1;

                try {
                    await Refresh(
                        () => apiListFetchFunc(1)
                        , apiListGetAndFetchFunc
                        , (apiResponse) => {
                            Items = new ObservableCollection<TListEntity>(apiResponse.Results);
                            MoreItemsAvailable = !string.IsNullOrWhiteSpace(apiResponse.Next);
                            customChangeNotificationRegistrations.ForEach(p => OnPropertyChanged(p));
                        }
                        , forceReload);
                }
                catch(TaskCanceledException) { } // do nothing
                catch (Exception)
                {
                    await notificationService.NotifyIfInForeground(this, TextResources.ErrorText, TextResources.DocumentListFetchFailedText);
                }                
            });
        }

        protected virtual async Task LoadMoreItems()
        {
            if (MoreItemsAvailable) {
                IsLoadingMoreItems = true;
                try
                {
                    var apiResult = await apiListFetchFunc(++currentPage);
                    MoreItemsAvailable = !string.IsNullOrWhiteSpace(apiResult.Next);
                    apiResult.Results.ForEach(i => Items.Add(i));
                }
                catch (Exception){
                }
                finally { 
                    IsLoadingMoreItems = false;
                }
            }
        }
    }
}
