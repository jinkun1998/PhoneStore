using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using PhoneStore.View.DetailViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class ItemDetailViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public ItemDetailViewModel(ItemModel item)
        {
            firebase = new FirebaseHelper();
            Item = item;
            this.DescriptionTapped = new Command(GotoDescription);
            this.CartTapped = new Command(GotoCart);
            this.ShareAction = new Command(ShareUri);
            this.AddCarttapped = new Command(AddCart);
        }

        private void GotoCart(object obj)
        {
            CartModel cart = new CartModel();
            cart.Code = Item.Code;
            cart.Image = Item.Image;
            cart.Name = Item.Name;
            cart.Price = Item.Price;
            cart.Shortdescription = Item.Shortdescription;
            cart.UserEmail = "tiensieqquocthao@gmail.com";
            cart.Description = Item.Description;
            cart.DescriptionLink = Item.DescriptionLink;
            cart.Quantity = 1;
            cart.InOrder = false;
            Task.Run(async () => await firebase.AddUserCart(cart).ConfigureAwait(true));
            //Task.Run(async () => await App.Database.SaveCartItemAsync(cart).ConfigureAwait(true));
            Application.Current.MainPage.Navigation.PushAsync(new YourCartPage());
        }

        private void AddCart(object obj)
        {
            CartModel cart = new CartModel();
            cart.Code = Item.Code;
            cart.Image = Item.Image;
            cart.Name = Item.Name;
            cart.Price = Item.Price;
            cart.Shortdescription = Item.Shortdescription;
            cart.UserEmail = "tiensieqquocthao@gmail.com";
            cart.Description = Item.Description;
            cart.DescriptionLink = Item.DescriptionLink;
            cart.Quantity = 1;
            cart.InOrder = false;
            Task.Run(async () => await firebase.AddUserCart(cart).ConfigureAwait(true));
            Application.Current.MainPage.DisplayAlert("Thông báo", "Đã thêm vào giỏ hàng thành công!", "Đã hiểu");
        }

        public void ShareUri(object ojb)
        {
            Share.RequestAsync(new ShareTextRequest
            {
                Uri = Item.DescriptionLink,
                Title = "Chia sẻ"
            });
        }

        private void GotoDescription(object obj)
        {
            Application.Current.MainPage.Navigation.PushAsync(new DescriptionPage(Item));
        }

        private ItemModel _item;

        public event PropertyChangedEventHandler PropertyChanged;

        public ItemModel Item
        {
            get { return _item; }
            set
            {
                _item = value;

            }
        }

        public Command DescriptionTapped { get; }
        public Command ShareAction { get; }
        public Command CartTapped { get; }
        public Command AddCarttapped { get; }
    }
}
