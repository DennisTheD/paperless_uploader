using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.ViewModels
{
    public class LandingViewModel : ViewModelBase
    {
        public LandingViewModel(
            INotificationService notificationService) 
            : base(notificationService)
        {
        }

        public override Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }
    }
}
