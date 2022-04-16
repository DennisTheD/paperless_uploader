using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Events
{
    public class TenantChangedEvent
    {
        public ApiSetup NewTenant { get; set; }

        public TenantChangedEvent(ApiSetup newTenant)
        {
            NewTenant = newTenant;
        }
    }
}
