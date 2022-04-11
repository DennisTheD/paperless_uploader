using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Services
{
    internal class NotificationService : INotificationService
    {
        public async Task Notify(string title, string message, string cancel = "OK")
        {
            var currentPage = GetCurrentPage();
            await currentPage?.DisplayAlert(title, message, cancel);
        }

        public Task<bool> Notify(string title, string message, string accept, string cancel)
        {
            var currentPage = GetCurrentPage();
            return currentPage.DisplayAlert(title, message, accept, cancel);
        }

        public async Task NotifyIfInForeground(object sender, string title, string message, string cancel = "OK")
        {
            if (IsSenderInForeground(sender))
            {
                var currentPage = GetCurrentPage();
                await currentPage.DisplayAlert(title, message, cancel);
            }
        }

        public async Task<string> SelectActionIfInForeground(object sender, string title, string cancel = "Abbrechen", string destruction = null, params string[] options)
        {
            if (IsSenderInForeground(sender))
            {
                var currentPage = GetCurrentPage();
                return await currentPage.DisplayActionSheet(title, cancel, destruction, options);
            }
            return cancel;
        }

        public Task<string> DisplayPromptIfInForeground(object sender, string title, string message)
        {
            if (IsSenderInForeground(sender))
            {
                var currentPage = GetCurrentPage();
                return currentPage.DisplayPromptAsync(title, message);
            }
            return null;
        }

        private bool IsSenderInForeground(object sender)
        {

            var currentPage = GetCurrentPage();
            var currentViewModel = currentPage?.BindingContext as ViewModelBase;
            if (currentViewModel == null || sender != currentViewModel)
                return false; // app in foreground but page has changed

            return true;
        }

        private Page GetCurrentPage()
        {
            var currentPage = Application.Current.MainPage;
            if (currentPage == null)
                return null; // App not in foreground

            if (currentPage is Shell)
            {
                if (Shell.Current?.CurrentItem?.CurrentItem is IShellSectionController shellController
                    && shellController.PresentedPage != null)
                {
                    currentPage = shellController.PresentedPage;
                }
                else if (Shell.Current?.CurrentPage != null)
                {
                    currentPage = Shell.Current?.CurrentPage;
                }
            }
            else if (currentPage is TabbedPage tabbedPage)
            {
                currentPage = tabbedPage.CurrentPage;
            }
            return currentPage;
        }
    }
}
