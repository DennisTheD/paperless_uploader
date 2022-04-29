using PaperlessClient.Mobile.Models;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IPreferenceService
    {
        bool GetBoolPreference(AppPreference preference);
        void SetBoolPreference(AppPreference preference, bool value);
    }
}
