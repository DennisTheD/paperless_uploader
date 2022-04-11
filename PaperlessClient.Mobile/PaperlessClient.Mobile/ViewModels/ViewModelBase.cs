using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public abstract class ViewModelBase : BindableObject
    {
        protected INotificationService notificationService;
        public Page Page { get; set; }

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
            get
            {
                return _isBusy;
            }
            set
            {
                var oldVal = _isBusy;
                _isBusy = value;
                if (oldVal != value)
                    OnPropertyChanged();
            }
        }

        private Command refreshCommand;
        public Command RefreshCommand
        {
            get { return refreshCommand; }
            set { refreshCommand = value; OnPropertyChanged(); }
        }

        public ViewModelBase(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public abstract Task InitializeAsync(object parameter);

        public virtual Task OnReappearing(object parameter)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnDisappearing(object parameter)
        {
            return Task.CompletedTask;
        }

        protected async Task Refresh<T>(
            Func<Task<T>> fetchFunc
            , Func<IObservable<T>> getAndFetchFunc
            , Action<T> resultFunc
            , bool forceReload = false)
        {
            if (forceReload)
            {
                try
                {
                    IsBusy = true;
                    var result = await fetchFunc();
                    IsBusy = false;
                    resultFunc(result);
                }
                catch (Exception)
                {
                    await notificationService.NotifyIfInForeground(this, "Fehler", "Die Daten konnten nicht aktuallisiert werden.");
                }
                finally
                {
                    IsBusy = false;
                }
            }
            else
            {
                IsBusy = true;
                getAndFetchFunc()
                    .Subscribe(
                        (a) => {
                            IsBusy = false;
                            resultFunc(a);
                        }
                        , (e) => {
                            notificationService.NotifyIfInForeground(this, "Fehler", "Die Daten konnten nicht aktuallisiert werden.");
                            IsBusy = false;
                        }
                        , () => {
                            IsBusy = false;
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
