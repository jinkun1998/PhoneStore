﻿using Acr.UserDialogs;
using PhoneStore.Firebase;
using PhoneStore.Models;
using PhoneStore.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class TabletViewModel
    {
        FirebaseHelper firebase;
        public TabletViewModel()
        {
            using (UserDialogs.Instance.Progress("Đang tải...", null, null, true, MaskType.Gradient))
            {
                firebase = new FirebaseHelper();
                var Items = Task.Run(async () => await firebase.GetAllItems()).Result;
                ItemCollection = Items.Where(it => it.Type == ItemModel.Category.Tablet).ToList();
                this.ItemTapped = new Command<object>(GotoDetail);
                this.BackButton = new Command(Back);
            }
        }

        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        
        private void GotoDetail(object obj)
        {
            var detail = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as ItemModel;
            Application.Current.MainPage.Navigation.PushAsync(new ItemDetailPage(detail));
        }

        private List<ItemModel> _itemCollection;
        public List<ItemModel> ItemCollection
        {
            get { return _itemCollection; }
            set { _itemCollection = value; }
        }

        private ObservableCollection<RotatorModel> _rotatorModels;
        public ObservableCollection<RotatorModel> RotatorModels
        {
            get { return _rotatorModels; }
            set { _rotatorModels = value; }
        }

        public Command<object> ItemTapped { get; }
        public Command BackButton { get; }
    }
}
