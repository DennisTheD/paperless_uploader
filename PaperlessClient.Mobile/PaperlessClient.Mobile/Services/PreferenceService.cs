using PaperlessClient.Mobile.Attributes;
using PaperlessClient.Mobile.Extensions;
using PaperlessClient.Mobile.Models;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace PaperlessClient.Mobile.Services
{
    public class PreferenceService : IPreferenceService
    {
        public bool GetBoolPreference(AppPreference preference) {
            var prefAttribute = preference.GetAttributeOfType<PreferenceAttribute>();
            var defaultValue = (bool)prefAttribute.DefaultValue;
            return Preferences.Get(preference.ToString(), defaultValue);
        }

        public void SetBoolPreference(AppPreference preference, bool value) { 
            Preferences.Set(preference.ToString(), value);
        }
    }
}
