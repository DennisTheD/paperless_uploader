using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.NavigationHints
{
    public class UploadFileNavigationHint
    {
        public Uri FileUri { get; set; }
        public bool DeleteFileAfterUpload { get; set; }
    }
}
