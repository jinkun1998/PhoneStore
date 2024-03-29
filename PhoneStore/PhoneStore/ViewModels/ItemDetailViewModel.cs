﻿using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.SQLite;
using PhoneStore.View;
using PhoneStore.View.DetailViews;
using PhoneStore.ViewModel;
using Plugin.FirebaseAuth;
using Plugin.Toast;
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
            try
            {
                using (UserDialogs.Instance.Progress("Đang tải...", null, null, true, MaskType.Gradient))
                {
                    firebase = new FirebaseHelper();
                    Item = item;
                    CheckFavorite();
                    this.DescriptionTapped = new Command(GotoDescription);
                    this.CartTapped = new Command(GotoCart);
                    this.ShareAction = new Command(ShareUri);
                    this.AddCarttapped = new Command(AddCart);
                    this.FavoriteTapped = new Command(FavoriteCmd);
                    this.BackButton = new Command(Back);
                }
            }
            catch (Exception)
            {

            }
        }

        #region Logic
        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        public void CheckFavorite()
        {
            using (UserDialogs.Instance.Progress("Vui lòng chờ...", null, null, true, MaskType.Gradient))
            {
                var allItems = Task.Run(async () => await firebase.GetAllFavoriteItems()).Result;
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                var exitsFavorite = allItems.Where(it => it.Code == Item.Code && it.UserEmail == user.Email).FirstOrDefault();
                if (exitsFavorite == null)
                {
                    IsFavorite = false;
                }
                else
                {
                    IsFavorite = true;
                }
            }  
        }

        private void FavoriteCmd(object obj)
        {
            //SfButton btn = obj as SfButton;
            var allItems = Task.Run(async () => await firebase.GetAllFavoriteItems()).Result;
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            var exitsFavorite = allItems.Where(it => it.Code == Item.Code && it.UserEmail == user.Email).FirstOrDefault();
            if (exitsFavorite == null)
            {
                IsFavorite = true;
                Item.UserEmail = user.Email;
                CrossToastPopUp.Current.ShowToastMessage("Đã thích");
                Task.Run(async () => await firebase.AddUserFavoriteItem(Item));
            }
            else
            {
                IsFavorite = false;
                CrossToastPopUp.Current.ShowToastMessage("Đã hủy thích");
                Task.Run(async () => await firebase.DeleteUserFavoriteItem(exitsFavorite));
            }
        }

        private void GotoCart(object obj)
        {
            if (SelectedColor == null)
            {
                Application.Current.MainPage.DisplayAlert("Thông báo", "Chưa chọn màu sắc sản phẩm!", "Đã hiểu");
            }
            else
            {
                using (UserDialogs.Instance.Progress("Đang xử lý...", null, null, true, MaskType.Gradient))
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
            using (UserDialogs.Instance.Progress("Đang xử lý...", null, null, true, MaskType.Gradient))
            {
                Share.RequestAsync(new ShareTextRequest
                {
                    Uri = Item.DescriptionLink,
                    Title = "Chia sẻ"
                });
            }
        }

        private void GotoDescription(object obj)
        {
            using (UserDialogs.Instance.Progress("Đang xử lý...", null, null, true, MaskType.Gradient))
            {
                Application.Current.MainPage.Navigation.PushAsync(new DescriptionPage(Item));
            }
        }
        #endregion

        #region Properties
        private ItemModel _item;
        public ItemModel Item
        {
            get { return _item; }
            set
            {
                _item = value;
                OnPropertyChanged(nameof(Item));
            }
        }

        private bool _isfavorite;
        public bool IsFavorite
        {
            get { return _isfavorite; }
            set
            {
                _isfavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
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
        public Command FavoriteTapped { get; }
        public Command BackButton { get; set; }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
