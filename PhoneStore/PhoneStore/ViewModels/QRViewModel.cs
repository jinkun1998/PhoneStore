using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using Plugin.FirebaseAuth;
using Plugin.Toast;
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
                        if (exitsUser != null)
                        {
                            var allUserPromos = Task.Run(async () => await firebase.GetAllPromos()).Result;
                            var exitsPromo = allUserPromos.Where(it => it.QREmail == result.Text).FirstOrDefault();
                            if (exitsPromo == null)
                            {
                                QRPromoModel promo = new QRPromoModel();
                                int tempCode = 000000;
                                do
                                {
                                    Random rd = new Random();
                                    tempCode = rd.Next(0, 999999);
                                } while (allUserPromos.Where(it => it.Code == tempCode.ToString()).Count() != 0);

                                promo.Code = tempCode.ToString("000000");
                                promo.UserEmail = user.Email;
                                promo.QREmail = result.Text;
                                promo.Discount = 5;
                                promo.IsUsed = false;
                                await firebase.AddUserPromo(promo);
                                Vibration.Vibrate();
                                await Application.Current.MainPage.Navigation.PopAsync(true);
                                CrossToastPopUp.Current.ShowToastMessage("Đã thêm mã thành công!");
                            }
                            else
                            {
                                Vibration.Vibrate();
                                await Application.Current.MainPage.Navigation.PopAsync(true);
                                CrossToastPopUp.Current.ShowToastMessage("Bạn đã thêm mã này trước đó rồi!");
                            }
                        }
                        else
                        {
                            Vibration.Vibrate();
                            await Application.Current.MainPage.Navigation.PopAsync(true);
                            CrossToastPopUp.Current.ShowToastMessage("Mã không tồn tại trên hệ thống!");
                        }
                    }
                    else
                    {
                        Vibration.Vibrate();
                        await Application.Current.MainPage.Navigation.PopAsync(true);
                        CrossToastPopUp.Current.ShowToastMessage("Bạn không thể tự giới thiệu chính mình!");
                    }
                });
            };
            await Application.Current.MainPage.Navigation.PushAsync(ScannerPage, true);
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
