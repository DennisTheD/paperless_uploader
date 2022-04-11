using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Models
{
    public class FileUploadRequest
    {
        public Uri FileUri { get; set; }
        public string LocalPath => FileUri?.LocalPath;
        public string FileTitle { get; set; }

        public FileUploadRequest(string fileUri)
        {
            FileUri = new Uri(fileUri);
        }       
        

    }
}
