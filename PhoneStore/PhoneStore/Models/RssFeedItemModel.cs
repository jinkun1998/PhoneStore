using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PhoneStore.Models
{
    public class RssFeedItemModel
    {
        public string title { get; set; }
        public string image { get; set; }
        public HtmlWebViewSource desHtml { get; set; }
        public string link { get; set; }
        public string pubDate { get; set; }
    }
}
