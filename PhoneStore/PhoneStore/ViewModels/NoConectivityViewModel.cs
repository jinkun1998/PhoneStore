using PhoneStore.View;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class NoConectivityViewModel
    {
        public NoConectivityViewModel()
        {
            this.TryAgainTapped = new Command(TryAgainCmd);
        }

        private void TryAgainCmd(object obj)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                if (user == null)
                {
                    Application.Current.MainPage.Navigation.PushAsync(new FirstPage(), false);
                    Application.Current.MainPage = new NavigationPage(new FirstPage());
                }
                else
                {
                    Application.Current.MainPage.Navigation.PushAsync(new HomePage(), false);
                    Application.Current.MainPage = new NavigationPage(new HomePage());
                }
            }
        }

        public Command TryAgainTapped { get; }
    }
}
