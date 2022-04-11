using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IApiService
    {
        Task<bool> IsSetupComplete();
        Task<bool> Login(Uri endpoint, string username, string password);
        Task<bool> UploadInForeground(Uri fileUri, string documentTitle = null);
    }
}
