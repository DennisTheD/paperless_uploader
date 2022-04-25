using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Components
{
    public class AuthorizedWebView : WebView
    {
        #region commands
        public static readonly BindableProperty LoadStartCommandProperty =
            BindableProperty.Create(nameof(LoadStartCommand), typeof(Command), typeof(AuthorizedWebView), default(Command));
        public Command LoadStartCommand
        {
            get => (Command)GetValue(LoadStartCommandProperty);
            set => SetValue(LoadStartCommandProperty, value);
        }

        public static readonly BindableProperty LoadFinishedCommandProperty =
            BindableProperty.Create(nameof(LoadFinishedCommand), typeof(Command), typeof(AuthorizedWebView), default(Command));
        public Command LoadFinishedCommand
        {
            get => (Command)GetValue(LoadFinishedCommandProperty);
            set => SetValue(LoadFinishedCommandProperty, value);
        }

        // this needs to get called from the platform specific renderer
        public static readonly BindableProperty LoadErrorCommandProperty =
            BindableProperty.Create(nameof(LoadErrorCommand), typeof(Command), typeof(AuthorizedWebView), default(Command));
        public Command LoadErrorCommand
        {
            get => (Command)GetValue(LoadErrorCommandProperty);
            set => SetValue(LoadErrorCommandProperty, value);
        }
        #endregion
    }
}
