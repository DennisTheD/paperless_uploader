using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.NavigationHints;
using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile
{
    public partial class App : Application
    {
        private INavigationService navigationService;      
        private ITenantService tenantService;
        private IFileUploadQueueService fileUploadQueueService;

        public App()
        {
            InitializeComponent();
            ServiceLocator.Setup();
            MessagingCenter.Subscribe<FileUploadRequest>(this, nameof(FileUploadRequest), UploadRequestReceived);
            MessagingCenter.Subscribe<LogoutRequest>(this, nameof(LogoutRequest), LogoutRequestReceived);

            navigationService = ServiceLocator.Resolve<INavigationService>();
            tenantService = ServiceLocator.Resolve<ITenantService>();
            fileUploadQueueService = ServiceLocator.Resolve<IFileUploadQueueService>();

            MainPage = new LoadingPage();

#pragma warning disable CS4014
            InitializeAsync();
#pragma warning restore CS4014
        }

        private async void LogoutRequestReceived(LogoutRequest obj)
        {
            await InitializeAsync();
        }

        private async void UploadRequestReceived(FileUploadRequest uploadRequest)
        {
            await navigationService.InitializationTask;
            fileUploadQueueService.AddTask(uploadRequest);
            await navigationService.NavigateToAsync("//" + nameof(UploadFilePage));
        }

        private async Task InitializeAsync() {
            await tenantService.InitializeAsync();
            await navigationService.InitializeAsync();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
