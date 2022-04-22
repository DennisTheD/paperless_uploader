using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Models
{
    public class Correspondent : NamedApiEntity
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("last_correspondence")]
        public DateTime? LastCorrespondence { get; set; }
    }
}
