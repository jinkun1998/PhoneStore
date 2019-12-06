using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public CartViewModel()
        {
            firebase = new FirebaseHelper();
            Items = Task.Run(async () => await firebase.GetUserCartItems("tiensieqquocthao@gmail.com")).Result;
            foreach (var item in Items)
            {
                TotalPrice += item.Price * item.Quantity;
            }

            this.AddQuantityTapped = new Command(AddQuantityChanged);
            this.RemoveQuantityTapped = new Command(RemoveQuantityChanged);
            this.TakeOrderTapped = new Command(TakeOrder);
            this.DeleteItemTapped = new Command(DeleteItem);
        }

        private void DeleteItem(object obj)
        {
            var item = obj as CartModel;
            Task.Run(async () => await firebase.DeleteUserCartInOrder(item).ConfigureAwait(true));
        }

        private void TakeOrder(object obj)
        {
            if (Items.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("Thông báo", "Giỏ hàng đang rỗng!", "Đã hiểu");
            }
            else
            {
                Application.Current.MainPage.Navigation.PushAsync(new ShipmentPage(Items));
            }

        }

        private void RemoveQuantityChanged(object obj)
        {
            CartModel item = obj as CartModel;
            if (item.Quantity > 0)
            {
                item.Quantity--;
                if (item.Quantity == 0)
                {
                    Task.Run(async () => await firebase.DeleteUserCartInOrder(item).ConfigureAwait(true));
                }
                else
                {
                    TotalPrice -= item.Price;
                }
            }

            Task.Run(async () => await firebase.UpdateUserCartItem(item).ConfigureAwait(true));
        }

        private void AddQuantityChanged(object obj)
        {
            CartModel item = obj as CartModel;
            item.Quantity++;
            TotalPrice += item.Price;
            Task.Run(async () => await firebase.UpdateUserCartItem(item).ConfigureAwait(true));
        }

        public List<CartModel> Items { get; set; }
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

        public event PropertyChangedEventHandler PropertyChanged;


        public Command AddQuantityTapped { get; }
        public Command RemoveQuantityTapped { get; }
        public Command TakeOrderTapped { get; }
        public Command DeleteItemTapped { get; }


        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
