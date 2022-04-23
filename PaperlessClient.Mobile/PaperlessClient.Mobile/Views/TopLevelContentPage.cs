using PaperlessClient.Mobile.ViewModels;

namespace PaperlessClient.Mobile.Views
{
    public class TopLevelContentPage<T> : CustomContentPage<T> where T : ViewModelBase
    {
        public TopLevelContentPage()
            : base()
        {
            viewModel.Page = this;
            viewModel.InitializeAsync(null);
        }
    }
}
