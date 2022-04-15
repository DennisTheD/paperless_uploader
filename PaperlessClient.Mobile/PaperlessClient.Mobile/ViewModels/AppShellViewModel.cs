using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class AppShellViewModel : ViewModelBase
    {
        private ITenantService tenantService;

        private List<ApiSetup> tenants;
        public List<ApiSetup> Tenants {
            get => tenants;
            set => SetProperty(ref tenants, value);
        }

        private Command changeTenantCommand;
        public Command ChnageTenantCommand {
            get {
                if (changeTenantCommand == null) {
                    changeTenantCommand = new Command(ChangeTenant);
                }
                return changeTenantCommand;
            }
        }

        private async void ChangeTenant(object obj)
        {
            if (obj is ApiSetup tenant) { 
                tenantService.ChangeTenant(tenant);
            }
            Tenants = await tenantService.GetTennants();
        }

        public AppShellViewModel(
            INotificationService notificationService
            , ITenantService tenantService)
            : base(notificationService)
        {
            this.tenantService = tenantService;
        }

        public override async Task InitializeAsync(object parameter)
        {
            Tenants = await tenantService.GetTennants();
        }
    }
}
