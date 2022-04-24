using PaperlessClient.Mobile.Converters;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.NavigationHints;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class DocumentListViewModel : ListViewModelBase<Document>
    {
        #region props
        private int currentPage = 1;

        private bool isLoadingMoreDocuments;
        public bool IsLoadingMoreDocuments {
            get => isLoadingMoreDocuments;
            set { 
                SetProperty(ref isLoadingMoreDocuments, value);
                LoadMoreDocumentsCommand.ChangeCanExecute();
            }
        }

        private bool moreDocumentsAvailable;
        public bool MoreDocumentsAvailable
        {
            get => moreDocumentsAvailable;
            set
            {
                SetProperty(ref moreDocumentsAvailable, value);
                LoadMoreDocumentsCommand.ChangeCanExecute();
            }
        }
        #endregion

        #region services
        private IDocumentService documentService;
        private INavigationService navigationService;
        #endregion

        #region commands
        private Command loadMoreDocumentsCommand;
        public Command LoadMoreDocumentsCommand {
            get {
                if (loadMoreDocumentsCommand == null) {
                    loadMoreDocumentsCommand = new Command(async(o) => { await LoadMoreDocuments(); }
                    , (o) => !IsLoadingMoreDocuments && MoreDocumentsAvailable);
                }
                return loadMoreDocumentsCommand;
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
                  , () => documentService.GetDocuments(1)
                  , documentService.GetAndFetchDocuments
                  , FilterDocuments)
        {
            this.documentService = documentService;
            this.navigationService = navigationService;
            MoreDocumentsAvailable = true;

            RequireConverter(
                typeof(IdsToTagNameListConverter)
                , typeof(IdToCorrespondentNameConverter)
                , typeof(IdToDocumentTypeNameConverter));

            ItemSelectedCommand = new Command(async(d) => await OnDocumentSelected(d));
        }

        private static List<Document> FilterDocuments(string arg1, List<Document> arg2)
        {
            return arg2;
        }

        private async Task OnDocumentSelected(object doc)
        {
            if (SelectedItem != null) {
                await navigationService.NavigateToAsync(
                    nameof(DocumentViewerPage)
                    , new DocumentViewerNavigationHint() { DocumentId = SelectedItem.Id, DocumentTitle = SelectedItem.Title });
            }
        }

        private async Task LoadMoreDocuments()
        {
            if (documentService.MoreResultsAvailable)
            {
                IsLoadingMoreDocuments = true;
                try
                {
                    var documents = await documentService.GetDocuments(++currentPage);
                    documents.ForEach(document => Items.Add(document));
                }
                catch (Exception)
                {
                }
                finally
                {
                    IsLoadingMoreDocuments = false;
                }
            }
            else {
                MoreDocumentsAvailable = false;
            }
        }
    }
}
