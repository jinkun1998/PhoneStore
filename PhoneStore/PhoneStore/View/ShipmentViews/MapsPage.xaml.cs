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
    public partial class MapsPage : ContentPage
    {
        public MapsPage()
        {
            InitializeComponent();
            MapsViewModel vm = new MapsViewModel();
            this.BindingContext = vm;
        }
        public MapsPage(OrderModel order)
        {
            InitializeComponent();
            MapsViewModel vm = new MapsViewModel(order);
            this.BindingContext = vm;
        }
    }
}