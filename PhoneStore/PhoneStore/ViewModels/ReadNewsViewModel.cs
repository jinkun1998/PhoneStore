using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class ReadNewsViewModel
    {
        public ReadNewsViewModel(string link)
        {
            using (UserDialogs.Instance.Progress("Đang tải..."))
            {
                this.Link = link;

                this.BackButton = new Command(Back);
            }
        }

        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public string Link { get; set; }
        public Command BackButton { get; }
    }
}
