using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.Data
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseYear { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string MovieUrl { get; set; }
        public string MoviePosterUrl { get; set; }
        
        public double Rate { get; set; }

    }
}
