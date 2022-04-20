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
    public class IdToDocumentTypeNameConverter : IValueConverter
    {
        public IdToDocumentTypeNameConverter()
        {
            var docTypeService = ServiceLocator.Resolve<IDocumentTypeService>();
            docTypeService.GetAndFetchAllDocumentTypes()
                .Subscribe((d) => { DocumentTypes = d; });
        }

        public List<DocumentType> DocumentTypes { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id) {
                var docType = DocumentTypes?.Where(d => d.Id == id).FirstOrDefault();
                if (docType != null) { 
                    return docType.Name;
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
