using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View.MainViews.User.MyOrderViews;
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
    public class MyOrderViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public MyOrderViewModel()
        {
            using (UserDialogs.Instance.Loading("Đang tải..."))
            {
                firebase = new FirebaseHelper();
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                Orders = Task.Run(async () => await firebase.GetAllUserOrders(user.Email)).Result;
                //OrdersCount = Orders.Count;
                if (Orders.Count == 0)
                    IsVisible = true;

                this.ItemTapped = new Command(GotoOrderDetail);
                this.BackButton = new Command(Back);
            }
        }


        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        private void GotoOrderDetail(object obj)
        {
            var detail = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as OrderModel;
            Application.Current.MainPage.Navigation.PushAsync(new MyOrderDetailPage(detail));
        }

        #region Properties
        private List<OrderModel> _orders;
        public List<OrderModel> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }
        private int _ordersCount;
        public int OrdersCount
        {
            get { return _ordersCount; }
            set
            {
                _ordersCount = value;
                OnPropertyChanged(nameof(OrdersCount));
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
        #endregion

        #region Command
        public Command ItemTapped { get; }
        public Command BackButton { get; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
