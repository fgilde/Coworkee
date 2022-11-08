namespace CleanArchitectureBase.Application.Requests.Mail
{
    public class MailRequest
    {
        public string RecipientName { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string SenderName { get; set; }
    }
}