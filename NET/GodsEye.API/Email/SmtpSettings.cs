namespace GodsEye.API.Email
{
    public class SmtpSettings
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; }
        public string User { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool EnableSsl { get; set; }
    }
}
