using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneStore.Models
{
    public class RotatorModel
    {
        public RotatorModel()
        {

        }
        private string _image;
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }
    }
}
