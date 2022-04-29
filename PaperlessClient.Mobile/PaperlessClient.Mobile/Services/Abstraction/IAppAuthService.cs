using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IAppAuthService
    {
        bool SupportsBiometricAuthentication();

        Task<bool> AuthenticateUser();
    }
}
