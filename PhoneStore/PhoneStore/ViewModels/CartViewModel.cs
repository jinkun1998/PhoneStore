using Android.Content.Res;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.SQLite;
using PhoneStore.View;
using PhoneStore.ViewModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        public FirebaseHelper firebase;
        public CartViewModel()
        {
            firebase = new FirebaseHelper();
            Items = Task.Run(async () => await App.SQLiteDb.GetItemsAsync()).Result;
            //Items = Task.Run(async () => await firebase.GetUserCartItems(FirebaseHelper.userEmail, FirebaseHelper.userToken)).Result;
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
            Task.Run(async () => await App.SQLiteDb.DeleteItemAsync(item));
            //Task.Run(async () => await firebase.DeleteUserCartInOrder(item, FirebaseHelper.userToken).ConfigureAwait(true));
            //Application.Current.MainPage.Navigation.PopAsync(false);
            Thread.Sleep(500);
            Application.Current.MainPage.Navigation.PushAsync(new YourCartPage(), false);
            Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
        }

        private void TakeOrder(object obj)
        {
            if (Items.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("Thông báo", "Giỏ hàng đang rỗng!", "Đã hiểu");
            }
            else
            {
                Application.Current.MainPage.Navigation.PushAsync(new ShipmentPage());
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
                    Items.Remove(item);
                    App.SQLiteDb.DeleteItemAsync(item);
                    Application.Current.MainPage.Navigation.PushAsync(new YourCartPage(), false);
                    Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                }
                else
                {
                    TotalPrice -= item.Price;
                }
            }

            App.SQLiteDb.SaveItemAsync(item);
        }

        private void AddQuantityChanged(object obj)
        {
            CartModel item = obj as CartModel;
            item.Quantity++;
            TotalPrice += item.Price;
            App.SQLiteDb.SaveItemAsync(item);
        }

        private List<CartModel> _items;
        public List<CartModel> Items 
        { 
            get { return _items; }
            set { _items = value; OnPropertyChanged(); }
        }


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
