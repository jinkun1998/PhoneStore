using PhoneStore.Models;
using PhoneStore.View;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PhoneStore.ViewModels
{
    public class MapsViewModel : INotifyPropertyChanged
    {
        public MapsViewModel()
        {
            this.FindAddressTapped = new Command(FindAddress);
            this.ChooseThisLocationTapped = new Command(ChooseThisLocation);
            this.DetectLocationTapped = new Command(DetectLocation);
        }

        public MapsViewModel(OrderModel order)
        {
            this.Order = order;

            this.FindAddressTapped = new Command(FindAddress);
            this.ChooseThisLocationTapped = new Command(ChooseThisLocation);
            this.DetectLocationTapped = new Command(DetectLocation);
        }

        private async void DetectLocation(object obj)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await Application.Current.MainPage.DisplayAlert("Yêu cầu quyền truy cập", "Ứng dụng cần truy cập vị trí để có thể tìm vị trí của bạn.", "Đã hiểu");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    Xamarin.Forms.Maps.Map map = obj as Xamarin.Forms.Maps.Map;
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 1;

                    var location = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));
                    Xamarin.Forms.Maps.Position position = new Xamarin.Forms.Maps.Position(location.Latitude, location.Longitude);
                    map.Pins.Clear();
                    if (position != null)
                    {
                        var placemarks = await Geocoding.GetPlacemarksAsync(position.Latitude, position.Longitude);
                        var placemark = placemarks?.FirstOrDefault();
                        Pin pin = new Pin
                        {
                            Position = position,
                            Label = placemark.SubThoroughfare + " " + placemark.Thoroughfare,
                            Address = placemark.SubThoroughfare + " " + placemark.Thoroughfare + ", " + placemark.SubAdminArea + ", " + placemark.CountryName,
                            Type = PinType.Place,
                        };
                        Address = pin.Address;
                        map.Pins.Add(pin);
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(50)));
                    }
                    //Permission granted, do what you want do.
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Yêu cầu quyền truy cập", "Yêu cầu quyền truy cập đã bị từ chối. Hẫy thử lại sau.", "Đã hiểu");
                }
            }
            catch (Exception ex)
            {
                //...
            }
        }

        private void ChooseThisLocation(object obj)
        {
            Order.Address = Address;
            Application.Current.MainPage.Navigation.PushAsync(new ShipmentPage(Order));
            Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
            Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
        }

        private async void FindAddress(object obj)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await Application.Current.MainPage.DisplayAlert("Yêu cầu quyền truy cập", "Ứng dụng cần truy cập vị trí để có thể tìm vị trí của bạn.", "Đã hiểu");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    Xamarin.Forms.Maps.Map map = obj as Xamarin.Forms.Maps.Map;
                    map.Pins.Clear();
                    var positions = new List<Xamarin.Forms.Maps.Position>(await new Geocoder().GetPositionsForAddressAsync(Address));
                    Xamarin.Forms.Maps.Position position = positions.FirstOrDefault();
                    if (position != null)
                    {
                        var placemarks = await Geocoding.GetPlacemarksAsync(position.Latitude, position.Longitude);
                        var placemark = placemarks?.FirstOrDefault();
                        Pin pin = new Pin
                        {
                            Position = position,
                            Label = placemark.SubThoroughfare + " " + placemark.Thoroughfare,
                            Address = placemark.SubThoroughfare + " " + placemark.Thoroughfare + ", " + placemark.SubAdminArea + ", " + placemark.CountryName,
                            Type = PinType.Place,
                        };
                        Address = pin.Address;
                        map.Pins.Add(pin);
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(50)));
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Lỗi", "Không tìm thấy địa điểm!", "Đã hiểu");
                    }
                    //Permission granted, do what you want do.
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Yêu cầu quyền truy cập", "Yêu cầu quyền truy cập đã bị từ chối. Hẫy thử lại sau.", "Đã hiểu");
                }
            }
            catch (Exception ex)
            {
                //...
            }
        }
        #region Properties
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
        #endregion

        #region Command
        public Command FindAddressTapped { get; }
        public Command ChooseThisLocationTapped { get; }
        public Command DetectLocationTapped { get; }
        #endregion
    }
}
