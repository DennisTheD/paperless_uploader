using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Models
{
    public class Tag : NamedApiEntity
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("is_inbox_tag")]
        public bool IsInboxTag { get; set; }

    }
}
