using System;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Hubs.Events.Base;
using Coworkee.Application.Requests.Mail;
using Coworkee.Shared.Constants.Application;
using Nextended.Core.Extensions;

namespace Coworkee.Application.Requests;

public class NotificationRequest: ClientEventBase
{
    public string NotificationTypeId { get; [Obsolete("Public setter is required for serializer but you should not use it")] set; } = Guid.NewGuid().ToString(); // Public setter is required for serializer but you should not use it
    public NotificationAsMail SendAsMail { get; set; }
    public bool SkipCurrentUser { get; set; }

    /// <summary>
    /// If true notification will stored in Database, otherwise its one time only
    /// </summary>
    public bool PersistInDb { get; set; }
    public string Subject { get; set; }
    public string Excerpt { get; set; }
    public string Content { get; set; }
    public string Url { get; set; }
    public string SenderEmailAddress { get; set; }
    public string SenderName { get; set; }

    public NotificationDto ToDto()
    {
        return this.MapTo<NotificationDto>().SetProperties(d => d.CreatedOn = DateTime.Now);
    }

    public MailRequest ToMailRequest(UserResponse user)
    {
        return new MailRequest
        {
            RecipientName = $"{user.FirstName} {user.LastName}",
            To = user.Email,
            Body = Content,
            Subject = Subject,
            From = SenderEmailAddress,
            SenderName = SenderName
        };
    }
}

public enum NotificationAsMail
{
    Always,
    Never,
    WhenTargetOffline
}