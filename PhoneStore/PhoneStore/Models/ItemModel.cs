using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PhoneStore.Models
{
    public class ItemModel : INotifyPropertyChanged
    {
        public ItemModel()
        {

        }

        private string _code;
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _image;
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private string _rate;
        public string Rate
        {
            get { return _rate; }
            set { _rate = value; OnPropertyChanged(); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private decimal _price;
        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }

        private string _shortdescription;
        public string Shortdescription
        {
            get { return _shortdescription; }
            set { _shortdescription = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _descriptionlink;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DescriptionLink
        {
            get { return _descriptionlink; }
            set { _descriptionlink = value; }
        }

        public List<ColorModel> Colors { get; set; }
        public ObservableCollection<RotatorModel> RotatorImages { get; set; }
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }
    }
}
