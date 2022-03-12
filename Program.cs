using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Xml;

namespace WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {

            GetHTMLAsync();

        }

        private static async void GetHTMLAsync()
        {
            var url = "https://washingtondc.craigslist.org/search/apa?query=apartment+or+rent&search_distance=2&postal=20896&availabilityMode=0&private_room=1&sale_date=all+dates";

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var rentalsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(n => n.GetAttributeValue("id", "")
                .Equals("curtain")).ToList();

            //var rentalList = rentalsHtml[0].Descendants();


            Console.WriteLine(rentalsHtml);

            //https://stackoverflow.com/questions/35885533/unable-to-get-child-categories-inside-ul-using-htmlagilitypack-c-sharp-asp-net
        }
    }
}
