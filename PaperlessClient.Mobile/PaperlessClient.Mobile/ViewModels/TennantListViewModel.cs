using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.NavigationHints;
using PaperlessClient.Mobile.Resources;
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
        private ITenantService tenantService;

        #region Commands
        private Command deleteTenantCommand;
        public Command DeleteTenantCommand {
            get {
                if (deleteTenantCommand == null) {
                    deleteTenantCommand = new Command(async (o) => { await DeleteTenant(o); });
                }
                return deleteTenantCommand;
            }
        }

        private Command setDefaultTenantCommand;
        public Command SetDefaultTenantCommand
        {
            get
            {
                if (setDefaultTenantCommand == null)
                {
                    setDefaultTenantCommand = new Command(SetDefaultTenant);
                }
                return setDefaultTenantCommand;
            }
        }
        #endregion


        public TennantListViewModel(
            INavigationService navigationService
            , INotificationService notificationService
            , ITenantService tenantService) 
            
            : base(notificationService, tenantService.GetTennants, null, TennantFilter)
        {
            this.navigationService = navigationService;
            this.tenantService = tenantService;
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

        private async Task DeleteTenant(object obj)
        {
            if (!(obj is ApiSetup tenant)) {
                return;
            }

            var confirmationResult = await notificationService.SelectActionIfInForeground(
                this
                , TextResources.DeleteTenantConfirmationText
                , TextResources.NoText
                , TextResources.YesText);

            if (!string.IsNullOrWhiteSpace(confirmationResult)
                && confirmationResult == TextResources.YesText) {

                await tenantService.DeleteTenant(tenant);
                await InitializeAsync(null);
            }
        }

        private void SetDefaultTenant(object obj)
        {
            if (obj is ApiSetup tenant) { 
                tenantService.SetDefaultTenant(tenant);
            }
        }

        private static List<ApiSetup> TennantFilter(string arg1, List<ApiSetup> arg2)
        {
            return arg2;
        }
    }
}
