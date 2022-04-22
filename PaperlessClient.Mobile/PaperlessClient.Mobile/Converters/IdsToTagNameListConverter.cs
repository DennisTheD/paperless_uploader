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
    public class IdsToTagNameListConverter : TenantAwareBaseConverter<Tag>
    {
        public IdsToTagNameListConverter()
        {
            var tagService = ServiceLocator.Resolve<ITagService>();
            UpdateFetchFunc(tagService.GetAndFetchAllTags);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<int> ids) {
                var tagNames = Items?
                    .OrderBy(t => t.Name)
                    .Where(t => ids.Contains(t.Id))
                    .Select(t => t.Name);

                if (tagNames != null && tagNames.Count() > 0) { 
                    return string.Join("; ", tagNames);
                }
                return "";
            }

            return "INVALID INPUT";
        }
    }
}
