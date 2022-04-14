using PaperlessClient.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface ITenantService
    {

        Task InitializeAsync();
        ApiSetup GetCurrentTennant();
        Task<bool> Login(
            Uri endpoint
            , string username
            , string password
            , string tennantName
            , bool setAsDefault = false);

        Task<List<ApiSetup>> GetTennants();
    }
}
