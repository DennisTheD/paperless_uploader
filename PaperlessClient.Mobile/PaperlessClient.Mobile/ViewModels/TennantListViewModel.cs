using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.ViewModels
{
    public class TennantListViewModel : ListViewModelBase<ApiSetup>
    {
        public TennantListViewModel(
            INotificationService notificationService
            , ITenantService tenantService) 
            
            : base(notificationService, tenantService.GetTennants, null, TennantFilter)
        {
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

        private static List<ApiSetup> TennantFilter(string arg1, List<ApiSetup> arg2)
        {
            return arg2;
        }
    }
}
