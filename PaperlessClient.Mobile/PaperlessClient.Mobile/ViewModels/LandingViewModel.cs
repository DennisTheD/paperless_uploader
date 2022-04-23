using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class LandingViewModel : ViewModelBase
    {
        private Command logoutCommand;
        public Command LogoutCommand {
            get {
                if (logoutCommand == null) {
                    logoutCommand = new Command(RequestLogout);
                }
                return logoutCommand;
            }
        }        

        public LandingViewModel(
            INotificationService notificationService
            , ITenantService tenantService) 
            : base(notificationService, tenantService)
        {
        }

        public override Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void RequestLogout(object obj)
        {
            MessagingCenter.Send(new LogoutRequest(), nameof(LogoutRequest));
        }
    }
}
