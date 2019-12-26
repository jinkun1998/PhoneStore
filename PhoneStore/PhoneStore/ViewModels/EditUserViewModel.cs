using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using Plugin.FirebaseAuth;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Syncfusion.XForms.Buttons;
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
            using (UserDialogs.Instance.Progress("Đang lưu...", null, null, true, MaskType.Gradient))
            {
                firebase = new FirebaseHelper();
                firebaseStorage = new FirebaseStorageHelper();
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
                User = Task.Run(async () => await App.SQLiteDb.GetUserAsync(user.Email)).Result;
                Image = User.AvatarLink;
                Name = User.FullName;
                Email = User.Email;
                Phone = User.Phone;
                Address = User.Address;
                DoB = User.DoB;
                IsEdit = false;

                this.SaveUserTapped = new Command(SaveUser);
                this.BackButton = new Command(Back);
                this.ChangeAvatarTapped = new Command(ChangeAvatar);
                this.EditTapped = new Command(Edit);
            }
        }

        private void Edit(object obj)
        {
            IsEdit = true;
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
                    using (UserDialogs.Instance.Progress("Đang xử lý...", null, null, true, MaskType.Gradient))
                    {
                        var imageLink = Task.Run(async () => await firebaseStorage.UploadFile(selectedImage.GetStream(), Path.GetFileName(User.Email))).Result;
                        if (imageLink != null)
                        {
                            Image = imageLink;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Lỗi!", "Không thể upload ảnh lên server!\nThử lại sau.", "Đã hiểu");
                        }
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
            using (UserDialogs.Instance.Progress("Vui lòng chờ...", null, null, true, MaskType.Gradient))
            {
                UserModel user = new UserModel();
                user.Address = Address;
                user.AvatarLink = Image;
                user.DoB = DoB;
                user.Email = Email;
                user.FullName = Name;
                user.Phone = Phone;
                await App.SQLiteDb.SaveUserAsync(user);
                await firebase.UpdateUser(user);
                await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
                Application.Current.MainPage = new NavigationPage(new HomePage());
            }
            await Application.Current.MainPage.DisplayAlert("Thông báo", "Đã sửa đổi thông tin thành công!", "Đã hiểu");
            IsEdit = false;
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
        private bool _isedit;
        public bool IsEdit
        {
            get { return _isedit; }
            set
            {
                _isedit = value;
                OnPropertyChanged(nameof(IsEdit));
            }
        }
        #endregion

        #region Command
        public Command SaveUserTapped { get; }
        public Command BackButton { get; }
        public Command ChangeAvatarTapped { get; }
        public Command EditTapped { get; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
