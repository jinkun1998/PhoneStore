﻿using PhoneStore.Models;
using PhoneStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhoneStore.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShipmentPage : ContentPage
    {
        public ShipmentPage()
        {
            InitializeComponent();
            ShipmentViewModel vm = new ShipmentViewModel();
            this.BindingContext = vm;
        }
        public ShipmentPage(OrderModel order)
        {
            InitializeComponent();
            ShipmentViewModel vm = new ShipmentViewModel(order);
            this.BindingContext = vm;
        }
        
    }
}