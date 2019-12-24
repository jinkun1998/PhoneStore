
using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.SQLite;
using PhoneStore.View;
using PhoneStore.View.MainViews;
using PhoneStore.View.ShipmentViews;
using Plugin.FirebaseAuth;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;

namespace PhoneStore
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTg1NTE1QDMxMzcyZTM0MmUzMEdlQnpRaWd6MEVKbjgxdXErL20xQ1VyTzg5bWljRTFtWERER2lpcUUvclU9");
            CheckDatabase();
            CheckNetwork();
            //MainPage = new NavigationPage(new NoConnecitvityPage());
        }

        protected override void OnStart()
        {
            AppCenter.Start("android=2f9e0ba7-24a1-48c6-ae44-2c26781b8d70;", typeof(Analytics), typeof(Crashes), typeof(Push));
            //AppCenter.Start("android=2f9e0ba7-24a1-48c6-ae44-2c26781b8d70;" +
            //      "uwp={Your UWP App secret here};" +
            //      "ios={Your iOS App secret here}",
            //      typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public void CheckDatabase()
        {
            if (db == null)
            {
                db = new SQLiteHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhoneStore.db3"));
            }
        }

        public void CheckNetwork()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                CheckUserSignIn();
            }
            else
            {
                MainPage = new NavigationPage(new NoConnecitvityPage());
            }
        }

        public void CheckUserSignIn()
        {
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            if (user == null)
            {
                MainPage = new NavigationPage(new FirstPage());
            }
            //else if (user != null && !user.IsEmailVerified)
            //{
            //    MainPage = new NavigationPage(new VerifyEmailPage());
            //}
            else
            {
                MainPage = new NavigationPage(new HomePage());
            }
        }

        private static SQLiteHelper db;

        public static SQLiteHelper SQLiteDb { get { return db; } }
    }
}
