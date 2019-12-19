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

            this.cmdLoadItem = new Command(LoadItem);
        }

        #region Logic
        public List<ItemModel> GetUserFavoriteItems()
        {
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            var allItems = Task.Run(async () => await firebase.GetAllFavoriteItems()).Result;
            var userItems = allItems.Where(it => it.UserEmail == user.Email).ToList();
            return userItems;
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
        #endregion

        #region Command
        public Command cmdLoadItem { get; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
