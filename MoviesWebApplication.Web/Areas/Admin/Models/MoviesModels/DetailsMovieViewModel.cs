using MoviesWebApplication.Web.Areas.Admin.Models.ActorsModels;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels
{
    public class DetailsMovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [Display(Name = "Release Year")]
        public DateTime ReleaseYear { get; set; }
        public TimeSpan Duration { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string MovieUrl { get; set; }
        public string MoviePosterUrl { get; set; }
        public double Rate { get; set; }
        public IEnumerable<GeneresIncludeMoviesGeneresViewModel> Generes { get; set; }
        public IEnumerable<ActorsIncludeMoviesActorsViewModel> Actors{get;set;}
        public IEnumerable<DirectorsIncludeMoviesDirectorsViewModel> Directors { get; set; }
        public IEnumerable<ReviewersIncludeMoviesReviewersViewModel> Reviewers { get; set; }
    }
}
