using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class PhoneViewModel
    {
        FirebaseHelper firebase;
        public PhoneViewModel()
        {
            firebase = new FirebaseHelper();
            ItemCollection = Task.Run(async () => await getAllItemAsync()).Result;
            this.ItemTapped = new Command(GotoDetail);
            this.BackButton = new Command(Back);
        }

        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        private async Task<List<ItemModel>> getAllItemAsync()
        {
            var itemFirebases = await firebase.GetAllItem();
            List<ItemModel> Items = new List<ItemModel>();
            foreach (var itemFirebase in itemFirebases)
            {
                ItemModel item = new ItemModel();
                item.Name = itemFirebase.Name;
                item.Image = itemFirebase.Image;
                item.Price = itemFirebase.Price;
                item.Shortdescription = itemFirebase.Shortdescription;
                item.Description = itemFirebase.Description;
                item.DescriptionLink = itemFirebase.DescriptionLink;
                item.Colors = itemFirebase.Colors;
                item.RotatorImages = itemFirebase.RotatorImages;
                Items.Add(item);
            }
            return Items;
        }

        private void GotoDetail(object obj)
        {
            var detail = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as ItemModel;
            Application.Current.MainPage.Navigation.PushAsync(new ItemDetailPage(detail));
        }

        private List<ItemModel> _itemCollection;
        public List<ItemModel> ItemCollection
        {
            get { return _itemCollection; }
            set { _itemCollection = value; }
        }

        private ObservableCollection<RotatorModel> _rotatorModels;
        public ObservableCollection<RotatorModel> RotatorModels
        {
            get { return _rotatorModels; }
            set { _rotatorModels = value; }
        }

        public Command ItemTapped { get; }
        public Command BackButton { get; }
    }
}
