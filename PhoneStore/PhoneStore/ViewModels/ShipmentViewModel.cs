using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using PhoneStore.View.ShipmentViews;
using Plugin.FirebaseAuth;
using Plugin.Toast;
using System;
using System.Collections.Generic;
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
            using (UserDialogs.Instance.Progress("Vui lòng chờ...", null, null, true, MaskType.Gradient))
            {
                firebase = new FirebaseHelper();
                Order = new OrderModel();
                var carts = Task.Run(async () => await App.SQLiteDb.GetItemsAsync()).Result;
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                var userDB = Task.Run(async () => await App.SQLiteDb.GetUserAsync(user.Email)).Result;
                Order.Carts = carts;
                Shipping = 30000;
                foreach (var cart in Order.Carts)
                {
                    Price += cart.Quantity * cart.Price;
                }
                Height = (120 * Order.Carts.Count) + 60;
                Address = userDB.Address;
                UserPhone = userDB.Phone;
                UserName = userDB.FullName;
                if (Promo != null)
                {
                    IsShow = true;
                    Discount = (Promo.Discount * Price) / 100;
                    TotalPrice = Price + Shipping - Discount;
                }
                else
                {
                    TotalPrice = Price + Shipping;
                }

                this.CreateOrder = new Command(CreateNewOrder);
                this.ChangeLocationTapped = new Command(ChangeLocation);
                this.BackButton = new Command(Back);
                this.PromoTapped = new Command(PromoCmd);
            }
        }

        public ShipmentViewModel(OrderModel order)
        {
            using (UserDialogs.Instance.Progress("Đang tải...", null, null, true, MaskType.Gradient))
            {
                this.Order = order;
                this.Promo = order.Promo;
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                Address = Order.Address;
                Note = Order.Note;
                UserName = Order.UserName;
                UserPhone = Order.UserPhone;
                Shipping = 30000;
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
                    Price += cart.Quantity * cart.Price;
                }
                Height = (120 * Order.Carts.Count) + 60;
                if (Promo != null)
                {
                    IsShow = true;
                    Discount = (Promo.Discount * Price) / 100;
                    TotalPrice = Price + Shipping - Discount;
                }
                else
                {
                    TotalPrice = Price + Shipping;
                }

                this.CreateOrder = new Command(CreateNewOrder);
                this.ChangeLocationTapped = new Command(ChangeLocation);
                this.BackButton = new Command(Back);
                this.PromoTapped = new Command(PromoCmd);
            }
        }
        
        #region Logic

        private void PromoCmd(object obj)
        {
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            Order.UserEmail = user.Email;
            Order.CreatedOn = DateTime.Now;
            Order.Note = Note;
            Order.Address = Address;
            Order.UserName = UserName;
            Order.UserPhone = UserPhone;
            Order.Promo = Promo;
            if (isChecked == false)
            {
                Order.Payment = OrderModel.PaymentType.COD;
            }
            else
            {
                Order.Payment = OrderModel.PaymentType.Bank;
            }
            Application.Current.MainPage.Navigation.PushAsync(new ChoosePromoPage(Order));
        }

        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        private void ChangeLocation(object obj)
        {
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            Order.UserEmail = user.Email;
            Order.CreatedOn = DateTime.Now;
            Order.Note = Note;
            Order.Address = Address;
            Order.UserName = UserName;
            Order.UserPhone = UserPhone;
            Order.Promo = Promo;
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
            using (UserDialogs.Instance.Loading("Đang xử lý...", null, null, true, MaskType.Gradient))
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
                    Order.UserPhone = UserPhone;
                    Order.UserName = UserName;
                    Order.TotalPrice = TotalPrice;
                    if (Promo != null)
                    {
                        Promo.IsUsed = true;
                        Order.Promo = Promo;
                    }
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
                    
                    await firebase.UpdateUserPromo(Promo);
                    await Application.Current.MainPage.DisplayAlert("Thông báo", "Đã đặt hàng thành công!", "Đã hiểu");
                    await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                    Application.Current.MainPage = new NavigationPage(new HomePage());
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

        private bool _canshow;
        public bool IsShow
        {
            get { return _canshow; }
            set
            {
                _canshow = value; ;
                OnPropertyChanged(nameof(IsShow));
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value; ;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
        private decimal _discount;
        public decimal Discount
        {
            get { return _discount; }
            set
            {
                _discount = value; ;
                OnPropertyChanged(nameof(Discount));
            }
        }
        private decimal _price;
        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        private QRPromoModel _promo;
        public QRPromoModel Promo
        {
            get { return _promo; }
            set
            {
                _promo = value;
                OnPropertyChanged(nameof(Promo));
            }
        }
        private decimal _shipping;
        public decimal Shipping
        {
            get { return _shipping; }
            set
            {
                _shipping = value;
                OnPropertyChanged(nameof(Shipping));
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
        private string _username;
        public string UserName
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        private string _userphone;
        public string UserPhone
        {
            get { return _userphone; }
            set
            {
                _userphone = value;
                OnPropertyChanged(nameof(UserPhone));
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
        public Command PromoTapped { get; }
        public Command ItemTapped { get; }
        #endregion
    }
}
