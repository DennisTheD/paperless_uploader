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
        

        public AppShellViewModel(
            INotificationService notificationService
            , ITenantService tenantService)
            : base(notificationService)
        {
            this.tenantService = tenantService;

            MessagingCenter.Subscribe<TenantListChangedEvent>(this, nameof(TenantListChangedEvent), async(e) => { await RefreshTenantList(e); });
            MessagingCenter.Subscribe<TenantChangedEvent>(this, nameof(TenantChangedEvent), async (e) => { await RefreshTenantList(e); });
        }        

        public override async Task InitializeAsync(object parameter)
        {
            await RefreshTenantList(parameter);
        }

        private async Task RefreshTenantList(object obj)
        {
            Tenants = await tenantService.GetTennants();
        }

        private void ChangeTenant(object obj)
        {
            if (obj is ApiSetup tenant)
            {
                tenantService.ChangeTenant(tenant);
            }
        }
    }
}
