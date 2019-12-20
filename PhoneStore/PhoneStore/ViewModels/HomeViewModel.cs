using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.SQLite;
using PhoneStore.View;
using PhoneStore.View.MainViews.User.EditUser;
using PhoneStore.View.MainViews.User.FavoriteViews;
using PhoneStore.View.MainViews.User.MyOrderViews;
using Plugin.FirebaseAuth;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PhoneStore.ViewModel
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public FirebaseHelper firebase;
        public HomeViewModel()
        {
            firebase = new FirebaseHelper();
            var cart = Task.Run(async () => await App.SQLiteDb.GetItemsAsync()).Result;
            if (cart != null)
                ItemCount = cart.Count;
            RotatorModels = Task.Run(async () => await getRotatorsAsync().ConfigureAwait(true)).Result;
            ItemModels = Task.Run(async () => await getAllItemAsync().ConfigureAwait(true)).Result;
            var currentUser = CrossFirebaseAuth.Current.Instance.CurrentUser;
            User = Task.Run(async () => await App.SQLiteDb.GetUserAsync(currentUser.Email)).Result;
            if (User != null)
            {
                Name = User.FullName;
                Image = User.AvatarLink;
            }
            if (RotatorModels.Count > 0 && ItemModels.Count > 0)
                isVisible = false;

            UserDialogs.Instance.ShowLoading("Vui lòng chờ...");
            this.cmdPhone = new Command(GoToPhone);
            this.cmdLoadItem = new Command<object>(GotoItemDetail);
            this.CartTapped = new Command(GotoCart);
            this.SearchTapped = new Command(GotoSearch);
            this.MyOrderTapped = new Command(GotoMyOrder);
            this.SignOutTapped = new Command(SignOutCmd);
            this.MyFavoriteTapped = new Command(FavoriteCmd);
            this.EditProfile = new Command(EditUser);
            UserDialogs.Instance.HideLoading();

        }

        #region Logic

        private async void EditUser(object obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new InfomationPage());
        }

        private async void FavoriteCmd(object obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new FavoritePage());
        }
        private async void SignOutCmd(object obj)
        {
            var result = await Application.Current.MainPage.DisplayAlert("Cảnh báo", "Bạn có chắc chắn muốn đăng xuất?", "Chắc chắn", "Hủy");
            if (result)
            {
                UserDialogs.Instance.ShowLoading("Vui lòng chờ...");
                CrossFirebaseAuth.Current.Instance.SignOut();
                await Application.Current.MainPage.Navigation.PushAsync(new FirstPage());
                Application.Current.MainPage = new NavigationPage(new FirstPage());
                var items = Task.Run(async () => await App.SQLiteDb.GetItemsAsync()).Result;
                foreach (var item in items)
                {
                    await App.SQLiteDb.DeleteItemAsync(item);
                }
                var users = Task.Run(async () => await App.SQLiteDb.GetUsersAsync()).Result;
                foreach (var item in users)
                {
                    await App.SQLiteDb.DeleteUserAsync(item);
                }
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void GotoMyOrder(object obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new MyOrderPage());
        }

        private async void GotoSearch(object obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SearchPage());
        }

        private async void GotoCart(object obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new YourCartPage());
        }

        private async Task<List<ItemModel>> getAllItemAsync()
        {
            var itemFirebases = await firebase.GetAllItem().ConfigureAwait(true);
            List<ItemModel> Items = new List<ItemModel>();
            Items = itemFirebases;
            return Items;
        }

        private async Task<List<RotatorModel>> getRotatorsAsync()
        {
            var rotators = await firebase.GetRotators().ConfigureAwait(true);
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
        #endregion

        #region Properties
        private List<RotatorModel> _rotatorModels;
        public List<RotatorModel> RotatorModels
        {
            get { return _rotatorModels; }
            set { _rotatorModels = value; }
        }

        private int _itemCount;
        public int ItemCount
        {
            get { return _itemCount; }
            set
            {
                _itemCount = value;
                OnPropertyChanged(nameof(ItemCount));
            }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _image;
        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        public bool isVisible { get; set; }
        public List<ItemModel> ItemModels { get; set; }
        public CartModel Cart { get; set; }
        public UserModel User { get; set; }
        #endregion

        #region Command
        public Command cmdPhone { get; }
        public Command CartTapped { get; }
        public Command SearchTapped { get; }
        public Command<object> cmdLoadItem { get; }
        public Command MyOrderTapped { get; }
        public Command MyFavoriteTapped { get; }
        public Command SignOutTapped { get; }
        public Command EditProfile { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
        #endregion
    }
}
