namespace Appel.SharpTemplate.Infrastructure
{
    public class AppSettings
    {
        public string SmtpHost { get; set; }
        public string SmtpPort { get; set; }
        public string EmailUser { get; set; }
        public string EmailPassword { get; set; }
        public string AuthTokenSecretKey { get; set; }
        public string EmailTokenSecretKey { get; set; }
    }
}
