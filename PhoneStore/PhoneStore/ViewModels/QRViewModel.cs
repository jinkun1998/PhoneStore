using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace PhoneStore.ViewModels
{
    public class QRViewModel
    {
        FirebaseHelper firebase;
        public QRViewModel()
        {
            firebase = new FirebaseHelper();
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            Email = user.Email;

            this.BackButton = new Command(Back);
            this.ScanButton = new Command(Scan);
        }

        private void Scan(object obj)
        {
            Scanner();
        }

        public async void Scanner()
        {
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            var ScannerPage = new ZXingScannerPage();

            ScannerPage.OnScanResult += (result) => {
                // Parar de escanear
                ScannerPage.IsScanning = false;

                // Alert com o código escaneado
                Device.BeginInvokeOnMainThread(async () => {
                    if (result != null && result.Text != user.Email)
                    {
                        var allUsers = Task.Run(async () => await firebase.GetAllUsers()).Result;
                        var exitsUser = allUsers.Where(it => it.Email == user.Email).FirstOrDefault();
                        if (exitsUser == null)
                        {
                            var allUserPromos = Task.Run(async () => await firebase.GetAllPromos()).Result;
                            var exitsPromo = allUserPromos.Where(it => it.QREmail == result.Text).FirstOrDefault();
                            if (exitsPromo == null)
                            {
                                QRPromoModel promo = new QRPromoModel();
                                promo.UserEmail = user.Email;
                                promo.QREmail = result.Text;
                                promo.Discount = 5;
                                promo.IsUsed = false;
                                await firebase.AddUserPromo(promo);
                                Vibration.Vibrate();
                                await Application.Current.MainPage.DisplayAlert("Thành công", "Bạn đã thêm mã thành công!", "Đã hiểu");
                                await Application.Current.MainPage.Navigation.PushAsync(new QRPage(), false);
                                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                            }
                            else
                            {
                                Vibration.Vibrate();
                                await Application.Current.MainPage.DisplayAlert("Lỗi", "Bạn đã thêm mã này trước đó rồi!", "Đã hiểu");
                                await Application.Current.MainPage.Navigation.PushAsync(new QRPage(), false);
                                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                            }
                        }
                        else
                        {
                            Vibration.Vibrate();
                            await Application.Current.MainPage.DisplayAlert("Lỗi", "Mã không tồn tại trên hệ thống!\nVui lòng thử mã khác.", "Đã hiểu");
                            await Application.Current.MainPage.Navigation.PushAsync(new QRPage(), false);
                            Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                        }
                    }
                    else
                    {
                        Vibration.Vibrate();
                        await Application.Current.MainPage.DisplayAlert("Lỗi", "Bạn không thể tự giới thiệu chính mình!", "Đã hiểu");
                        await Application.Current.MainPage.Navigation.PushAsync(new QRPage(), false);
                        Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 2]);
                    }
                });
            };
            await Application.Current.MainPage.Navigation.PushAsync(ScannerPage);
        }

        public string Email { get; set; }

        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync(true);
        }

        public Command BackButton { get; }
        public Command ScanButton { get; }
    }
}
