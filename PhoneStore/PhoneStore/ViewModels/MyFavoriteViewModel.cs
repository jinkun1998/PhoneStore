using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class MyFavoriteViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public MyFavoriteViewModel()
        {
            firebase = new FirebaseHelper();
            Items = GetUserFavoriteItems();
            if (Items.Count == 0)
                IsVisible = true;
            this.cmdLoadItem = new Command<object>(LoadItem);
            this.BackButton = new Command(Back);
        }

        #region Logic
        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        public List<ItemModel> GetUserFavoriteItems()
        {
            using (UserDialogs.Instance.Loading("Đang tải..."))
            {
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                var allItems = Task.Run(async () => await firebase.GetAllFavoriteItems()).Result;
                var userItems = allItems.Where(it => it.UserEmail == user.Email).ToList();
                return userItems;
            }
            
        }

        private void LoadItem(object obj)
        {
            var detail = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as ItemModel;
            Application.Current.MainPage.Navigation.PushAsync(new ItemDetailPage(detail));
        }
        #endregion

        #region Popertiies 
        private List<ItemModel> _items;
        public List<ItemModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }
        private bool _isvisible;
        public bool IsVisible
        {
            get { return _isvisible; }
            set
            {
                _isvisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
        #endregion

        #region Command
        public Command<object> cmdLoadItem { get; }
        public Command BackButton { get; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
