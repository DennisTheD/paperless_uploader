using PaperlessClient.Mobile.Events;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class AuthenticationFailureViewModel : ViewModelBase
    {
        private Command retryAuthCommand;
        public Command RetryAuthCommand {
            get {
                if (retryAuthCommand == null) {
                    retryAuthCommand = new Command(RetryAuth);
                }
                return retryAuthCommand;
            }
        }        

        public AuthenticationFailureViewModel(
            INotificationService notificationService
            , ITenantService tenantService) 
            : base(notificationService, tenantService)
        {
        }

        public override Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void RetryAuth(object obj)
        {
            MessagingCenter.Send(new AppAuthRequiredEvent(), nameof(AppAuthRequiredEvent));
        }
    }
}
