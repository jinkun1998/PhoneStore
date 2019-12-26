using PhoneStore.Firebase;
using PhoneStore.Models;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class PromoViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public PromoViewModel()
        {
            firebase = new FirebaseHelper();
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            Promos = Task.Run(async () => await firebase.GetAllUserPromo(user.Email)).Result;
            if (Promos.Count == 0)
            {
                IsVisible = true;
            }

            this.BackButton = new Command(Back);
        }

        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private List<QRPromoModel> _promos;
        public List<QRPromoModel> Promos
        {
            get { return _promos; }
            set
            {
                _promos = value;
                OnPropertyChanged(nameof(Promos));
            }
        }

        private bool _isvisible;
        public bool IsVisible
        {
            get { return _isvisible; }
            set
            {
                _isvisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public Command BackButton { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
