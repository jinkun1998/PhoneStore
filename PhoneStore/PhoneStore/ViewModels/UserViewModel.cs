
using Acr.UserDialogs;
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
using System.Threading;
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
            this.ContinueTapped = new Command(Continue);
        }

        private async void Continue(object obj)
        {
            UserDialogs.Instance.ShowLoading("Vui lòng chờ");
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

            await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
            Application.Current.MainPage = new NavigationPage(new HomePage());
            UserDialogs.Instance.HideLoading();
        }

        #region logic
        private async void LoginAsync(object obj)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Vui lòng chờ");
                var auth = await CrossFirebaseAuth.Current.Instance.SignInWithEmailAndPasswordAsync(Email, Pwd);
                await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                Application.Current.MainPage = new NavigationPage(new HomePage());
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                await Application.Current.MainPage.DisplayAlert("Đăng nhập thất bại", "Email hoặc mật khẩu không hợp lệ! \nVui lòng kiểm tra lại.", "Xác nhận");
            }
            
        }

        private async void Register(object obj)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Vui lòng chờ");
                var auth = await CrossFirebaseAuth.Current.Instance.CreateUserWithEmailAndPasswordAsync(Email, Pwd);
                await Application.Current.MainPage.Navigation.PushAsync(new AddnewUserPage());
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
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
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
