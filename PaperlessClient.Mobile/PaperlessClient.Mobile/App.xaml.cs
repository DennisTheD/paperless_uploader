﻿using PaperlessClient.Mobile.Models;
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
        private IApiService apiService;

        public App()
        {
            InitializeComponent();
            ServiceLocator.Setup();
            MessagingCenter.Subscribe<FileUploadRequest>(this, nameof(FileUploadRequest), UploadRequestReceived);
            MessagingCenter.Subscribe<LogoutRequest>(this, nameof(LogoutRequest), LogoutRequestReceived);

            navigationService = ServiceLocator.Resolve<INavigationService>();
            apiService = ServiceLocator.Resolve<IApiService>();

            MainPage = new LoadingPage();

#pragma warning disable CS4014
            InitializeAsync();
#pragma warning restore CS4014
        }

        private async void LogoutRequestReceived(LogoutRequest obj)
        {
            await apiService.Logout();
            await InitializeAsync();
        }

        private async void UploadRequestReceived(FileUploadRequest uploadRequest)
        {
            await navigationService.InitializationTask;
            await navigationService.NavigateToAsync(
                nameof(UploadFilePage)
                , new UploadFileNavigationHint()
                {
                    FileUri = uploadRequest.FileUri,
                    Title = uploadRequest.FileTitle,
                    DeleteFileAfterUpload = true
                });
        }

        private async Task InitializeAsync() {
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
