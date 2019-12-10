using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PhoneStore.Models
{
    public class CartModel : INotifyPropertyChanged
    {
        [PrimaryKey]
        public string CartCode { get; set; }
        public string Code { get; set; }

        public string Image { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Shortdescription { get; set; }

        public string Description { get; set; }

        public string DescriptionLink { get; set; }

        public bool InOrder { get; set; }

        public string UserEmail { get; set; }

        private string _rate;
        public string Rate
        {
            get { return _rate; }
            set { _rate = value; OnPropertyChanged(); }
        }
        public string Color { get; set; }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
