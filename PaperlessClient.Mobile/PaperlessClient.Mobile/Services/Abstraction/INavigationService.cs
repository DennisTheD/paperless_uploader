using PaperlessClient.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface INavigationService
    {
        Task InitializationTask { get; }
        ViewModelBase PreviousPageViewModel { get; }
        ViewModelBase ActiveViewModel { get; set; }
        Task InitializeAsync();
        Task NavigateToAsync(string uri);
        Task NavigateToAsync(string uri, object parameter);
        Task NavigateToAndAndPopAsync(string uri, object parameter);
        Task PopAsync();
    }
}
