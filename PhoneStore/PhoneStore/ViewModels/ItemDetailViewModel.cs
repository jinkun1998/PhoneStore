using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using PhoneStore.View.DetailViews;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
            //this.ColorTapped = new Command(SelectedColorCmd);
        }

        private void SelectedColorCmd(object obj)
        {
            //SelectedColor = (obj as ColorModel).Value.ToString();
        }

        private void GotoCart(object obj)
        {
            List<CartModel> allCarts = Task.Run(async () => await firebase.GetUserCartItems(FirebaseHelper.userEmail, FirebaseHelper.userToken)).Result;
            var exitsCart = allCarts.Where(it => it.Code == Item.Code).FirstOrDefault();
            if (exitsCart == null)
            {
                CartModel cart = new CartModel();
                Random rd = new Random();
                var tempCode = rd.Next(0, 999999);
                cart.CartCode = tempCode.ToString("000000");
                cart.Code = Item.Code;
                cart.Image = Item.Image;
                cart.Name = Item.Name;
                cart.Price = Item.Price;
                cart.Shortdescription = Item.Shortdescription;
                cart.UserEmail = FirebaseHelper.userEmail;
                cart.Description = Item.Description;
                cart.DescriptionLink = Item.DescriptionLink;
                cart.Quantity = 1;
                //cart.Color = SelectedColor.ToString();
                cart.InOrder = false;
                Task.Run(async () => await firebase.AddUserCart(cart, FirebaseHelper.userToken).ConfigureAwait(true));
            }
            else
            {
                exitsCart.Quantity++;
                Task.Run(async () => await firebase.UpdateUserCartItem(exitsCart, FirebaseHelper.userToken).ConfigureAwait(true));
            }
            //Task.Run(async () => await App.Database.SaveCartItemAsync(cart).ConfigureAwait(true));
            Application.Current.MainPage.Navigation.PushAsync(new YourCartPage());
        }

        private void AddCart(object obj)
        {
            List<CartModel> allCarts = Task.Run(async () => await firebase.GetUserCartItems("tiensieqquocthao@gmail.com", FirebaseHelper.userToken)).Result;
            var exitsCart = allCarts.Where(it => it.Code == Item.Code).FirstOrDefault();
            if (exitsCart == null)
            {
                CartModel cart = new CartModel();
                Random rd = new Random();
                var tempCode = rd.Next(0, 999999);
                cart.CartCode = tempCode.ToString("000000");
                cart.Code = Item.Code;
                cart.Image = Item.Image;
                cart.Name = Item.Name;
                cart.Price = Item.Price;
                cart.Shortdescription = Item.Shortdescription;
                cart.UserEmail = "tiensieqquocthao@gmail.com";
                cart.Description = Item.Description;
                cart.DescriptionLink = Item.DescriptionLink;
                cart.Quantity = 1;
                //cart.Color = SelectedColor.ToString();
                cart.InOrder = false;
                Task.Run(async () => await firebase.AddUserCart(cart, FirebaseHelper.userToken).ConfigureAwait(true));
            }
            else
            {
                exitsCart.Quantity++;
                Task.Run(async () => await firebase.UpdateUserCartItem(exitsCart, FirebaseHelper.userToken).ConfigureAwait(true));
            }
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


        #region Properties
        private ItemModel _item;
        public ItemModel Item
        {
            get { return _item; }
            set
            {
                _item = value;

            }
        }

        private SfChip _selectedcolor;
        public SfChip SelectedColor 
        { 
            get { return _selectedcolor; } 
            set { _selectedcolor = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Command
        public Command DescriptionTapped { get; }
        public Command ShareAction { get; }
        public Command CartTapped { get; }
        public Command AddCarttapped { get; }
        public Command ColorTapped { get; }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
