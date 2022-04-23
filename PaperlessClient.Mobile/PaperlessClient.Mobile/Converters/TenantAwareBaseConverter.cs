using PaperlessClient.Mobile.Converters.Interfaces;
using PaperlessClient.Mobile.Events;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Converters
{
    public abstract class TenantAwareBaseConverter<TEntity> : BindableObject, IValueConverter, ITenantAwareConverter
        where TEntity : ApiEntity
    {
        private Func<IObservable<List<TEntity>>> getAndFetchFunc;
        protected ITenantService tenantService;

        protected List<TEntity> Items { get; private set; }


        // use this property to get access to the pages vm and register the converter
        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create(nameof(ViewModel), typeof(ViewModelBase), typeof(TenantAwareBaseConverter<TEntity>), default(ViewModelBase), propertyChanged: OnViewModelPropertyChanged);        

        public ViewModelBase ViewModel {
            get => (ViewModelBase)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value); 
        }


        public TenantAwareBaseConverter()
        {
            tenantService = ServiceLocator.Resolve<ITenantService>();            
        }

        #region IValueConverter implementation
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (Items is IEnumerable<NamedApiEntity> namedItems
                && value is  int id) {
                return namedItems?.Where(c => c.Id == id).FirstOrDefault()?.Name;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Datasource 
        protected virtual void UpdateFetchFunc(
            Func<IObservable<List<TEntity>>> getAndFetchFunc)
        {
            this.getAndFetchFunc = getAndFetchFunc;
        }        

        
        #endregion

        #region ITenantAwareConverter implementation
        // register the converter at the vm
        private static void OnViewModelPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is ViewModelBase viewModel) {
                viewModel.RegisterTenantAwareConverter(bindable as TenantAwareBaseConverter<TEntity>);
            }
        }

        public virtual Task UpdateDataSource()
        {
            var tcs = new TaskCompletionSource<bool>();
            if (getAndFetchFunc != null)
            {
                getAndFetchFunc()
                    .Subscribe(

                        onNext: (items) =>
                        {
                            Items = items;
                            tcs.TrySetResult(true);
                        }

                        , onError: (e) =>
                        {
                            tcs.TrySetException(e);
                        });
            }
            else { 
                tcs.TrySetResult(false);
            }
            return tcs.Task; 
        }

        #endregion
    }
}
