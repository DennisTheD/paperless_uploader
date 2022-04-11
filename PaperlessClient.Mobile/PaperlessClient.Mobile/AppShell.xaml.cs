using PaperlessClient.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaperlessClient.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(UploadFilePage), typeof(UploadFilePage));
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);
            //if (args.Source == ShellNavigationSource.Pop)
            //{
            //    OnNavigatingBackwards?.Invoke(this, args);
            //}
        }

        public event EventHandler<ShellNavigatedEventArgs> OnNavigatingBackwards;
    }
}