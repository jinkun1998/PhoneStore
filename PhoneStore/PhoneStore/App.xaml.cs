using PhoneStore.View;
using PhoneStore.View.MainViews;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhoneStore
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTU2MDEwQDMxMzcyZTMzMmUzMGR2enYvelhPZmo2UmgxZE1yR2RQWWcxTU85NWREVi9zS0MyMkZSQ2xFZGc9");

            MainPage = new NavigationPage(new HomePage())
            {
                BarTextColor = Xamarin.Forms.Color.Black,
                BackgroundColor = Xamarin.Forms.Color.White,
                BarBackgroundColor = Xamarin.Forms.Color.White,

            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
