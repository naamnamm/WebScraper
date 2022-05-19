using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Topshelf;

namespace WebScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false);

            IConfiguration config = builder.Build();

            var mail = config.GetSection("Mail").Get<Mail>();

            List<Rental> rentalList = GetHTMLAsync();

            await sendEmail(rentalList, mail);

        }

        private static async Task sendEmail(List<Rental> rentalList, Mail mail)
        {
            try
            {
                var sender = new SmtpSender(() => new SmtpClient()
                {
                    //send to papercut
                    //for testing - set this to false
                    //EnableSsl = false,
                    //DeliveryMethod = SmtpDeliveryMethod.Network,
                    //Host = "localhost",
                    //Port = 25,

                    //sender pondpattohs@gmail.com connect to gmail server
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    Host = "smtp.mail.yahoo.com",
                    Port = 587,
                    Credentials = new NetworkCredential(mail.Username, mail.Password)

                });


                var template = @"  
                    Hey @Model.Name, here is your daily rental list. 
                    @for(var i = 1; i < @Model.RentalList.Count; i++) 
                    {                        
                        <p>
                            <b>Item:</b> @i
                            <br>
                            <b>Title:</b> @Model.RentalList[i].Title 
                            <br>
                            <b>Location:</b> @Model.RentalList[i].Location 
                            <br>
                            <b>SquareFootage:</b> @Model.RentalList[i].SquareFootage 
                            <br>
                            <b>Price:</b> @Model.RentalList[i].Price
                            <br>
                            <b>Link:</b> @Model.RentalList[i].Link
                        </p>
                    }

                ";

                var model = new { Name = "Naam", RentalList = rentalList };

                Email.DefaultSender = sender;
                Email.DefaultRenderer = new RazorRenderer();

                var email = await Email
                    .From("naam.namm@yahoo.com")
                    .To("naam.pt@gmail.com", "Naam")
                    .Subject("Daily Rental List")
                    .UsingTemplate(template, model)
                    .SendAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<Rental> GetHTMLAsync()
        {      
            var url = new URI("apartment", "1300", "1800", "0").ToString();

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

// var url = "https://washingtondc.craigslist.org/search/apa?query=apartment+or+rent&search_distance=2&postal=20896&availabilityMode=0&sale_date=2022-05-04";

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

/*<p>Title: @item.Title</p>
                        <p>Location: @item.Location</p>
                        <p>SquareFootage: @item.SquareFootage</p>
                        <p>Link: @item.Link</p>*/



//List<Rental> rentalList = GetHTMLAsync();

//foreach (var item in rentalList)
//{
//    Console.WriteLine(item);
//}

//try
//{
//    // create a server
//    var sender = new SmtpSender(() => new SmtpClient()
//    {
//        //send to papercut
//        //for testing - set this to false
//        //EnableSsl = false,
//        //DeliveryMethod = SmtpDeliveryMethod.Network,
//        //Host = "localhost",
//        //Port = 25,

//        //sender pondpattohs@gmail.com connect to gmail server
//        DeliveryMethod = SmtpDeliveryMethod.Network,
//        UseDefaultCredentials = false,
//        EnableSsl = true,
//        Host = "smtp.gmail.com",
//        Port = 587,
//        //need to change email to actual email and password
//        //Q: can I use other server to send email to my account
//        //because I don't want to put in my credentials in here
//        //and I don't want to set up my gmail account to be less secure
//        Credentials = new NetworkCredential("email", "password")

//    });


//    var template = @"  
//                    Hey @Model.Name, here is your daily rental list. 
//                    @for(var i = 1; i < @Model.RentalList.Count; i++) 
//                    {                        
//                        <p>
//                            <b>Item:</b> @i
//                            <br>
//                            <b>Title:</b> @Model.RentalList[i].Title 
//                            <br>
//                            <b>Location:</b> @Model.RentalList[i].Location 
//                            <br>
//                            <b>SquareFootage:</b> @Model.RentalList[i].SquareFootage 
//                            <br>
//                            <b>Price:</b> @Model.RentalList[i].Price
//                            <br>
//                            <b>Link:</b> @Model.RentalList[i].Link
//                        </p>
//                    }

//                ";

//    var model = new { Name = "Naam", RentalList = rentalList };

//    Email.DefaultSender = sender;
//    Email.DefaultRenderer = new RazorRenderer();

//    var email = await Email
//        .From("pondpattohs@gmail.com")
//        .To("naam.pt@gmail.com", "Naam")
//        .Subject("Daily Rental List")
//        .UsingTemplate(template, model)
//        .SendAsync();

//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex);
//    throw;
//}