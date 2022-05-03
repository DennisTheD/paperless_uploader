using PaperlessClient.Mobile.Converters;
using PaperlessClient.Mobile.Events;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.NavigationHints;
using PaperlessClient.Mobile.Resources;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class DocumentListViewModel : ApiListViewModelsBase<Document>
    {
        private bool isInitialized = false;

        private bool isInSearchMode;
        public bool IsInSearchMode {
            get => isInSearchMode;
            set => SetProperty(ref isInSearchMode, value);
        }

        #region services
        private IDocumentService documentService;
        private INavigationService navigationService;
        #endregion

        #region commands
        private Command searchCommand;
        public Command SearchCommand
        {
            get
            {
                if (searchCommand == null)
                {
                    searchCommand = new Command(async (o) => { await SearchDocuments(1); });
                }
                return searchCommand;
            }
        }

        private Command quitSearchCommand;
        public Command QuitSearchCommand
        {
            get
            {
                if (quitSearchCommand == null)
                {
                    quitSearchCommand = new Command(QuitSearch);
                }
                return quitSearchCommand;
            }
        }        
        #endregion

        public DocumentListViewModel(
            IDocumentService documentService
            , INavigationService navigationService
            , INotificationService notificationService
            , ITenantService tenantService) 
            : base(
                  notificationService
                  , tenantService
                  , documentService.GetDocuments
                  , documentService.GetAndFetchDocuments)
        {
            this.documentService = documentService;
            this.navigationService = navigationService;

            RequireConverter(
                typeof(IdsToTagNameListConverter)
                , typeof(IdToCorrespondentNameConverter)
                , typeof(IdToDocumentTypeNameConverter));

            ItemSelectedCommand = new Command(async(d) => await OnDocumentSelected(d));
        }

        private async Task OnDocumentSelected(object doc)
        {
            if (SelectedItem != null) {
                await navigationService.NavigateToAsync(
                    nameof(DocumentViewerPage)
                    , new DocumentViewerNavigationHint() { DocumentId = SelectedItem.Id, DocumentTitle = SelectedItem.Title });
            }
        }       

        private async Task SearchDocuments(int page = 1)
        {
            if (page == 1) // switch into search mode
            {
                Items = null;
                IsInSearchMode = true;
            }      
            
            IsBusy = true;
            try
            {                
                var apiResponse = await documentService.SearchDocuments(SearchText, page);
                MoreItemsAvailable = !string.IsNullOrWhiteSpace(apiResponse.Next);
                if (page == 1)
                {
                    IsInSearchMode = true;
                    Items = new ObservableCollection<Document>(apiResponse.Results);
                    currentPage = 1;
                }
                else {
                    apiResponse.Results.ForEach(d => Items.Add(d));
                }
            }
            catch (Exception)
            {
                await notificationService.NotifyIfInForeground(this, TextResources.ErrorText, TextResources.DocumentListFetchFailedText);
            }
            finally {
                IsBusy = false;
            }            
        }

        public override async Task InitializeAsync(object parameter)
        {
            if (!isInitialized) {
                await base.InitializeAsync(parameter);
                isInitialized = true;
            }            
        }

        protected override async Task LoadMoreItems()
        {
            if (!IsInSearchMode)
            {
                await base.LoadMoreItems();
            }
            else if (MoreItemsAvailable) {
                IsLoadingMoreItems = true;
                await SearchDocuments(++currentPage);
                IsLoadingMoreItems = false;
            }                          
        }

        private void QuitSearch()
        {
            IsInSearchMode = false;
            Items = null;
            RefreshCommand.Execute(false);
        }

        protected override void OnTenantChanged(TenantChangedEvent eventArgs)
        {
            SearchText = null;
            IsInSearchMode = false;
            base.OnTenantChanged(eventArgs);
        }
    }
}
