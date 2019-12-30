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
            try
            {
                firebase = new FirebaseHelper();

                this.SearchCmd = new Command(Search);
                this.ItemTapped = new Command(LoadItem);
            }
            catch (Exception)
            {
                
            }
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
                List<ItemModel> lowerItems = new List<ItemModel>();
                foreach (var item in allItems)
                {
                    ItemModel newItem = new ItemModel();
                    newItem.Code = item.Code;
                    newItem.Colors = item.Colors;
                    newItem.CreatedDate = item.CreatedDate;
                    newItem.Description = item.Description;
                    newItem.DescriptionLink = item.DescriptionLink;
                    newItem.Image = item.Image;
                    newItem.Name = item.Name.ToLower();
                    newItem.Price = item.Price;
                    newItem.Rate = item.Rate;
                    newItem.RotatorImages = item.RotatorImages;
                    newItem.Shortdescription = item.Shortdescription;
                    newItem.Type = item.Type;
                    lowerItems.Add(newItem);
                }
                var foundItems = lowerItems.Where(it => it.Name.Contains(Text.ToLower())).ToList();
                var normalItems = new List<ItemModel>();
                foreach (var item in foundItems)
                {
                    normalItems.Add(allItems.Where(it => it.Code == item.Code).FirstOrDefault());
                }
                if (normalItems.Count > 0)
                {
                    lv.IsVisible = true;
                    ItemCollection = normalItems;
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
                UserDialogs.Instance.HideLoading();
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

