using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public abstract class ListViewModelBase<TListEntity> : ViewModelBase
    {
        protected Func<Task<List<TListEntity>>> fetchFunc;
        protected Func<IObservable<List<TListEntity>>> getAndFetchFunc;

        protected List<string> customChangeNotificationRegistrations;

        private bool filteringEnabled;
        protected string filter1;
        private Func<string, Task<List<TListEntity>>> filteredFetchFunc;
        private Func<string, IObservable<List<TListEntity>>> filteredGetAndFetchFunc;

        protected bool cacheChangedInBackground;
        protected Func<string, List<TListEntity>, List<TListEntity>> filterFunc;

        protected ObservableCollection<TListEntity> items;
        public ObservableCollection<TListEntity> Items
        {
            get
            {
                if (string.IsNullOrWhiteSpace(searchText) || filterFunc == null)
                {
                    return items;
                }
                return new ObservableCollection<TListEntity>(filterFunc(searchText, items.ToList()));
            }
            protected set { items = value; OnPropertyChanged(); }
        }

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; OnPropertyChanged(); OnPropertyChanged(nameof(Items)); }
        }

        private TListEntity selectedItem;
        public TListEntity SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; OnPropertyChanged(); }
        }

        #region commands
        private Command itemSelectedCommand;
        public Command ItemSelectedCommand
        {
            get { return itemSelectedCommand; }
            protected set { itemSelectedCommand = value; OnPropertyChanged(); }
        }

        private Command addCommand;
        public Command AddCommand
        {
            get { return addCommand; }
            protected set { addCommand = value; OnPropertyChanged(); }
        }
        #endregion

        #region constructors
        private ListViewModelBase(
            INotificationService notificationService,
            Func<string, List<TListEntity>, List<TListEntity>> filterFunc = null)
            : base(notificationService)
        {
            this.filterFunc = filterFunc;
            customChangeNotificationRegistrations = new List<string>();
        }

        protected ListViewModelBase(
            INotificationService notificationService
            , Func<Task<List<TListEntity>>> fetchFunc
            , Func<IObservable<List<TListEntity>>> getAndFetchFunc
            , Func<string, List<TListEntity>, List<TListEntity>> filterFunc = null)
            : this(notificationService, filterFunc)
        {
            UpdateFetchFuncs(fetchFunc, getAndFetchFunc);
        }
        #endregion        

        public override Task OnReappearing(object parameter)
        {
            selectedItem = default(TListEntity);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(SelectedItem));
            });
            return base.OnReappearing(parameter);
        }

        public override async Task InitializeAsync(object parameter)
        {
            if (filteringEnabled)
            {
                if (string.IsNullOrWhiteSpace(filter1))
                    throw new Exception("For filtering enabled list pages, filed filer1 shoud be set in InitaliceAsync.");

                await Refresh<List<TListEntity>>(
                    async () => { return await filteredFetchFunc(filter1); }
                    , () => { return filteredGetAndFetchFunc(filter1); }
                    , (items) => {
                        Items = new ObservableCollection<TListEntity>(items);
                        customChangeNotificationRegistrations.ForEach(p => OnPropertyChanged(p));
                    });
            }
            else
            {
                await Refresh(
                fetchFunc
                , getAndFetchFunc
                , (items) => {
                    Items = new ObservableCollection<TListEntity>(items);
                    customChangeNotificationRegistrations.ForEach(p => OnPropertyChanged(p));
                });
            }
        }

        protected void UpdateFetchFuncs(
            Func<Task<List<TListEntity>>> fetchFunc
            , Func<IObservable<List<TListEntity>>> getAndFetchFunc)
        {

            this.fetchFunc = fetchFunc;
            this.getAndFetchFunc = getAndFetchFunc;

            RefreshCommand = new Command(async () => {
                await Refresh(
                fetchFunc
                , getAndFetchFunc
                , (items) => {
                    Items = new ObservableCollection<TListEntity>(items);
                    customChangeNotificationRegistrations.ForEach(p => OnPropertyChanged(p));
                }
                , true);
            });
        }

        protected void RegisterCustomChangeNotification(string propertyName)
        {
            customChangeNotificationRegistrations.Add(propertyName);
        }
    }
}
