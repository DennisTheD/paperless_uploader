using PaperlessClient.Mobile.NavigationHints;
using PaperlessClient.Mobile.Resources;
using PaperlessClient.Mobile.Services.Abstraction;
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
            , INotificationService notificationService) 
            : base(notificationService)
        {
            this.apiService = apiService;
            this.navigationService = navigationService;
        }

        public override async Task InitializeAsync(object parameter)
        {
            if (parameter is UploadFileNavigationHint navigationHint) { 
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
            catch (Exception)
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

            await navigationService.PopAsync();
        }
    }
}
