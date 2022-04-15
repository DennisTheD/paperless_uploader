using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
