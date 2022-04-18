using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Extensions
{
    public static class ByteArrayExtension
    {
        public static ImageSource ToImageSource(this byte[] imageBytes) {
            return ImageSource.FromStream(() =>
            {
                return new MemoryStream(imageBytes);
            });
        }
    }
}
