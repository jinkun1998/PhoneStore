using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneStore.Models
{
    public class RssFeedModel
    {
        public RssFeedModel(string Name, string Link, string Image)
        {
            name = Name;
            link = Link;
            image = Image;
        }
        public string link { get; set; }
        public string image { get; set; }
        public string name { get; set; }
    }
}
