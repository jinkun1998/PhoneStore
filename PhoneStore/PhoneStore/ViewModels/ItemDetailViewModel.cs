using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.SQLite;
using PhoneStore.View;
using PhoneStore.View.DetailViews;
using SQLite;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class ItemDetailViewModel : INotifyPropertyChanged
    {
        public FirebaseHelper firebase;
        public ItemDetailViewModel(ItemModel item)
        {
            firebase = new FirebaseHelper();
            Item = item;
            this.DescriptionTapped = new Command(GotoDescription);
            this.CartTapped = new Command(GotoCart);
            this.ShareAction = new Command(ShareUri);
            this.AddCarttapped = new Command(AddCart);
        }

        private void SelectedColorCmd(object obj)
        {
            //SelectedColor = (obj as ColorModel).Value.ToString();
        }

        private void GotoCart(object obj)
        {
            if (SelectedColor == null)
            {
                Application.Current.MainPage.DisplayAlert("Thông báo", "Chưa chọn màu sắc sản phẩm!", "Đã hiểu");
            }
            else
            {
                var exitsCart = App.SQLiteDb.GetItemAsync(Item.Code).Result;
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
                    cart.Color = SelectedColor.Value;
                    cart.InOrder = false;
                    Task.Run(async () => await App.SQLiteDb.SaveItemAsync(cart));
                }
                else
                {
                    exitsCart.Quantity++;
                    App.SQLiteDb.SaveItemAsync(exitsCart);
                }
                Thread.Sleep(500);
                Application.Current.MainPage.Navigation.PushAsync(new YourCartPage());
            }
            
        }

        private void AddCart(object obj)
        {
            if (SelectedColor == null)
            {
                Application.Current.MainPage.DisplayAlert("Thông báo", "Chưa chọn màu sắc sản phẩm!", "Đã hiểu");
            }
            else
            {
                var exitsCart = App.SQLiteDb.GetItemAsync(Item.Code).Result;
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
                    cart.Color = SelectedColor.Value;
                    cart.InOrder = false;
                    Task.Run(async () => await App.SQLiteDb.SaveItemAsync(cart));
                }
                else
                {
                    exitsCart.Quantity++;
                    App.SQLiteDb.SaveItemAsync(exitsCart);
                }
                Application.Current.MainPage.DisplayAlert("Thông báo", "Đã thêm vào giỏ hàng thành công!", "Đã hiểu");
            }
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

        private ColorModel _selectedcolor;
        public ColorModel SelectedColor 
        { 
            get { return _selectedcolor; } 
            set { _selectedcolor = value; OnPropertyChanged(nameof(SelectedColor)); }
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
