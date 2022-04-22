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
    public class IdToCorrespondentNameConverter : IValueConverter
    {
        private ICorrespondentService correspondentService;
        private List<Correspondent> correspondents;
        public IdToCorrespondentNameConverter()
        {
            correspondentService = ServiceLocator.Resolve<ICorrespondentService>();
            correspondentService.GetAndFetchAllCorrespondents()
                .Subscribe(correspondents => this.correspondents = correspondents);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id) {
                return correspondents?.Where(c => c.Id == id).FirstOrDefault()?.Name;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
