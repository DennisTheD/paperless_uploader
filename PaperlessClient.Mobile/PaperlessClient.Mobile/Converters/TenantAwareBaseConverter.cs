using PaperlessClient.Mobile.Events;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Converters
{
    public abstract class TenantAwareBaseConverter<TEntity> : IValueConverter
        where TEntity : ApiEntity
    {
        private Func<IObservable<List<TEntity>>> getAndFetchFunc;

        protected List<TEntity> Items { get; private set; }
        protected ApiSetup Tenant { get; private set; }

        public TenantAwareBaseConverter()
        {
            var tenantService = ServiceLocator.Resolve<ITenantService>();
            Tenant = tenantService.GetCurrentTennant();

            MessagingCenter.Subscribe<TenantChangedEvent>(this, nameof(TenantChangedEvent), (e) => {
                Tenant = e.NewTenant;
                UpdateItems();
            });            
        }

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

        protected virtual void UpdateFetchFunc(
            Func<IObservable<List<TEntity>>> getAndFetchFunc)
        {
            this.getAndFetchFunc = getAndFetchFunc;
            UpdateItems();
        }

        protected virtual void UpdateItems() {
            if (getAndFetchFunc != null) {
                getAndFetchFunc()
                    .Subscribe(i => Items = i);
            }
        }
    }
}
