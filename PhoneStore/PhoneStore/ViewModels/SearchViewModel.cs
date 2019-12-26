using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using Syncfusion.ListView.XForms;
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
    public class SearchViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public SearchViewModel()
        {
            firebase = new FirebaseHelper();

            this.SearchCmd = new Command(Search);
            this.ItemTapped = new Command(LoadItem);
        }

        private void LoadItem(object obj)
        {
            var detail = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as ItemModel;
            Application.Current.MainPage.Navigation.PushAsync(new ItemDetailPage(detail));
        }

        private void Search(object obj)
        {
            SfListView lv = obj as SfListView;
            UserDialogs.Instance.ShowLoading("Đang tìm kiếm...");
            try
            {
                var allItems = Task.Run(async () => await firebase.GetAllItems()).Result;
                var foundItem3 = allItems.Where(it => it.Name.Contains(Text)).ToList();
                var foundItem1 = allItems.Where(it => it.Name.Contains(Text.ToLower())).ToList();
                var foundItem2 = allItems.Where(it => it.Name.Contains(Text.ToUpper())).ToList();
                List<ItemModel> items = new List<ItemModel>();
                foreach (var item in foundItem3)
                {
                    items.Add(item);
                }
                foreach (var item in foundItem1)
                {
                    items.Add(item);
                }
                foreach (var item in foundItem2)
                {
                    items.Add(item);
                }
                if (items.Count > 0)
                {
                    lv.IsVisible = true;
                    ItemCollection = items;
                    UserDialogs.Instance.HideLoading();
                    IsVisible = false;
                }
                else
                {
                    lv.IsVisible = false;
                    ItemCollection.Clear();
                    UserDialogs.Instance.HideLoading();
                    IsVisible = true;
                }
            }
            catch (Exception)
            {
                IsVisible = true;
            }
        }

        public bool _isvisible;
        public bool IsVisible
        {
            get { return _isvisible; }
            set
            {
                _isvisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
        public string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        public List<ItemModel> _items;
        public List<ItemModel> ItemCollection
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(ItemCollection));
            }
        }
        public Command SearchCmd { get; }
        public Command ItemTapped { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}

