using PaperlessClient.Mobile.Events;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Utils
{
    public class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        private ITenantService tenantService;

        public AuthenticatedHttpClientHandler()
        {
            tenantService = ServiceLocator.Resolve<ITenantService>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tenant = tenantService.GetCurrentTennant();
            request.Headers.Add("Authorization", "Token " + tenant?.Token ?? "");
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
