using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.SQLite;
using PhoneStore.View;
using PhoneStore.View.ListViews;
using PhoneStore.View.MainViews.User.EditUser;
using PhoneStore.View.MainViews.User.FavoriteViews;
using PhoneStore.View.MainViews.User.MyOrderViews;
using Plugin.FirebaseAuth;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

            using (UserDialogs.Instance.Loading("Vui lòng chờ...", null, null, true, MaskType.Gradient))
            {
                firebase = new FirebaseHelper();
                var cart = Task.Run(async () => await App.SQLiteDb.GetItemsAsync()).Result;
                if (cart != null)
                    ItemCount = cart.Count;
                RotatorModels = Task.Run(async () => await getRotatorsAsync().ConfigureAwait(true)).Result;
                ItemModels = Task.Run(async () => await getAllItemAsync().ConfigureAwait(true)).Result;
                NewItems = ItemModels.OrderBy(it => it.CreatedDate).ToList();
                HotItems = ItemModels.Where(it => it.Rate >= 3.5).ToList();
                DiscountItems = ItemModels.OrderBy(it => it.Price).ToList();
                var currentUser = CrossFirebaseAuth.Current.Instance.CurrentUser;
                User = Task.Run(async () => await App.SQLiteDb.GetUserAsync(currentUser.Email)).Result;
                if (User != null)
                {
                    Name = User.FullName;
                    Image = User.AvatarLink;
                }

                this.cmdPhone = new Command(GoToPhone);
                this.cmdTablet = new Command(GoToTablet);
                this.cmdWatch = new Command(GoToWatch);
                this.cmdLoadItem = new Command<object>(GotoItemDetail);
                this.CartTapped = new Command(GotoCart);
                this.SearchTapped = new Command(GotoSearch);
                this.MyOrderTapped = new Command(GotoMyOrder);
                this.SignOutTapped = new Command(SignOutCmd);
                this.MyFavoriteTapped = new Command(FavoriteCmd);
                this.EditProfile = new Command(EditUser);
                this.AppInfo = new Command(ShowInfo);
                this.QRCodeTapped = new Command(GotoQRCodeAsync);
            }
        }


        #region Logic
        private async void GotoQRCodeAsync(object obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new QRPage());
        }
        private void ShowInfo(object obj)
        {
            Application.Current.MainPage.DisplayAlert("Thông tin ứng dụng", "Copyright © PhoneStore 2019\n" +
                "Chủ sở hữu: Đoàn Lê Quốc Thảo\n" +
                "Lớp: 10DHPM01\n" +
                "Trường: Đại học Gia Định\n" +
                "Khóa: 10\n" +
                "Email:quocthao23061998.tg@gmail.com", "Đã hiểu");
        }

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
                using (UserDialogs.Instance.Loading("Vui lòng chờ..."))
                {
                    CrossFirebaseAuth.Current.Instance.SignOut();
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
                }
                await Application.Current.MainPage.Navigation.PushAsync(new FirstPage());
                Application.Current.MainPage = new NavigationPage(new FirstPage());
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
            var itemFirebases = await firebase.GetAllItems().ConfigureAwait(true);
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
        private void GoToTablet(object obj)
        {
            Application.Current.MainPage.Navigation.PushAsync(new TabletPage());
        }
        private void GoToWatch(object obj)
        {
            Application.Current.MainPage.Navigation.PushAsync(new WatchPage());
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
        public List<ItemModel> NewItems { get; set; }
        public List<ItemModel> HotItems { get; set; }
        public List<ItemModel> DiscountItems { get; set; }
        public CartModel Cart { get; set; }
        public UserModel User { get; set; }
        #endregion

        #region Command
        public Command cmdPhone { get; }
        public Command cmdTablet { get; }
        public Command cmdWatch { get; }
        public Command CartTapped { get; }
        public Command SearchTapped { get; }
        public Command<object> cmdLoadItem { get; }
        public Command MyOrderTapped { get; }
        public Command MyFavoriteTapped { get; }
        public Command SignOutTapped { get; }
        public Command QRCodeTapped { get; }
        public Command EditProfile { get; }
        public Command AppInfo { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
        #endregion
    }
}
