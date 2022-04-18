using Newtonsoft.Json;
using System.Collections.Generic;

namespace PaperlessClient.Mobile.Models
{
    public class ApiListResponse<TEntity>
        where TEntity : ApiEntity
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("next")]
        public string Next { get; set; }
        [JsonProperty("previous")]
        public string Previous { get; set; }
        [JsonProperty("results")]
        public List<TEntity> Results { get; set; }
    }
}
