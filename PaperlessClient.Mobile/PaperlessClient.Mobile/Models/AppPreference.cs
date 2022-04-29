using PaperlessClient.Mobile.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Models
{
    public enum AppPreference
    {
        [Preference(typeof(bool), false)]
        USE_AUTHENTICATION
    }
}
