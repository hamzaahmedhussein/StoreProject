namespace Application.Settings
{
    public interface IMailingService
    {
        Task SendMailAsync(MailMessage message);

    }
}
