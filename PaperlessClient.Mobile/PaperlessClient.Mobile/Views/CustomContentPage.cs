using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Views
{
    public class CustomContentPage<T> : ContentPage where T : ViewModelBase
    {
        protected T viewModel;

        public CustomContentPage()
            : base()
        {
            MethodInfo targetMethod = typeof(ServiceLocator).GetMethod(nameof(ServiceLocator.Resolve));
            var vmResolver = targetMethod.MakeGenericMethod(typeof(T));
            BindingContext = viewModel = (T)vmResolver.Invoke(null, null);
        }
    }
}
