using Firebase.Auth;
using Firebase.Database;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public UserViewModel()
        {
            firebase = new FirebaseHelper();
            this.SignInTapped = new Command(GotoSignin);
            this.SignUpTapped = new Command(GotoSignup);
            this.LoginTapped = new Command(Login);
            this.RegisterTapped = new Command(Register);
        }

        private void Login(object obj)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseHelper.ApiKey));
                var auth = authProvider.SignInWithEmailAndPasswordAsync(Email, Pwd).Result;
                auth = Task.Run(() => auth.GetFreshAuthAsync()).Result;

                var firebaseClient = new FirebaseClient(
                "https://thebossapp-dee9f.firebaseio.com/",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = async () => (await auth.GetFreshAuthAsync().ConfigureAwait(true)).FirebaseToken
                });
                FirebaseHelper.userToken = auth.FirebaseToken;

                UserModel user = new UserModel();
                user.Email = auth.User.Email;
                user.AvatarLink = auth.User.PhotoUrl;
                user.FullName = auth.User.DisplayName;
                user.Phone = auth.User.PhoneNumber;
                user.Token = auth.FirebaseToken;
                user.IsLogged = true;
                Task.Run(async () => await App.SQLiteDb.SaveUserAsync(user));

                Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                Application.Current.MainPage = new NavigationPage(new HomePage());
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Đăng nhập thất bại", "Email hoặc mật khẩu không hợp lệ! \nVui lòng kiểm tra lại.", "Xác nhận");
            }
        }

        private void Register(object obj)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseHelper.ApiKey));
                var auth = authProvider.CreateUserWithEmailAndPasswordAsync(Email, Pwd, "", true).Result;
                auth = Task.Run(() => auth.GetFreshAuthAsync()).Result;

                var firebaseClient = new FirebaseClient(
                "https://thebossapp-dee9f.firebaseio.com/",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = async () => (await auth.GetFreshAuthAsync().ConfigureAwait(true)).FirebaseToken
                });
                FirebaseHelper.userToken = auth.FirebaseToken;
                FirebaseHelper.userEmail = auth.User.Email;
                Application.Current.MainPage.DisplayAlert("Thông báo", "Đăng ký thành công", "Xác nhận");
                Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                Application.Current.MainPage = new NavigationPage(new HomePage());

            }
            catch
            {
                Application.Current.MainPage.DisplayAlert("Đăng ký thất bại", "Email hoặc mật khẩu không hợp lệ! \nVui lòng kiểm tra lại.", "Xác nhận");
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
        #endregion

        #region Command
        public Command SignInTapped { get; }
        public Command SignUpTapped { get; }
        public Command LoginTapped { get; }
        public Command RegisterTapped { get; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
