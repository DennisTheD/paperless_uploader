using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.NavigationHints;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.ViewModels
{
    public class TennantListViewModel : ListViewModelBase<ApiSetup>
    {
        private INavigationService navigationService;
        public TennantListViewModel(
            INavigationService navigationService
            , INotificationService notificationService
            , ITenantService tenantService) 
            
            : base(notificationService, tenantService.GetTennants, null, TennantFilter)
        {
            this.navigationService = navigationService;
            AddCommand = new Command(AddTennant);
        }
        
        public override async Task InitializeAsync(object parameter)
        {
            await Refresh(
                fetchFunc
                , getAndFetchFunc
                , (items) => {
                    Items = new ObservableCollection<ApiSetup>(items);
                    customChangeNotificationRegistrations.ForEach(p => OnPropertyChanged(p));
                }, true);
        }

        private async void AddTennant(object obj)
        {
            await navigationService.NavigateToAsync(
                nameof(SetupPage)
                , new SetupNavigationHint() { 
                    
                    OnSuccess = async () => { 
                        await navigationService.PopAsync();
                        await Refresh(
                            fetchFunc
                            , getAndFetchFunc
                            , (items) => {
                                Items = new ObservableCollection<ApiSetup>(items);
                                customChangeNotificationRegistrations.ForEach(p => OnPropertyChanged(p));
                        }, true);
                    }
                });
        }


        private static List<ApiSetup> TennantFilter(string arg1, List<ApiSetup> arg2)
        {
            return arg2;
        }
    }
}
