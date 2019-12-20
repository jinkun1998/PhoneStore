using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View.MainViews.User.MyOrderViews;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class MyOrderDetailViewModel
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

            this.CancelOrderTapped = new Command(CancelOrder);
            this.BackButton = new Command(Back);
        }

        #region Logic
        private async void CancelOrder(object obj)
        {
            var result = await Application.Current.MainPage.DisplayAlert("Thông báo", "Bạn có chắc chắn muốn hủy đơn hàng này?", "Chắc chắn", "Hủy bỏ");
            if (result)
            {
                UserDialogs.Instance.ShowLoading("Vui lòng chờ");
                Order.Status = OrderModel.OrderStatus.Cancelled;
                await Task.Run(async () => await firebase.UpdateUserOrder(Order)).ConfigureAwait(true);
                Thread.Sleep(500);
                await Application.Current.MainPage.Navigation.PushAsync(new MyOrderPage());
                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                UserDialogs.Instance.HideLoading();
            }
        }
        #endregion

        #region Properties
        public OrderModel Order { get; set; }
        #endregion

        #region Command
        public Command CancelOrderTapped { get; set; }
        public Command BackButton { get; }
        #endregion
    }
}
