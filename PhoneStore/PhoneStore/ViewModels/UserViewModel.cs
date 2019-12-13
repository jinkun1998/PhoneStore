
using Firebase.Database;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
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
    public class UserViewModel : INotifyPropertyChanged
    {
        FirebaseHelper firebase;
        public UserViewModel()
        {
            firebase = new FirebaseHelper();
            this.SignInTapped = new Command(GotoSignin);
            this.SignUpTapped = new Command(GotoSignup);
            this.LoginTapped = new Command(LoginAsync);
            this.RegisterTapped = new Command(Register);
        }

        private async void LoginAsync(object obj)
        {
            try
            {
                var auth = await CrossFirebaseAuth.Current.Instance.SignInWithEmailAndPasswordAsync(Email, Pwd);
                

                await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                Application.Current.MainPage = new NavigationPage(new HomePage());
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
                var auth = await CrossFirebaseAuth.Current.Instance.CreateUserWithEmailAndPasswordAsync(Email, Pwd);


                await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                Application.Current.MainPage = new NavigationPage(new HomePage());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Đăng nhập thất bại", "Email hoặc mật khẩu không hợp lệ! \nVui lòng kiểm tra lại.", "Xác nhận");
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
