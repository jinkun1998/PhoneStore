
using Acr.UserDialogs;
using Firebase.Database;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using Plugin.FirebaseAuth;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        FirebaseStorageHelper storageHelper;
        public UserViewModel()
        {
            firebase = new FirebaseHelper();
            storageHelper = new FirebaseStorageHelper();
            DoB = DateTime.Now;

            this.SignInTapped = new Command(GotoSignin);
            this.SignUpTapped = new Command(GotoSignup);
            this.LoginTapped = new Command(LoginAsync);
            this.RegisterTapped = new Command(Register);
            this.ContinueTapped = new Command(Continue);
            this.ChangeAvatarTapped = new Command(ChangeAvatar);
            this.ForgotPassTapped = new Command(ResetPass);
        }

        private async void ResetPass(object obj)
        {
            await CrossFirebaseAuth.Current.Instance.SendPasswordResetEmailAsync(Email);
        }

        private async void ChangeAvatar(object obj)
        {
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                var mediaOptions = new PickMediaOptions()
                {
                    PhotoSize = PhotoSize.Medium,
                };
                var selectedImage = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
                using (UserDialogs.Instance.Loading("Đang tải...", null, null, true, MaskType.Gradient))
                {
                    if (selectedImage != null)
                    {
                        var imageLink = Task.Run(async () => await storageHelper.UploadFile(selectedImage.GetStream(), Path.GetFileName(user.Email))).Result;
                        if (imageLink != null)
                        {
                            Image = imageLink;
                            UserDialogs.Instance.HideLoading();
                        }
                        else
                        {
                            UserDialogs.Instance.HideLoading();
                            await Application.Current.MainPage.DisplayAlert("Lỗi!", "Không thể upload ảnh lên server!\nThử lại sau.", "Đã hiểu");
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Lỗi!", "Lỗi chọn hình ảnh!\nThwur lại sau.", "Đã hiểu");
                    }
                }
            }
        }

        private async void Continue(object obj)
        {
            using (UserDialogs.Instance.Loading("Vui lòng chờ...", null, null, true, MaskType.Gradient))
            {
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                UserModel newUser = new UserModel();
                newUser.Address = Address;
                newUser.AvatarLink = Image;
                newUser.DoB = DoB;
                newUser.Email = user.Email;
                newUser.FullName = Name;
                newUser.Phone = Phone;
                await App.SQLiteDb.SaveUserAsync(newUser);
                await firebase.AddUser(newUser);
                //await Application.Current.MainPage.Navigation.PushAsync(new VerifyEmailPage());
                await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                Application.Current.MainPage = new NavigationPage(new HomePage());
            }
        }

        #region logic
        private async void LoginAsync(object obj)
        {
            try
            {
                using (UserDialogs.Instance.Loading("Vui lòng chờ...", null, null, true, MaskType.Gradient))
                {
                    await CrossFirebaseAuth.Current.Instance.SignInWithEmailAndPasswordAsync(Email, Pwd);
                    var checkUser = CrossFirebaseAuth.Current.Instance.CurrentUser;
                    //if (checkUser.IsEmailVerified)
                    //{
                    var user = Task.Run(async () => await firebase.GetUser(Email)).Result;
                    await App.SQLiteDb.SaveUserAsync(user);
                    await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                    Application.Current.MainPage = new NavigationPage(new HomePage());
                    //}
                    //else
                    //{
                    //await Application.Current.MainPage.Navigation.PushAsync(new VerifyEmailPage());
                    //}
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Đăng nhập thất bại", "Email hoặc mật khẩu không hợp lệ! \nVui lòng kiểm tra lại.", "Xác nhận");
            }
            
        }

        private async void Register(object obj)
        {
            try
            {
                using (UserDialogs.Instance.Loading("Vui lòng chờ...", null, null, true, MaskType.Gradient))
                {
                    await CrossFirebaseAuth.Current.Instance.CreateUserWithEmailAndPasswordAsync(Email, Pwd);
                    await CrossFirebaseAuth.Current.Instance.CurrentUser.SendEmailVerificationAsync();
                    await Application.Current.MainPage.Navigation.PushAsync(new AddnewUserPage());
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Đăng ký thất bại", "Email hoặc mật khẩu không hợp lệ! \nVui lòng kiểm tra lại.", "Xác nhận");
            }
        }

        private void GotoSignup(object obj)
        {
            Application.Current.MainPage.Navigation.PushAsync(new SignUpPage());
        }

        private void GotoSignin(object obj)
        {
            Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
        }
        #endregion

        #region Properties
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _pwd;
        public string Pwd
        {
            get { return _pwd; }
            set
            {
                _pwd = value;
                OnPropertyChanged();
            }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _image;
        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
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
        private DateTime _dob;
        public DateTime DoB
        {
            get { return _dob; }
            set
            {
                _dob = value;
                OnPropertyChanged(nameof(DoB));
            }
        }
        #endregion

        #region Command
        public Command SignInTapped { get; }
        public Command SignUpTapped { get; }
        public Command LoginTapped { get; }
        public Command RegisterTapped { get; }
        public Command ContinueTapped { get; }
        public Command ChangeAvatarTapped { get; }
        public Command ForgotPassTapped { get; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
