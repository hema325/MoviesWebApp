using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Models.MoviesModels
{
    public class WatchMovieViewModel
    {
        public string Title { get; set; }
        [Display(Name = "Release Year")]
        public DateTime ReleaseYear { get; set; }
        public TimeSpan Duration { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string MovieUrl { get; set; }
        public string MoviePosterUrl { get; set; }
        public double Rate { get; set; }
        public IEnumerable<string> Generes { get; set; }
        public IEnumerable<MovieActorsViewModel> Actors { get; set; }
        public IEnumerable<MovieDirectorsViewModel> Directors { get; set; }
    }
}
