namespace GodsEye.Application.Interfaces
{
    public interface IEmailService
    {
        Task<string> LoadTemplateAsync(string templateName, Dictionary<string, string> values);
        Task SendAsync(IEnumerable<string> to, string subject, string htmlBody);
    }
}
