using PhoneStore.Models;
using PhoneStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhoneStore.View.DetailViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DescriptionPage : ContentPage
    {
        public DescriptionPage(ItemModel item)
        {
            InitializeComponent();
            ItemDetailViewModel viewModel = new ItemDetailViewModel(item);
            this.BindingContext = viewModel;
        }
    }
}