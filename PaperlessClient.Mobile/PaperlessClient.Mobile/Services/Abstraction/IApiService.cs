using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IApiService
    {
        Task<T> Get<T>(string endpoint, Dictionary<string, string> parameters = null, CancellationToken cancellationToken = default(CancellationToken));
        Task UploadInForeground(Uri fileUri, string documentTitle = null);
    }
}
