using GodsEye.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace GodsEye.Infrastructure.Email
{
    public class MailKitEmailSender : IEmailService
    {
        private readonly SmtpSettings _settings;

        public MailKitEmailSender(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<string> LoadTemplateAsync(string templateName, Dictionary<string, string> values)
        {
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Email",
                "Templates",
                templateName
            );

            var html = await File.ReadAllTextAsync(path);

            foreach (var (key, value) in values)
            {
                html = html.Replace($"{{{{{key}}}}}", value);
            }

            return html;
        }

        public async Task SendAsync(IEnumerable<string> to, string subject, string htmlBody)
        {
            var message = new MimeMessage();

            message.From.Add(MailboxAddress.Parse(_settings.User));

            // 🔥 vários destinatários
            foreach (var email in to)
            {
                message.To.Add(MailboxAddress.Parse(email));
            }

            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.SslOnConnect
            );

            await client.AuthenticateAsync(
                _settings.User,
                _settings.Password
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
