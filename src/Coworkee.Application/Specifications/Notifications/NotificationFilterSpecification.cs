using Coworkee.Application.Features.Notifications.Queries.GetAllPaged;
using Coworkee.Application.Specifications.Base;
using Coworkee.Domain.Entities.Notifications;

namespace Coworkee.Application.Specifications.Notifications
{
    public class NotificationFilterSpecification : SpecificationBase<Notification>
    {
        public NotificationFilterSpecification(string searchString, string userId, GetAllNotificationsQuery query)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.UserId == userId
                                && (string.IsNullOrEmpty(query.NotificationTypeId) || p.NotificationTypeId == query.NotificationTypeId)
                                && (!query.UnreadOnly || !p.IsRead) &&
                                p.Content != null && p.Content.Contains(searchString) 
                                || p.Subject != null && p.Subject.Contains(searchString)
                                || p.Url != null && p.Url.Contains(searchString);
            }
            else
            {
                Criteria = p => p.UserId == userId && (string.IsNullOrEmpty(query.NotificationTypeId) || p.NotificationTypeId == query.NotificationTypeId) && (!query.UnreadOnly || !p.IsRead);
            }
        }
    }
}