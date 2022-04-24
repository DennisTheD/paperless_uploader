using PaperlessClient.Mobile.NavigationHints;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PaperlessClient.Mobile.ViewModels
{
    public class DocumentViewerViewModel : ViewModelBase
    {
        private int documentId = 0;

        private string filePath;
        public string FilePath {
            get => filePath;
            set => SetProperty(ref filePath, value);
        }

        public DocumentViewerViewModel(
            INotificationService notificationService
            , ITenantService tenantService) 
            : base(notificationService, tenantService)
        {
        }

        public override async Task InitializeAsync(object parameter)
        {
            if (parameter is DocumentViewerNavigationHint navigationHint) {
                Title = navigationHint.DocumentTitle;
                documentId = navigationHint.DocumentId;

                FilePath = $"{tenantService.GetCurrentTennant().Endpoint}api/documents/{documentId}/download/";
            }
        }
    }
}
