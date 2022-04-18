using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Models
{
    public class Document : ApiEntity
    {        
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("tags")]
        public List<int> Tags { get; set; }
        [JsonProperty("document_type")]
        public int? DocumentType { get; set; }
        [JsonProperty("correspondent")]
        public int? Correspondent { get; set; }
        [JsonProperty("created")]
        public DateTime Created { get; set; }
        [JsonProperty("added")]
        public DateTime Added { get; set; }
        [JsonProperty("archive_serial_number")]
        public string ArchiveSerialNumber { get; set; }
        [JsonProperty("original_file_name")]
        public string OriginalFileName { get; set; }
        [JsonProperty("archived_file_name")]
        public string ArchivedFileName { get; set; }

    }
}
