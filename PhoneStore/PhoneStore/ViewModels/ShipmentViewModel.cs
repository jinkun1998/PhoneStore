using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using PhoneStore.View.ShipmentViews;
using Plugin.FirebaseAuth;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class ShipmentViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public ShipmentViewModel()
        {
            using (UserDialogs.Instance.Loading("Đang tải..."))
            {
                firebase = new FirebaseHelper();
                Order = new OrderModel();
                var carts = Task.Run(async () => await App.SQLiteDb.GetItemsAsync()).Result;
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                var userDB = Task.Run(async () => await App.SQLiteDb.GetUserAsync(user.Email)).Result;
                Order.Carts = carts;
                foreach (var cart in Order.Carts)
                {
                    TotalPrice += cart.Quantity * cart.Price;
                }
                Address = userDB.Address;

                this.CreateOrder = new Command(CreateNewOrder);
                this.ChangeLocationTapped = new Command(ChangeLocation);
                this.BackButton = new Command(Back);
            }
        }

        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        public ShipmentViewModel(OrderModel order)
        {
            using (UserDialogs.Instance.Loading("Đang tải..."))
            {
                this.Order = order;
                Address = Order.Address;
                Note = Order.Note;
                switch (Order.Payment)
                {
                    case OrderModel.PaymentType.COD:
                        this.isChecked = true;
                        break;
                    case OrderModel.PaymentType.Bank:
                        this.isChecked = false;
                        break;
                    default:
                        break;
                }
                firebase = new FirebaseHelper();
                foreach (var cart in Order.Carts)
                {
                    TotalPrice += cart.Quantity * cart.Price;
                }

                this.CreateOrder = new Command(CreateNewOrder);
                this.ChangeLocationTapped = new Command(ChangeLocation);
            }
        }

        #region Logic
        private void ChangeLocation(object obj)
        {
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            Order.UserEmail = user.Email;
            Order.CreatedOn = DateTime.Now;
            Order.Note = Note;
            Order.Address = Address;
            if (isChecked == false)
            {
                Order.Payment = OrderModel.PaymentType.COD;
            }
            else
            {
                Order.Payment = OrderModel.PaymentType.Bank;
            }
            Application.Current.MainPage.Navigation.PushAsync(new MapsPage(Order));
        }

        private async void CreateNewOrder(object obj)
        {
            using (UserDialogs.Instance.Loading("Đang xử lý..."))
            {
                if (Address != null)
                {
                    int tempCode = 000000;
                    var allOrders = Task.Run(async () => await firebase.GetAllOrders()).Result;
                    do
                    {
                        Random rd = new Random();
                        tempCode = rd.Next(0, 999999);
                    } while (allOrders.Where(it => it.Code == tempCode.ToString()).Count() != 0);

                    Order.Code = tempCode.ToString("000000");
                    var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                    Order.UserEmail = user.Email;
                    Order.CreatedOn = DateTime.Now;
                    Order.Note = Note;
                    Order.TotalPrice = TotalPrice;
                    Order.Address = Address;
                    Order.Status = OrderModel.OrderStatus.Ordered;
                    if (isChecked == false)
                    {
                        Order.Payment = OrderModel.PaymentType.COD;
                    }
                    else
                    {
                        Order.Payment = OrderModel.PaymentType.Bank;
                    }
                    #region Tạo thông tin shipment
                    Shipment = new ShipmentDetailModel();
                    Shipment.Title = "Đã đặt hàng";
                    Shipment.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    Shipment.Time = DateTime.Now.ToString("HH:ss");
                    Shipment.Status = Syncfusion.XForms.ProgressBar.StepStatus.InProgress;
                    Shipment.ProgressValue = 0;
                    Order.Shipments.Add(Shipment);
                    Shipment = new ShipmentDetailModel();
                    Shipment.Title = "Đã duyệt";
                    Shipment.Date = "";
                    Shipment.Time = "";
                    Shipment.Status = Syncfusion.XForms.ProgressBar.StepStatus.NotStarted;
                    Shipment.ProgressValue = 0;
                    Order.Shipments.Add(Shipment);
                    Shipment = new ShipmentDetailModel();
                    Shipment.Title = "Đang giao";
                    Shipment.Date = "";
                    Shipment.Time = "";
                    Shipment.Status = Syncfusion.XForms.ProgressBar.StepStatus.NotStarted;
                    Shipment.ProgressValue = 0;
                    Order.Shipments.Add(Shipment);
                    Shipment = new ShipmentDetailModel();
                    Shipment.Title = "Đã giao";
                    Shipment.Date = "";
                    Shipment.Time = "";
                    Shipment.Status = Syncfusion.XForms.ProgressBar.StepStatus.NotStarted;
                    Shipment.ProgressValue = 0;
                    Order.Shipments.Add(Shipment);
                    #endregion

                    await Task.Run(async () => await firebase.AddUserOrder(Order).ConfigureAwait(true));
                    foreach (var item in Order.Carts)
                    {
                        await App.SQLiteDb.DeleteItemAsync(item);
                        await Task.Delay(500);
                    }
                    await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                    Application.Current.MainPage = new NavigationPage(new HomePage());
                    await Application.Current.MainPage.DisplayAlert("Thông báo", "Đã đặt hàng thành công!", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Thông báo", "Chưa có thông tin địa chỉ!", "Đã hiểu");
                }
            }
        }
        #endregion 

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }

        #region Propereties
        private OrderModel _order;
        public OrderModel Order
        {
            get { return _order; }
            set
            {
                _order = value;
                OnPropertyChanged(nameof(Order));
            }
        }
        public bool isChecked { get; set; }
        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        private string _note;
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                OnPropertyChanged(nameof(Note));
            }
        }
        private ShipmentDetailModel _shipment;
        public ShipmentDetailModel Shipment
        {
            get { return _shipment; }
            set
            {
                _shipment = value;
                OnPropertyChanged(nameof(Shipment));
            }
        }
        #endregion

        #region Command
        public Command CreateOrder { get; }
        public Command ChooseDelivery { get; }
        public Command ChangeLocationTapped { get; }
        public Command BackButton { get; }
        #endregion
    }
}
