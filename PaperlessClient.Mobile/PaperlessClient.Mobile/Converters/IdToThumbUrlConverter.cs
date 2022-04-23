using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Converters
{
    public class IdToThumbUrlConverter : TenantAwareBaseConverter<Document>
    {
        private static readonly string THUMBNAIL_ENDPOINT = "api/documents/{id}/thumb/";       

        public IdToThumbUrlConverter()
        {
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id) {
                var endpoint = THUMBNAIL_ENDPOINT.Replace("{id}", id.ToString());
                return tenantService.GetCurrentTennant().Endpoint + endpoint;
            }
            return null;
        }

        public override Task UpdateDataSource()
            => Task.CompletedTask;
    }
}
