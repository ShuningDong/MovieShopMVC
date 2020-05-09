using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Core.ApiModels.Response
{
   public class MovieResponseModel
    {
        // we are gonna use this class to send Movie Info to Angular that will use this class data to display Movie Card

        public int Id { get; set; }
        public string Title { get; set; }
        public string PosterUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
