using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Converters.Interfaces
{
    public interface ITenantAwareConverter
    {
        Task UpdateDataSource();
    }
}
