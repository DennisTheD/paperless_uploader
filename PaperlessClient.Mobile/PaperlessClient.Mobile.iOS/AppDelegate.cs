using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using FFImageLoading;
using Foundation;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.Utils;
using UIKit;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            // setup the service locator here because its allready needed for image authentication 
            ServiceLocator.Setup();

            // initializ image loader
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            // setup authentication
            ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration
            {
                HttpClient = new HttpClient(new AuthenticatedHttpClientHandler())
            });

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            // create a copy of this file to avoid messing with ios security mechanisms
            var tmpFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var uri = new Uri(url.ToString());            

            try
            {
                File.Copy(uri.LocalPath, tmpFile);
                File.Delete(uri.LocalPath);
            }
            catch (Exception){
                return false;
            }

            var uploadRequest = new FileUploadRequest(tmpFile) { 
                FileTitle = Path.GetFileNameWithoutExtension(uri.LocalPath)
            };
            MessagingCenter.Send(uploadRequest, nameof(FileUploadRequest));
            return true;
        }
    }
}
