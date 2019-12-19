using PhoneStore.Firebase;
using PhoneStore.Models;
using Plugin.FirebaseAuth;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class EditUserViewModel : INotifyPropertyChanged
    {
        public FirebaseHelper firebase;
        public FirebaseStorageHelper firebaseStorage;
        public EditUserViewModel()
        {
            firebase = new FirebaseHelper();
            firebaseStorage = new FirebaseStorageHelper();
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            User = App.SQLiteDb.GetUserAsync(User.Email).Result;
            Image = User.AvatarLink;
            Name = User.FullName;
            Email = User.Email;
            Phone = User.Phone;
            Address = User.Address;
            DoB = User.DoB;

            this.SaveUserTapped = new Command(SaveUser);
            this.BackButton = new Command(Back);
            this.ChangeAvatarTapped = new Command(ChangeAvatar);
        }

        private async void ChangeAvatar(object obj)
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                var mediaOptions = new PickMediaOptions()
                {
                    PhotoSize = PhotoSize.Medium,
                };
                var selectedImage = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
                if (selectedImage != null)
                {
                    var imageLink = Task.Run(async () => await firebaseStorage.UploadFile(selectedImage.GetStream(), Path.GetFileName(selectedImage.Path))).Result;
                    if (imageLink != null)
                    {
                        Image = imageLink;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Lỗi!", "Không thể upload ảnh lên server!\nThwur lại sau.", "Đã hiểu");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Lỗi!", "Lỗi chọn hình ảnh!\nThwur lại sau.", "Đã hiểu");
                }
            }
        }

        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void SaveUser(object obj)
        {
            await App.SQLiteDb.SaveUserAsync(User);
            await firebase.UpdateUser(User);
        }

        #region Propereties
        private UserModel _user;
        public UserModel User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
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
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
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
        public Command SaveUserTapped { get; }
        public Command BackButton { get; }
        public Command ChangeAvatarTapped { get; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
