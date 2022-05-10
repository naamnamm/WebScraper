using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraper
{
    public class Rental
    {
        public Rental(string rentalID, string title, string location, string squareFootage, string price, string link)
        {
            RentalID = rentalID;
            Title = title;
            Location = location;
            SquareFootage = squareFootage;
            Price = price;
            Link = link;
        }

        public string RentalID { get; set; }

        public string Title { get; set; }
        public string Location { get; set; }
        public string SquareFootage { get; set; }

        public string Price { get; set; }
        public string Link { get; set; }

        public override string ToString()
        {
            var formatString = $"ID: {RentalID} \n" +
                $"Title: {Title} \n" +
                $"Location: {Location} \n" +
                $"SquareFootage: {SquareFootage} \n" +
                $"Price: {Price} \n" +
                $"Link: {Link} \n";

            return formatString;
        }
    }
}
