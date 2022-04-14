using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.ViewModels;
using PaperlessClient.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Services
{
    public class NavigationService : INavigationService
    {
        private IApiService apiService;        

        public ViewModelBase ActiveViewModel { get; set; }
        public ViewModelBase PreviousPageViewModel { get; set; }

        private TaskCompletionSource<bool> initializationTcs = new TaskCompletionSource<bool>();
        public Task InitializationTask => initializationTcs.Task;


        public NavigationService(IApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task InitializeAsync()
        {
            if (await apiService.IsSetupComplete())
            {
                var shell = new AppShell();
                shell.OnNavigatingBackwards += Shell_OnNavigatingBackwards;
                Application.Current.MainPage = shell;
                initializationTcs.SetResult(true);
            }
            else {
                // initialization was completed before, but a logout was completed
                // reset the initialization state
                if (initializationTcs.Task.IsCompleted) { 
                    initializationTcs = new TaskCompletionSource<bool> ();
                }

                SetupPage setupPage;
                Application.Current.MainPage 
                    = setupPage
                    = new SetupPage();

                var setupVm = setupPage.BindingContext as SetupViewModel;
                await setupVm.SetupTask;
                await InitializeAsync();
            }
        }

        public Task NavigateToAsync(string uri)
        {
            return InternalNavigateToAsync(uri, null);
        }

        public Task NavigateToAsync(string uri, object parameter)
        {
            return InternalNavigateToAsync(uri, parameter);
        }

        private async Task InternalNavigateToAsync(string uri, object parameter)
        {
            // wait with any navigation task while init is not completed 
            await InitializationTask;

            PreviousPageViewModel = ActiveViewModel;
            var currentShell = Application.Current.MainPage as Shell;
            if (currentShell == null)
            {
                var shell = new AppShell();
                shell.OnNavigatingBackwards += Shell_OnNavigatingBackwards;
                Application.Current.MainPage = shell;
            }
            await Shell.Current.GoToAsync(uri);

            // try to initialize the new page
            var page = (Shell.Current?.CurrentItem?.CurrentItem as IShellSectionController)?.PresentedPage;
            ActiveViewModel = page?.BindingContext as ViewModelBase;
            // pass the current page to the vm            
            if (ActiveViewModel != null)
            {
                ActiveViewModel.Page = page;
            }
            await (page?.BindingContext as ViewModelBase)?.InitializeAsync(parameter);
        }

        private async void Shell_OnNavigatingBackwards(object sender, ShellNavigatedEventArgs e)
        {
            ActiveViewModel?.OnDisappearing(null);
            PreviousPageViewModel = ActiveViewModel;
            var page = (Shell.Current?.CurrentItem?.CurrentItem as IShellSectionController)?.PresentedPage;
            ActiveViewModel = page?.BindingContext as ViewModelBase;
            await ActiveViewModel?.OnReappearing(e?.Previous);
        }

        public async Task NavigateToAndAndPopAsync(string uri, object parameter)
        {
            var currentPage = Shell.Current?.Navigation?.NavigationStack.LastOrDefault();
            await InternalNavigateToAsync(uri, parameter);
            if (currentPage != null)
            {
                Shell.Current.Navigation.RemovePage(currentPage);
            }
        }

        public Task PopAsync()
        {
            return Shell.Current?.Navigation.PopAsync();
        }
    }
}
