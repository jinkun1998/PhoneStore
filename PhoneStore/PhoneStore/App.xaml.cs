
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
            CheckUserSignIn();
            //MainPage = new NavigationPage(new ShipmentPage());
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

        public void CheckUserSignIn()
        {
            if (db == null)
            {
                db = new SQLiteHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhoneStore.db3"));
            }
            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            if (user == null)
            {
                MainPage = new NavigationPage(new FirstPage());
            }
            else
            {
                MainPage = new NavigationPage(new HomePage());
            }
        }

        private static SQLiteHelper db;

        public static SQLiteHelper SQLiteDb { get { return db; } }
    }
}
