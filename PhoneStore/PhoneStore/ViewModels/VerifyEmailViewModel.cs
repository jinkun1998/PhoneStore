﻿using Acr.UserDialogs;
using PhoneStore.View;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class VerifyEmailViewModel
    {
        public VerifyEmailViewModel()
        {
            this.CheckTapped = new Command(CheckVerifyEmail);
            UserDialogs.Instance.HideLoading();
        }

        private async void CheckVerifyEmail(object obj)
        {
            using (UserDialogs.Instance.Loading("Đang kiểm tra...", null, null, true, MaskType.Gradient))
            {
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                if (user.IsEmailVerified)
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                    Application.Current.MainPage = new NavigationPage(new HomePage());
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Thông báo", "Hãy kích hoạt tài khoản của bạn!", "Đã hiểu");
                }
            }
        }

        public Command CheckTapped { get; }
    }
}
