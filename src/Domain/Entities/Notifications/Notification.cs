using CleanArchitectureBase.Domain.Contracts;

namespace CleanArchitectureBase.Domain.Entities.Notifications;

public class Notification : AuditableEntity<int>
{
    public string UserId { get; set; }
    public string NotificationTypeId { get; set; }
    public string Subject { get; set; }
    public string Excerpt { get; set; }
    public string Content { get; set; }
    public string Url { get; set; }
    public bool IsRead { get; set; }
    public bool SentAsMail { get; set; }
}