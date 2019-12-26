using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
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
    public class ChoosePromoViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public ChoosePromoViewModel(OrderModel order)
        {
            firebase = new FirebaseHelper();
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            this.Order = order;
            Promos = Task.Run(async () => await firebase.GetAllUserPromo(user.Email)).Result;
            if (Promos.Count == 0)
                IsVisible = true;

            this.ItemTapped = new Command(LoadItem);
            this.BackButton = new Command(Back);
        }
        private void LoadItem(object obj)
        {
            decimal total = 0;
            var promo = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as QRPromoModel;
            foreach (var cart in Order.Carts)
            {
                total += cart.Price;
            }

            Order.Promo = promo;
            Application.Current.MainPage.Navigation.PushAsync(new ShipmentPage(Order));
            Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
            Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
        }
        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        public OrderModel Order { get; set; }

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

        public Command BackButton { get; }
        public Command ItemTapped { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
