using PaperlessClient.Mobile.NavigationHints;
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

        private Action onSuccessAction;

        private string tennantName;
        public string TennantName { 
            get => tennantName;
            set => SetProperty(ref tennantName, value);
        }
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

        private ITenantService tenantService;

        public SetupViewModel(
            INotificationService notificationService
            , ITenantService tenantService) 
            : base(notificationService, tenantService)
        {
            this.tenantService = tenantService;
        }

        public override Task InitializeAsync(object parameter)
        {
            if (parameter is SetupNavigationHint navigationHint) {
                onSuccessAction = navigationHint.OnSuccess;
            }

            return Task.CompletedTask;
        }

        private async Task Login() {
            if (!TryParseEndpoint(Endpoint, out Uri endpintUri)) {
                await notificationService.NotifyIfInForeground(this, TextResources.ErrorText, TextResources.InvalidServerText);
                return;
            }

            if (string.IsNullOrEmpty(TennantName)) { 
                TennantName = endpintUri.ToString();
            }

            IsBusy = true;
            var success = false;
            try{
                success = await tenantService.Login(endpintUri, Username, Password, TennantName);
            }
            catch (Exception){}
            finally {
                IsBusy = false;
            }

            if (!success)
            {
                await notificationService.NotifyIfInForeground(this, TextResources.ErrorText, TextResources.LoginFailedText);
            }
            else {
                setupTcs.SetResult(true);
                onSuccessAction?.Invoke();
            }
        }

        private bool TryParseEndpoint(string endpoint, out Uri uri) { 
            return Uri.TryCreate(endpoint, UriKind.Absolute, out uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
