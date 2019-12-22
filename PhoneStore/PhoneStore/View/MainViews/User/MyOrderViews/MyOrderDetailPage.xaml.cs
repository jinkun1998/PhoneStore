using PhoneStore.Models;
using PhoneStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhoneStore.View.MainViews.User.MyOrderViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyOrderDetailPage : ContentPage
    {
        public MyOrderDetailPage()
        {
            InitializeComponent();
            MyOrderDetailViewModel vm = new MyOrderDetailViewModel();
            this.BindingContext = vm;
        }
        public MyOrderDetailPage(OrderModel order)
        {
            InitializeComponent();
            MyOrderDetailViewModel vm = new MyOrderDetailViewModel(order);
            this.BindingContext = vm;
            if (vm.Order.Status != OrderModel.OrderStatus.Ordered)
                btnCancel.IsEnabled = false;
        }
    }
}