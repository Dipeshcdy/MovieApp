namespace MovieApp.Models.Email
{
    public class Email
    {
        public string SecurityKey { get; set; }
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
    }
}
