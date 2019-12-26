using PhoneStore.Models;
using PhoneStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhoneStore.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        public NewsPage(RssFeedModel feed)
        {
            InitializeComponent();
            RssFeedViewModel vm = new RssFeedViewModel(feed);
            this.BindingContext = vm;
        }
    }
}