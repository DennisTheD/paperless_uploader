using Newtonsoft.Json;

namespace PaperlessClient.Mobile.Models
{
    public class NamedApiEntity : ApiEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
