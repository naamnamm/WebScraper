using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraper
{
    public class Rental
    {

        public string RentalID { get; set; }

        public string Title { get; set; }
        public string Location { get; set; }
        public string SquareFootage { get; set; }
        public string Price { get; set; }

        public override string ToString()
        {
            var formatString = $"ID: {RentalID} \n" +
                $"Title: {Title} \n" +
                $"Location: {Location} \n" +
                $"SquareFootage: {SquareFootage} \n" +
                $"Price: {Price} \n";

            return formatString;
        }
    }
}
