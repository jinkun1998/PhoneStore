using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
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
        public FirebaseHelper firebase;
        public HomeViewModel()
        {
            firebase = new FirebaseHelper();
            //getAllItemAsync();
            var cart = Task.Run(async () => await firebase.GetUserCartItems("tiensieqquocthao@gmail.com")).Result;
            ItemCount = cart.Count;
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
            var itemFirebases = await firebase.GetAllItem().ConfigureAwait(true);
            List<ItemModel> Items = new List<ItemModel>();
            Items = itemFirebases;
            return Items;
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

        private ObservableCollection<RotatorModel> _rotatorModels;
        public ObservableCollection<RotatorModel> RotatorModels
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
