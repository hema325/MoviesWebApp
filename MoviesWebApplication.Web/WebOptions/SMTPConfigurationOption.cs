namespace MoviesWebApplication.Web.WebOptions
{
    public class SMTPConfigurationOption
    {
        public const string SMTPConfiguration = "SMTPConfiguration";
        public string Host{get;set;}
        public int Port { get; set; }
        public string ServerMail { get; set; }
        public string Password { get; set; }
    }
}
