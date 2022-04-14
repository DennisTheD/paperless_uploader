using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IApiService
    {
        Task UploadInForeground(Uri fileUri, string documentTitle = null);
    }
}
