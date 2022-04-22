using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Models
{
    public class Tag : ApiEntity
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("is_inbox_tag")]
        public bool IsInboxTag { get; set; }

    }
}
