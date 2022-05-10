using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraper
{
    public class URI
    {
        const string baseURI = "https://washingtondc.craigslist.org/search/hhh?";
        
        public URI(string query, string min_price, string max_price, string availabilityMode)
        {
            Query = query;
            Min_price = min_price;
            Max_price = max_price;
            AvailabilityMode = availabilityMode;
        }

        public string Query { get; set; }
        public string Min_price { get; set; }
        public string Max_price { get; set; }
        public string AvailabilityMode { get; set; }

        public override string ToString()
        {
            var formatString = $"{baseURI}" +
                $"query={Query}" +
                $"&min_price={Min_price}" +
                $"&max_price={Max_price}" +
                $"&availabilityMode={AvailabilityMode}" +
                $"&sale_date=all+dates";

            return formatString;
        }
    }
}
