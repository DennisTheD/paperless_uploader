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
    public class IdToCorrespondentNameConverter : TenantAwareBaseConverter<Correspondent>
    {
        private ICorrespondentService correspondentService;

        public IdToCorrespondentNameConverter()
        {
            correspondentService = ServiceLocator.Resolve<ICorrespondentService>();
            UpdateFetchFunc(correspondentService.GetAndFetchAllCorrespondents);
        }
    }
}
