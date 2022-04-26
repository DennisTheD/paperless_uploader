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
            MessagingCenter.Subscribe<AppAuthRequiredEvent>(this, nameof(AppAuthRequiredEvent), async (o) => await InitializeAsync());

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
            var authRequired = preferenceService.GetBoolPreference(AppPreference.USE_AUTHENTICATION);
            var authSuccess = false;
            if (authRequired) {
                var auth = ServiceLocator.Resolve<IAppAuthService>();
                try
                {
                    authSuccess = await auth.AuthenticateUser();
                }
                catch (Exception) { }                
            }

            if (!authRequired || authSuccess)
            {
                await tenantService.InitializeAsync();
                await navigationService.InitializeAsync();
            }
            else {
                if (MainPage == null || MainPage.GetType() != typeof(AuthenticationFailurePage)) {
                    MainPage = new AuthenticationFailurePage();
                }
            }
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
