using Acr.UserDialogs;
using Microsoft.Toolkit.Parsers.Rss;
using PhoneStore.Models;
using PhoneStore.View.MainViews.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace PhoneStore.ViewModels
{
    public class RssFeedViewModel
    {
        public RssFeedViewModel(RssFeedModel feed)
        {
            using (UserDialogs.Instance.Progress("Đang tải...", null, null, true, MaskType.Gradient))
            {
                this.Feed = feed;
                RSSNews = ParseXML(feed);

                this.FeedLoadTapped = new Command(LoadFeed);
                this.BackButton = new Command(Back);
            }
        }
        
        private async void Back(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void LoadFeed(object obj)
        {
            using (UserDialogs.Instance.Progress("Đang tải.."))
            {
                var detail = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as RssFeedItemModel;
                await Application.Current.MainPage.Navigation.PushAsync(new ReadNewsPage(detail.link));
            }
        }

        private List<RssFeedItemModel> ParseXML(RssFeedModel feed)
        {
            List<RssFeedItemModel> rssItems = new List<RssFeedItemModel>();
            using (var webClient = new WebClient())
                try
                {
                    var xmlUrl = new Uri(feed.link);
                    string result = webClient.DownloadString(xmlUrl);
                    XDocument doc = XDocument.Parse(result);
                    var temp = ((from u in doc.Descendants("item")
                                 select new RssFeedItemModel()
                                 {
                                     title = u.Element("title").Value,
                                     image = feed.image,
                                     desHtml = new HtmlWebViewSource
                                     {
                                         Html = u.Element("description").Value,
                                     },
                                     link = u.Element("link").Value.ToString(),
                                     pubDate = u.Element("pubDate").Value,
                                 }).ToList());
                    rssItems.AddRange(temp);
                }
                catch (Exception ex)
                {

                }
            return rssItems;
        }

        public RssFeedModel Feed { get; set; }
        public List<RssFeedItemModel> RSSNews { get; set; }
        public Command FeedLoadTapped { get; }
        public Command BackButton { get; }
    }
}
