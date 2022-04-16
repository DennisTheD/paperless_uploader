using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Events
{
    public class TenantListChangedEvent
    {
        public ApiSetup NewTenant { get; set; }
        public ApiSetup DeletedTenant { get; set; }
    }
}
