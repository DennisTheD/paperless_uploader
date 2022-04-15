using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.TemplateSelectors
{
    public class TenantListDisplaySelector : DataTemplateSelector
    {
        private ITenantService tenantService;

        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ActiveTemplate { get; set; }

        public TenantListDisplaySelector()
        {
            tenantService = ServiceLocator.Resolve<ITenantService>();
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var activeTenant = tenantService.GetCurrentTennant();
            if (item is ApiSetup tenant) {
                if (tenant.Endpoint == activeTenant.Endpoint) {
                    return ActiveTemplate;
                }
            }

            return DefaultTemplate;
        }
    }
}
