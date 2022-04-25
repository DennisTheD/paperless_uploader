using PaperlessClient.Mobile.Converters;
using PaperlessClient.Mobile.Converters.Interfaces;
using PaperlessClient.Mobile.Events;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public abstract class ViewModelBase : BindableObject
    {
        #region services
        protected INotificationService notificationService;
        protected ITenantService tenantService;
        #endregion

        #region props
        public Page Page { get; set; }

        protected List<Type> requiredConverters;
        protected List<ITenantAwareConverter> TenantAwareConverters { get; private set; }


        protected string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                var oldVal = title;
                title = value;
                if (oldVal != value)
                    OnPropertyChanged();
            }
        }

        protected bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        protected bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        #endregion

        #region commands
        private Command refreshCommand;
        public Command RefreshCommand
        {
            get { return refreshCommand; }
            set { refreshCommand = value; OnPropertyChanged(); }
        }
        #endregion

        public ViewModelBase(
            INotificationService notificationService
            , ITenantService tenantService)
        {
            this.notificationService = notificationService;
            this.tenantService = tenantService;

            requiredConverters = new List<Type>();
            TenantAwareConverters = new List<ITenantAwareConverter>();
            MessagingCenter.Subscribe<TenantChangedEvent>(this, nameof(TenantChangedEvent), async(e) => await HandleTenantChanged(e));
        }        

        #region lifecycle methods
        public abstract Task InitializeAsync(object parameter);

        public virtual Task OnReappearing(object parameter)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnDisappearing(object parameter)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region tenant awareness
        public void RequireConverter(params Type[] converterTypes) { 
            requiredConverters.AddRange(converterTypes);
        }

        public async Task RegisterTenantAwareConverter(ITenantAwareConverter converter)
        {
            TenantAwareConverters.Add(converter);


            if (requiredConverters.Count > 0) {
                var allConvertersReady = true;
                foreach (var converterType in requiredConverters) {
                    allConvertersReady = allConvertersReady && TenantAwareConverters.Any(c => c.GetType() == converterType);
                }

                if (allConvertersReady) {
                    try{
                        await Task.WhenAll(TenantAwareConverters.Select(c => c.UpdateDataSource()));
                    }
                    catch (Exception){}                    
                    OnConvertersReady();
                }
            }
        }

        private async Task HandleTenantChanged(TenantChangedEvent eventArgs)
        {
            OnTenantChanged(eventArgs);
            try {
                await Task.WhenAll(TenantAwareConverters.Select(c => c.UpdateDataSource()));
            }
            catch (Exception) {}            
            OnConvertersReady();
        }

        protected virtual void OnTenantChanged(TenantChangedEvent eventArgs) {
        }

        protected virtual void OnConvertersReady() {
        }
        #endregion

        protected async Task Refresh<T>(
            Func<Task<T>> fetchFunc
            , Func<IObservable<T>> getAndFetchFunc
            , Action<T> resultFunc
            , bool forceReload = false)
        {
            if (forceReload
                || getAndFetchFunc == null)
            {
                try
                {
                    var result = await fetchFunc();
                    IsRefreshing = false;
                    resultFunc(result);
                }
                catch (Exception)
                {
                    await notificationService.NotifyIfInForeground(this, "Fehler", "Die Daten konnten nicht aktuallisiert werden.");
                }
                finally
                {
                    IsRefreshing = false;
                }
            }
            else
            {
                getAndFetchFunc()
                    .Subscribe(
                        (a) => {
                            IsRefreshing = false;
                            resultFunc(a);
                        }
                        , (e) => {
                            notificationService.NotifyIfInForeground(this, "Fehler", "Die Daten konnten nicht aktuallisiert werden.");
                            IsRefreshing = false;
                        }
                        , () => {
                            IsRefreshing = false;
                        });
            }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
