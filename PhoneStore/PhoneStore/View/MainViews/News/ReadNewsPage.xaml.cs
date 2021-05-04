using PhoneStore.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhoneStore.View.MainViews.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadNewsPage : ContentPage
    {
        public ReadNewsPage(string link)
        {
            InitializeComponent();
            ReadNewsViewModel vm = new ReadNewsViewModel(link);
            this.BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await progressBar.ProgressTo(0.9, 900, Easing.SpringIn);
        }
        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            progressBar.IsVisible = true;
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            progressBar.IsVisible = false;
        }
    }
}