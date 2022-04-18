using Newtonsoft.Json;

namespace PaperlessClient.Mobile.Models
{
    public class ApiEntity
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
