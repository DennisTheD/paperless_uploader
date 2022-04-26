using PaperlessClient.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaperlessClient.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthenticationFailurePage : TopLevelContentPage<AuthenticationFailureViewModel>
    {
        public AuthenticationFailurePage()
        {
            InitializeComponent();
        }
    }
}