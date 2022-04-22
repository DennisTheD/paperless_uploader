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
    public class IdsToTagNameListConverter : IValueConverter
    {
        private readonly ITagService tagService;

        public List<Tag> Tags { get; set; }

        public IdsToTagNameListConverter()
        {
            tagService = ServiceLocator.Resolve<ITagService>();
            tagService.GetAndFetchAllTags()
                .Subscribe(tags => Tags = tags);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<int> ids) {
                var tagNames = Tags?
                    .OrderBy(t => t.Name)
                    .Where(t => ids.Contains(t.Id))
                    .Select(t => t.Name);

                if (tagNames != null && tagNames.Count() > 0) { 
                    return string.Join("; ", tagNames);
                }
                return "";
            }

            return "INVALID";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
