using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.SQLite;
using PhoneStore.View;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModel
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public SQLiteConnection conn;
        public FirebaseHelper firebase;
        public HomeViewModel()
        {
            firebase = new FirebaseHelper();
            var cart = Task.Run(async () => await App.SQLiteDb.GetItemsAsync()).Result;
            if (cart != null)
                ItemCount = cart.Count;
            RotatorModels = Task.Run(async () => await getRotatorsAsync().ConfigureAwait(true)).Result;
            ItemModels = Task.Run(async () => await getAllItemAsync().ConfigureAwait(true)).Result;
            this.cmdPhone = new Command(GoToPhone);
            this.cmdLoadItem = new Command<object>(GotoItemDetail);
            this.CartTapped = new Command(GotoCart);
            this.SearchTapped = new Command(GotoSearch);
        }

        private void GotoSearch(object obj)
        {
            Application.Current.MainPage.Navigation.PushAsync(new SearchPage());
        }

        private void GotoCart(object obj)
        {
            Application.Current.MainPage.Navigation.PushAsync(new YourCartPage());
        }

        private async Task<List<ItemModel>> getAllItemAsync()
        {
            var itemFirebases = await firebase.GetAllItem(FirebaseHelper.userToken).ConfigureAwait(true);
            List<ItemModel> Items = new List<ItemModel>();
            Items = itemFirebases;
            return Items;
        }

        private async Task<List<RotatorModel>> getRotatorsAsync()
        {
            var rotators = await firebase.GetRotators(FirebaseHelper.userToken).ConfigureAwait(true);
            List<RotatorModel> rotatorModels = new List<RotatorModel>();
            rotatorModels = rotators;
            return rotatorModels;
        }

        private void GotoItemDetail(object obj)
        {
            var detail = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as ItemModel;
            Application.Current.MainPage.Navigation.PushAsync(new ItemDetailPage(detail));
        }

        private void GoToPhone(object obj)
        {
            Application.Current.MainPage.Navigation.PushAsync(new PhonePage());
        }

        private List<RotatorModel> _rotatorModels;
        public List<RotatorModel> RotatorModels
        {
            get { return _rotatorModels; }
            set { _rotatorModels = value; }
        }

        private decimal _itemCount;
        public decimal ItemCount
        {
            get { return _itemCount; }
            set
            {
                _itemCount = value;
                OnPropertyChanged(nameof(ItemCount));
            }
        }
        
        public List<ItemModel> ItemModels { get; set; }
        public CartModel Cart { get; set; }

        public Command cmdPhone { get; }
        public Command CartTapped { get; }
        public Command SearchTapped { get; }
        public Command<object> cmdLoadItem { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
