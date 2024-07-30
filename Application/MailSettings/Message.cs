using MimeKit;

namespace Application.Settings
{
    public class MailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public MailMessage(IEnumerable<string> to, string subject, string content)
        {
            To = to.Select(x => new MailboxAddress("email", x)).ToList();
            Subject = subject;
            Content = content;
        }
    }
}
