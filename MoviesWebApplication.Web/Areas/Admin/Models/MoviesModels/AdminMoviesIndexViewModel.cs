namespace MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels
{
    public class AdminMoviesIndexViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public string MoviePosterUrl { get; set; }
        public double Rate { get; set; }
    }
}
