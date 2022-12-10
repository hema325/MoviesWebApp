using MoviesWebApplication.DAL.Data;

namespace MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels
{
    public class ReviewersIncludeMoviesReviewersViewModel
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public string ImgUrl { get; set; }
        public int ReviewerId { get; set; }
        public int MovieId { get; set; }
        public int Stars { get; set; }
    }
}
