using PaperlessClient.Mobile.NavigationHints;
using PaperlessClient.Mobile.Resources;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class DocumentViewerViewModel : ViewModelBase
    {
        private INavigationService navigationService;

        private int documentId = 0;

        private string filePath;
        public string FilePath {
            get => filePath;
            set => SetProperty(ref filePath, value);
        }

        #region commands
        private Command loadStartCommand;
        public Command LoadStartCommand {
            get {
                if (loadStartCommand == null) {
                    loadStartCommand = new Command(() => IsBusy = true);
                }
                return loadStartCommand;
            }
        }

        private Command loadFinishedCommand;
        public Command LoadFinishedCommand
        {
            get
            {
                if (loadFinishedCommand == null)
                {
                    loadFinishedCommand = new Command(() => IsBusy = false);
                }
                return loadFinishedCommand;
            }
        }

        private Command loadErrorCommand;
        public Command LoadErrorCommand
        {
            get
            {
                if (loadErrorCommand == null)
                {
                    loadErrorCommand = new Command(async(o) => await OnLoadError(o));
                }
                return loadErrorCommand;
            }
        }
        #endregion

        public DocumentViewerViewModel(
            INavigationService navigationService
            , INotificationService notificationService
            , ITenantService tenantService) 
            : base(notificationService, tenantService)
        {
            this.navigationService = navigationService;
        }

        public override Task InitializeAsync(object parameter)
        {
            if (parameter is DocumentViewerNavigationHint navigationHint) {
                Title = navigationHint.DocumentTitle;
                documentId = navigationHint.DocumentId;

                FilePath = $"{tenantService.GetCurrentTennant().Endpoint}api/documents/{documentId}/download/";
            }
            return Task.CompletedTask;
        }

        private async Task OnLoadError(object o)
        {
            IsBusy = false;
            await notificationService.NotifyIfInForeground(this, TextResources.ErrorText, TextResources.DocumentDownoadFailedText);
            await navigationService.PopAsync();
        }
    }
}
