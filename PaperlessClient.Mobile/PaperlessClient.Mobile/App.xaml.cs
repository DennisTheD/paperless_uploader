using PaperlessClient.Mobile.Events;
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
        private IPreferenceService preferenceService;

        public App()
        {
            InitializeComponent();
            // setup allready completed inside the platform specivic projects
            //ServiceLocator.Setup();
            MessagingCenter.Subscribe<FileUploadRequest>(this, nameof(FileUploadRequest), UploadRequestReceived);
            MessagingCenter.Subscribe<LoginRequiredEvent>(this, nameof(LoginRequiredEvent), LogoutRequestReceived);

            navigationService = ServiceLocator.Resolve<INavigationService>();
            tenantService = ServiceLocator.Resolve<ITenantService>();
            fileUploadQueueService = ServiceLocator.Resolve<IFileUploadQueueService>();
            preferenceService = ServiceLocator.Resolve<IPreferenceService>();

            MainPage = new LoadingPage();

#pragma warning disable CS4014
            InitializeAsync();
#pragma warning restore CS4014
        }

        private async void LogoutRequestReceived(LoginRequiredEvent loginRequiredEvent)
        {
            await InitializeAsync();
        }

        private async void UploadRequestReceived(FileUploadRequest uploadRequest)
        {
            await navigationService.InitializationTask;
            fileUploadQueueService.AddTask(uploadRequest);
            await navigationService.NavigateToAsync("//" + nameof(UploadFilePage)
                , new UploadFileNavigationHint() { DeleteFileAfterUpload = true, FileUri = uploadRequest.FileUri, Title = uploadRequest.FileTitle });
        }

        public async Task InitializeAsync() {
            await tenantService.InitializeAsync();
            await navigationService.InitializeAsync();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            if (!navigationService.IsLocked
                && preferenceService.GetBoolPreference(AppPreference.USE_AUTHENTICATION)) {
                navigationService.Lock();
            }
        }

        protected override void OnResume()
        {
        }
    }
}
