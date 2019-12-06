using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhoneStore.View.ListViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneView : ContentView
    {
        public PhoneView()
        {
            InitializeComponent();
        }
    }
}