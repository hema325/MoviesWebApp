namespace MoviesWebApplication.Web.DALOptions
{
    public class ConnectionStringsOption
    {
        public const string ConnectionStrings = "ConnectionStrings";
        public string DefaultConnection { get; set; }
        public string ConnectionWithoutDataBase { get; set; }
    }
}
