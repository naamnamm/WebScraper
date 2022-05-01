using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraper
{
    public class Case
    {
        public Case(string attachmentID, string title, string downloadLink)
        {
            AttachmentID = attachmentID;
            Title = title;
            DownloadLink = downloadLink;
        }


        public string DownloadLink { get; set; }
        public string AttachmentID { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            var formatString = $"AttachmentID: {AttachmentID} \n" +
                $"Title: {Title} \n" + 
                $"DownloadLink: {DownloadLink} \n" ;

            return formatString;
        }
    }
}
