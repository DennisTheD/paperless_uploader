using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class LockViewModel : ViewModelBase
    {
        private IAppAuthService appAuthService;
        private INavigationService navigationService;
        private IPreferenceService preferenceService;

        private Command retryAuthCommand;
        public Command RetryAuthCommand {
            get {
                if (retryAuthCommand == null) {
                    retryAuthCommand = new Command(async (o) => await Auhenticate());
                }
                return retryAuthCommand;
            }
        }        

        public LockViewModel(
            IAppAuthService appAuthService
            , INavigationService navigationService
            , INotificationService notificationService
            , IPreferenceService preferenceService
            , ITenantService tenantService) 
            : base(notificationService, tenantService)
        {
            this.appAuthService = appAuthService;
            this.navigationService = navigationService;
            this.preferenceService = preferenceService;
        }

        public override async Task InitializeAsync(object parameter)
        {
            if (!preferenceService.GetBoolPreference(AppPreference.USE_AUTHENTICATION)) {
                await navigationService.Unlock();
            }
        }

        private async Task Auhenticate()
        {
            var success = await appAuthService.AuthenticateUser();
            if (success) {
                await navigationService.Unlock();
            }
        }
    }
}
