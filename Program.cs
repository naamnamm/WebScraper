using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

            //string url = "https://apps.vandapharma.com/pdfjs/web/viewer.html?file=/attachments/156386?style=pdf";

            //string url2 = @"C:\Users\naamp\Downloads\Payment-Receipt-04-13-2022.pdf";
            
            //string url3 = @"https://mail-attachment.googleusercontent.com/attachment/u/0/?ui=2&ik=a982987066&attid=0.1&permmsgid=msg-a:r4254690538152362018&th=1806b0d17c2cbfef&view=att&disp=inline&realattid=f_l2hkjdib0&saddbat=ANGjdJ-TAeUcKJE_Xqro8fpYxexK5oP2aqcxPSAviKeRJSLYEsy8bP_qwCmZo0Y0CmszM0H8Yix2AvBW78NfTDP8lOv48dkxRYA7dWsw3kd4y9sL1_JGSS359o-bq7VWft2oq5PSaOKWeyc4iPAlDRdDMDDTpy9P49M3UbODUJMkWcQ5HNXqc_Hg4XJfooQ1zXuhwV26UC6G8fPWi_PAnuMar7hAaSfSCbqy_GUZudbQzSMHNuN6HuI0CYn1SZtpGB9r2WxUKOcGgnWkIoZYrcd--5fJEfhzTNBJhaAOAn_0b967fOku5917C7ya5a_K3cK4kJ_jkWrXXTXuQqSKV3jfcNkHabnMgRY-L8YELC5KDna-8dESGCGPZOig4oyeeCLedVb5Wt0H1xsMSdHBV3W6ETs8QtKkI7mjFeUVrvqysBiYDOezw07uIY94DWAz51UrN9nJVKEaClDR9ytSy1nYq9BzXfiYIkpdn1bDFwCm9Hg1gEuSXv85F93cUrbsL_addQDN_Kros1e-LqApk7lFjAXBpPXZ0Of6IHzDE0rqP-wbYGG_i1NCKkD-81RL6m-HJcfzL3yB0QaIfda7upEh8TmlXiRmZtw-C6pE_z9okOKAiYzztfR02Dr7YnAaOv1RVXuorNAERezsO9InVtB4Upv_fFfOcN5tu4-lsMPUBDvmRj_1hNkn31Y7FS5OMZUtqHY5tgJlJP0LiC8b0MH96PU9jl3braVCgai4-7Z2NlUQo-3OB8nHrGoL6kjHJyMQio-9PlMt0BUwg1pgt4T4giyvaJEnpGjoZo5OQw";
            
            //string url4 = @"https://drmindypelz.com/wp-content/uploads/2019/12/Muscle-Building-Reset.pdf";

            //string url5 = @"https://apps.vandapharma.com/attachments/202788?style";
            

            //string clientfile = @"C:\Users\naamp\source\repos\WebScraper_Work\bin\Debug\netcoreapp3.1\files\newfile.pdf";

            //WebClient wc = new WebClient();

            //wc.DownloadFile(new Uri(url5, UriKind.Absolute), clientfile);

            List<Case> rentalList = GetHTMLAsync();

            foreach (var item in rentalList)
            {
                Console.WriteLine(item);
            }

        }

        private static List<Case> GetHTMLAsync()
        {
            string path = @"C:\Users\naamp\source\repos\HTML_Scraper\Case ID_ 7021870 _ Jupiter.html";

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(path);

            var rentalsHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "")
                .Equals("DocumentViewer__sidebar")).ToList();

            var rentalList = rentalsHtml[0].Descendants("div")
                .Where(n => n.GetAttributeValue("class", "")
                .Contains("DocumentThumbnail__container")).ToList();

            foreach (var item in rentalList)
            {
                Console.WriteLine(item);
            }

            return parseHTMLNodeList(rentalList);

        }

        private static List<Case> parseHTMLNodeList(List<HtmlNode> rentalList)
        {
            List<Case> rentals = new List<Case>();

            Regex regex = new Regex(@"\b\d{6}\b");

            foreach (var rental in rentalList)
            {
                string attachmentID = rental.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");

                Match match = regex.Match(attachmentID);
                if (match.Success)
                {
                    attachmentID = match.ToString();
                };

                string title = rental.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "")
                .Equals("DocumentListItem__title"))
                .FirstOrDefault().InnerText
                .Replace("/", "-");

                // replace / with - for the date string


                string downloadLink = String.Concat("https://apps.vandapharma.com/attachments/", attachmentID, "?style");

                rentals.Add(new Case(attachmentID, title, downloadLink));

                //method 1 - file path
                string _filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + $"\\files\\{title}.pdf";

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(downloadLink, _filePath);

                    //method 2 - file path
                    Path.GetFullPath($"{Environment.CurrentDirectory}\\files\\{title}.pdf");
                    
                }

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