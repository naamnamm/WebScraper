using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Topshelf;

namespace WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {

            List<Rental> rentalList = GetHTMLAsync();

            foreach (var item in rentalList)
            {
                Console.WriteLine(item);
            }

        }

        private static List<Rental> GetHTMLAsync()
        {
           // var url = "https://washingtondc.craigslist.org/search/apa?query=apartment+or+rent&search_distance=2&postal=20896&availabilityMode=0&sale_date=2022-05-04";
            var url = new URI("apartment", "1300", "1800", "0").ToString();

            Console.WriteLine(url);

            var httpClient = new HttpClient();
            var task = httpClient.GetStringAsync(url);

            var html = task.GetAwaiter().GetResult();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var rentalsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(n => n.GetAttributeValue("id", "")
                .Equals("search-results")).ToList();

            var rentalList = rentalsHtml[0].Descendants("li")
                .Where(n => n.GetAttributeValue("data-pid", "").StartsWith("7")).ToList();

            return parseHTMLNodeList(rentalList);

        }

        private static List<Rental> parseHTMLNodeList(List<HtmlNode> rentalList)
        {
            List<Rental> rentals = new List<Rental>();

           
            // loop through rentalList (from HTML), get info, then add it to a rentals
            foreach (var rental in rentalList)
            {
                var resultUrl = rental.Descendants("a").FirstOrDefault()?
                .GetAttributeValue("href", "") ??
                String.Empty;

                if (!resultUrl.Contains("washingtondc")) return rentals;

                var rentalID = rental.GetAttributeValue("data-pid", "");

                var rentalTitle = rental.Descendants("h3")
                .Where(n => n.GetAttributeValue("class", "").Equals("result-heading")).FirstOrDefault()?
                .InnerText.Trim() ??
                String.Empty;

                var rentalLocation = rental.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("result-hood")).FirstOrDefault()?
                .InnerText.Trim().Trim('(', ')') ??
                String.Empty;

                var rentalSquarefootage = rental.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("housing")).FirstOrDefault()?
                .InnerText.Replace("\n", String.Empty).Replace(" ", "") ??
                String.Empty;

                string trimmedRentalSqFt = rentalSquarefootage.Length > 0 ?
                    rentalSquarefootage.Remove(rentalSquarefootage.Length - 1) : String.Empty;

                var rentalPrice = rental.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("result-price")).FirstOrDefault()?
                .InnerText.Trim() ??
                String.Empty;                   

                rentals.Add(new Rental(rentalID, rentalTitle,  rentalLocation, trimmedRentalSqFt, rentalPrice, resultUrl));

            }

            return rentals;
        }
    }
}

/* information to extract

1. Title
2. Cost
3. Square footage (if applicable)
4. Location (if applicable)
5. A picture (if applicable)
6. Link to original listing

 */


/*
1. get a List of rentals
2. 
 */

//https://stackoverflow.com/questions/35885533/unable-to-get-child-categories-inside-ul-using-htmlagilitypack-c-sharp-asp-net


//Trim('\r', '\n', '\t') - doesn't work
//Replace("\n", String.Empty).Replace("\r", String.Empty) - doesn't work
//Replace("\n", String.Empty).Trim()
// create a list of rentals


//var rentalID = rentalList[0].GetAttributeValue("data-pid", "");

//var rentalTitle = rentalList[0].Descendants("h3")
//.Where(n => n.GetAttributeValue("class", "").Equals("result-heading")).FirstOrDefault().InnerText.Trim();

//var rentalLocation = rentalList[0].Descendants("span")
//.Where(n => n.GetAttributeValue("class", "").Equals("result-hood")).FirstOrDefault().InnerText.Trim();

//var rentalPrice = rentalList[0].Descendants("span")
//.Where(n => n.GetAttributeValue("class", "").Equals("result-price")).FirstOrDefault().InnerText;

//var rentalSquarefootage = rentalList[0].Descendants("span")
//.Where(n => n.GetAttributeValue("class", "").Equals("housing"))
//.FirstOrDefault()?.InnerText
//.Replace("\n", String.Empty)
//.Replace("\n", String.Empty).Trim() ?? String.Empty;

//rentals.Add(new Rental() { RentalID = rentalID, Title = rentalTitle, Location = rentalLocation, Price = rentalPrice, SquareFootage = rentalSquarefootage });

//Console.WriteLine(rentals.ToString());

//foreach (var rental in rentalList)
//{      
//    //if a href is not equal to <a href="https://washingtondc.craigslist - exit out of the loop
//    var resultUrl = rental.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
//    if (!resultUrl.Contains("washingtondc")) return;

//    //ID
//    Console.WriteLine(rental.GetAttributeValue("data-pid", ""));

//    //Title
//    Console.WriteLine(rental.Descendants("h3")
//    .Where(n => n.GetAttributeValue("class", "").Equals("result-heading")).FirstOrDefault().InnerText.Trim());

//    //Price
//    Console.WriteLine(rental.Descendants("span")
//    .Where(n => n.GetAttributeValue("class", "").Equals("result-price")).FirstOrDefault().InnerText);

//    //Square footage - if getattribute[housing] is null ? return string empty : else return the innertext value.
//    var squarefootage = rental.Descendants("span")
//    .Where(n => n.GetAttributeValue("class", "").Equals("housing")).FirstOrDefault()?.InnerText.Replace("\n", String.Empty).Replace("\n", String.Empty).Trim() ?? String.Empty;
//    Console.WriteLine(squarefootage);

//    //location
//    Console.WriteLine(rental.Descendants("span")
//    .Where(n => n.GetAttributeValue("class", "").Equals("result-hood")).FirstOrDefault().InnerText.Trim());

//    Console.WriteLine();

//}