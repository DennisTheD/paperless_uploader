using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class PreferencesViewModel : ViewModelBase
    {
        private IAppAuthService appAuthService;
        private IPreferenceService preferenceService;

        #region props
        private bool ignoreLastToggle;
        private bool useAuthentication;
        public bool UseAuthentication {
            get => useAuthentication;
            set => SetProperty(ref useAuthentication, value);
        }
        #endregion

        #region commands
        private Command toggleAuthCommand;
        public Command ToggleAuthCommand {
            get {
                if (toggleAuthCommand == null) {
                    toggleAuthCommand = new Command(async (o) => await ToggleAuth());
                }
                return toggleAuthCommand;
            }
        }        
        #endregion

        public PreferencesViewModel(
            IAppAuthService appAuthService
            , INotificationService notificationService
            , IPreferenceService preferenceService
            , ITenantService tenantService) 
            : base(notificationService, tenantService)
        {
            this.appAuthService = appAuthService;
            this.preferenceService = preferenceService;
        }

        public override Task InitializeAsync(object parameter)
        {
            UseAuthentication = preferenceService.GetBoolPreference(AppPreference.USE_AUTHENTICATION);

            return Task.CompletedTask;
        }

        private async Task ToggleAuth()
        {
            if (ignoreLastToggle) {
                ignoreLastToggle = false;
                return;
            }                

            bool authSuccess = false;
            try
            {
                authSuccess = await appAuthService.AuthenticateUser();
            }
            catch (Exception) {}
            if (!authSuccess) {
                ignoreLastToggle = true;
                UseAuthentication = !UseAuthentication;
                return;
            }                

            preferenceService.SetBoolPreference(AppPreference.USE_AUTHENTICATION, UseAuthentication);
        }
    }
}
