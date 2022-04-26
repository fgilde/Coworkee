using System;
using CleanArchitectureBase.Application.Requests;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Application.Common.Models
{
    public class NotificationDto : DtoBase<int>
    {
        public string Subject { get; set; }
        public string NotificationTypeId { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public bool IsRead { get; set; }
        public bool SentAsMail { get; set; }
        public bool Expanded { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}