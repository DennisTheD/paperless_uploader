using PaperlessClient.Mobile.Events;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.NavigationHints;
using PaperlessClient.Mobile.Resources;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class UploadFileViewModel : ViewModelBase
    {
        private Uri fileUri;
        private bool deleteFileAfterUpload;

        private IApiService apiService;
        private INavigationService navigationService;

        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private ApiSetup tenant;
        public ApiSetup Tenant {
            get => tenant;
            set => SetProperty(ref tenant, value);
        }


        private Command uploadCommand;
        public Command UploadCommand { 
            get {
                if(uploadCommand == null){
                    uploadCommand = new Command(async () => { await Upload(); }, () => !IsBusy);
                }
                return uploadCommand;
            } 
        }

        public UploadFileViewModel(
            IApiService apiService
            , INavigationService navigationService
            , INotificationService notificationService
            , ITenantService tenantService
            , IFileUploadQueueService fileUploadQueueService) 
            : base(notificationService, tenantService)
        {
            this.apiService = apiService;
            this.navigationService = navigationService;

            // get the current tenant and register for changes to display the target tenant inside the form
            Tenant = tenantService.GetCurrentTennant();
            MessagingCenter.Subscribe<TenantChangedEvent>(
                this
                , nameof(TenantChangedEvent)
                , (e) => { Tenant = e.NewTenant; });

            // on first start, the InitializeAsync will not get called
            // we need to fake the initilization with this hack
            if (fileUploadQueueService.GetTask(out FileUploadRequest uploadRequest))
            {
                fileUri = uploadRequest.FileUri;
                deleteFileAfterUpload = true;
                Name = uploadRequest.FileTitle;

                InitializeAsync(null);
            }
        }

        public override async Task InitializeAsync(object parameter)
        {
            if (parameter is UploadFileNavigationHint navigationHint)
            {
                fileUri = navigationHint.FileUri;
                deleteFileAfterUpload = navigationHint.DeleteFileAfterUpload;
                Name = navigationHint.Title;
            }

            if (fileUri == null
                || !fileUri.IsFile) {
                await notificationService.NotifyIfInForeground(this, TextResources.ErrorText, TextResources.InvalidFileText);
                await navigationService.PopAsync();
                return;
            }
        }

        public async Task Upload() {
            IsBusy = true;
            UploadCommand.ChangeCanExecute();
            try
            {
                await apiService.UploadInForeground(fileUri, Name);
            }
            catch (Exception ex)
            {
                await notificationService.NotifyIfInForeground(this, TextResources.ErrorText, TextResources.UploadFailedText);
                return;
            }
            finally { 
                IsBusy = false;
                UploadCommand.ChangeCanExecute();
            }

            if (deleteFileAfterUpload) {
                try
                {
                    File.Delete(fileUri.LocalPath);
                }
                catch (Exception){}
            }

            await navigationService.NavigateToAndAndPopAsync(
                $"//{nameof(LandingPage)}"
                , null);
        }
    }
}
