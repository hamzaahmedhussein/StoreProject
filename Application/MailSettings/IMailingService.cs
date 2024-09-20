namespace Application.Settings
{
    public interface IMailingService
    {
        void SendMail(MailMessage message);

    }
}
