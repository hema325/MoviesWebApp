namespace MoviesWebApplication.Web.Areas.Admin.Models.UsersController
{
    public class UsersDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsBlocked { get; set; }
        public string ImgUrl { get; set; }
        public string Role { get; set; }
    }
}
