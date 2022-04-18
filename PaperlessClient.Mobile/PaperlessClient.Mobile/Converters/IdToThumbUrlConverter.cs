using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Converters
{
    public class IdToThumbUrlConverter : IValueConverter
    {
        private static readonly string THUMBNAIL_ENDPOINT = "api/documents/{id}/thumb/";
        private ITenantService tenantService;
        private ApiSetup apiSetup;

        public IdToThumbUrlConverter()
        {
            this.tenantService = ServiceLocator.Resolve<ITenantService>();
            this.apiSetup = tenantService.GetCurrentTennant();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id) {
                var endpoint = THUMBNAIL_ENDPOINT.Replace("{id}", id.ToString());
                return apiSetup.Endpoint + endpoint;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
