namespace MoviesWebApplication.DAL.Data
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public bool IsBlocked { get; set; }
        public string ImgUrl { get; set; }
        public Role Role { get; set; }

    }
}