using PaperlessClient.Mobile.Resources;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class SetupViewModel : ViewModelBase
    {
        private TaskCompletionSource<bool> setupTcs = new TaskCompletionSource<bool>();
        public Task SetupTask => setupTcs.Task;

        public string Endpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        private Command loginCommand;
        public Command LoginCommand {
            get {
                if (loginCommand == null) {
                    loginCommand = new Command(async () => { await Login(); });
                }
                return loginCommand;
            }
        }

        private IApiService apiService;

        public SetupViewModel(
            IApiService apiService
            , INotificationService notificationService) 
            : base(notificationService)
        {
            this.apiService = apiService;
        }

        public override Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private async Task Login() {
            if (!TryParseEndpoint(Endpoint, out Uri endpintUri)) {
                await notificationService.NotifyIfInForeground(this, TextResources.ErrorText, TextResources.InvalidServerText);
                return;
            }

            IsBusy = true;
            var success = false;
            try{
                success = await apiService.Login(endpintUri, Username, Password);
            }
            catch (Exception){}
            finally {
                IsBusy = false;
            }

            if (!success)
                await notificationService.NotifyIfInForeground(this, TextResources.ErrorText, TextResources.LoginFailedText);
            else
                setupTcs.SetResult(true);
        }

        private bool TryParseEndpoint(string endpoint, out Uri uri) { 
            return Uri.TryCreate(endpoint, UriKind.Absolute, out uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
