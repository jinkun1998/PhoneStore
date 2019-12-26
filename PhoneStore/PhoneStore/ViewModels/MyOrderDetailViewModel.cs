using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View.MainViews.User.MyOrderViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class MyOrderDetailViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public MyOrderDetailViewModel()
        {
            this.BackButton = new Command(Back);
        }

        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        public MyOrderDetailViewModel(OrderModel order)
        {
            firebase = new FirebaseHelper();
            Order = order;
            Height = (120 * Order.Carts.Count) + 60;


            this.CancelOrderTapped = new Command(CancelOrder);
            this.BackButton = new Command(Back);
        }

        #region Logic
        private async void CancelOrder(object obj)
        {
            var result = await Application.Current.MainPage.DisplayAlert("Thông báo", "Bạn có chắc chắn muốn hủy đơn hàng này?", "Chắc chắn", "Hủy bỏ");
            if (result)
            {
                using (UserDialogs.Instance.Loading("Vui lòng chờ...", null, null, true, MaskType.Gradient))
                {
                    Order.Status = OrderModel.OrderStatus.Cancelled;
                    await Task.Run(async () => await firebase.UpdateUserOrder(Order)).ConfigureAwait(true);
                    Thread.Sleep(500);
                    await Application.Current.MainPage.Navigation.PushAsync(new MyOrderPage());
                    Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                    Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                }
                
            }
        }
        #endregion

        #region Properties
        private int _height;
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        public OrderModel Order { get; set; }
        #endregion

        #region Command
        public Command CancelOrderTapped { get; set; }
        public Command BackButton { get; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
