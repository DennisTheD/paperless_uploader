using Foundation;
using PaperlessClient.Mobile.Components;
using PaperlessClient.Mobile.iOS.CustomRenderer;
using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AuthorizedWebView), typeof(AuthorizedWebViewRenderer))]
namespace PaperlessClient.Mobile.iOS.CustomRenderer
{
    public class AuthorizedWebViewRenderer : ViewRenderer<AuthorizedWebView, UIWebView>
    {
        private ITenantService tenantService;
        public AuthorizedWebViewRenderer()
        {
            this.tenantService = ServiceLocator.Resolve<ITenantService>();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AuthorizedWebView> e)
        {
            base.OnElementChanged(e);

            var webView = Control as UIWebView;

            if (webView == null)
            {
                webView = new UIWebView();
                SetNativeControl(webView);
            }

            webView.ScalesPageToFit = true;
            webView.LoadError += OnLoadError;
            webView.LoadStarted += OnLoadStart;
            webView.LoadFinished += OnLoadFinished;
        }        

        private void OnLoadStart(object sender, EventArgs e)
        {
            Element.LoadStartCommand?.Execute(e);
        }

        private void OnLoadFinished(object sender, EventArgs e)
        {
            Element.LoadFinishedCommand?.Execute(e);
        }

        private void OnLoadError(object sender, UIWebErrorArgs e)
        {
            Element.LoadErrorCommand?.Execute(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(Element.Source)
                && Element.Source != null) {
                
                UrlWebViewSource source = (Xamarin.Forms.UrlWebViewSource)Element.Source;
                var webRequest = new NSMutableUrlRequest(new NSUrl(source.Url));
                var headerKey = new NSString("Authorization");
                var headerValue = new NSString($"Token {tenantService.GetCurrentTennant().Token}");
                var dictionary = new NSDictionary(headerKey, headerValue);

                webRequest.Headers = dictionary;

                this.Control.LoadRequest(webRequest);
            }
        }
    }
}