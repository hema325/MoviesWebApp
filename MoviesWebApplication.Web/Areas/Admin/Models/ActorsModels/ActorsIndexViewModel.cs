using MoviesWebApplication.DAL.Data;

namespace MoviesWebApplication.Web.Areas.Admin.Models.ActorsModels
{
    public class ActorsIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public string ImgUrl { get; set; }
    }
}
