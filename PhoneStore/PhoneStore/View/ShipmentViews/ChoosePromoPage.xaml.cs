using PhoneStore.Models;
using PhoneStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhoneStore.View.ShipmentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePromoPage : ContentPage
    {
        public ChoosePromoPage()
        {
            InitializeComponent();
        }
        public ChoosePromoPage(OrderModel order)
        {
            InitializeComponent();
            ChoosePromoViewModel vm = new ChoosePromoViewModel(order);
            this.BindingContext = vm;
        }
    }
}