using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Events
{
    public class DefaultTenantChnagedEvent
    {
        public ApiSetup NewDefaultTenant { get; set; }
    }
}
