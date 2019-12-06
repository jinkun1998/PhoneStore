using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using PhoneStore.View.MainViews;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class ShipmentViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public ShipmentViewModel(List<CartModel> carts)
        {
            firebase = new FirebaseHelper();
            Order = new OrderModel();
            Order.Carts = carts;
            foreach (var cart in Order.Carts)
            {
                TotalPrice += cart.Quantity * cart.Price;
            }

            this.CreateOrder = new Command(CreateNewOrder);
            this.ChooseDelivery = new Command(Delivery);
        }

        private void Delivery(object obj)
        {
            var radioButoon = obj as SfButton;
        }

        private void CreateNewOrder(object obj)
        {
            Order.Code = "123456";
            Order.UserEmail = "tiensieqquocthao@gmail.com";
            Order.CreatedOn = DateTime.Now;
            Order.Note = "Test Order";
            Order.Address = "800 Nguyễn Văn Linh";
            Task.Run(async () => await firebase.AddUserOrder(Order).ConfigureAwait(true));
            Application.Current.MainPage.DisplayAlert("Thông báo", "Đã đặt hàng thành công!", "OK");
            foreach (var item in Order.Carts)
            {
                Task.Run(async () => await firebase.DeleteUserCartInOrder(item).ConfigureAwait(true));
                Task.Delay(500);
            }
            Application.Current.MainPage.Navigation.PushAsync(new NavigationPage(new HomePage()));
            Application.Current.MainPage = new HomePage();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
        public OrderModel Order { get; set; }
        public bool isChecked { get; set; }
        public Command CreateOrder { get; }
        public Command ChooseDelivery { get; }
        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }
}
