using MoviesWebApplication.DAL.Data;

namespace MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels
{
    public class DirectorsIncludeMoviesDirectorsViewModel
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public string ImgUrl { get; set; }
        public int DirectorId { get; set; }
        public int MovieId { get; set; }
    }
}
