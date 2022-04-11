using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Models
{
    public class ApiLoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }

    }
}
