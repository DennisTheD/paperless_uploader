using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Models
{
    public class DocumentType : NamedApiEntity
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("match")]
        public string Match { get; set; }
        
        [JsonProperty("matching_algorithm")]
        public int MatchingAlgorithm { get; set; }
        
        [JsonProperty("is_insensitive")]
        public bool IsInsensitive { get; set; }
        
        [JsonProperty("document_count")]
        public int DocumentCount { get; set; }
    }
}
