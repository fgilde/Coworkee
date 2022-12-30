using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Features.Notifications.Commands.AddEdit;
using Coworkee.Application.Features.Notifications.Commands.Delete;
using Coworkee.Application.Features.Notifications.Commands.MarkAll;
using Coworkee.Application.Features.Notifications.Queries;
using Coworkee.Application.Features.Notifications.Queries.GetAllPaged;
using Coworkee.Application.Features.Notifications.Queries.GetById;
using Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coworkee.Server.Controllers.Notifications
{
    public class NotificationsController : BaseApiController<NotificationsController>
    {
        /// <summary>
        /// Get All Notifications
        /// </summary>
        /// <returns>Status 200 OK</returns>
        // [Authorize(Policy = Permissions.Products.View)]
        [Authorize]
        [HttpGet]
        [Produces(typeof(PaginatedResult<NotificationDto>))]
        public async Task<IActionResult> GetAll([FromQuery] GetAllNotificationsQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        /// <summary>
        /// Returns count of unread notifications
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize]
        [HttpGet(nameof(Unread))]
        [Produces(typeof(int))]
        public async Task<IActionResult> Unread(CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new GetUnreadCount.Request(), cancellationToken));
        }

        /// <summary>
        /// Deletes given notifications if they are for current user
        /// </summary>
        /// <param name="ids">Notifications to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK response</returns>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(string[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteNotificationsCommand { Ids = UnhashIds(ids) }, cancellationToken));
        }

        /// <summary>
        /// Deletes given notifications if they are for current user
        /// </summary>
        /// <param name="ids">Notifications to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK response</returns>
        [Authorize]
        [HttpDelete(nameof(DeleteAll))]
        public async Task<IActionResult> DeleteAll(CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteNotificationsCommand { All = true }, cancellationToken));
        }

        /// <summary>
        /// Gets a Specific Notification by an Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize]
        [HttpGet("{id}")]
        [Produces(typeof(NotificationDto))]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetNotificationByIdQuery(UnhashId(id)), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Marks all users notifications as read or unread
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isRead">read status</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize]
        [HttpPost(nameof(MarkAllRead))]
        public async Task<IActionResult> MarkAllRead(bool isRead, CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new MarkAllNotificationsCommand.Request {IsRead = isRead}, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Marks a notification as read or unread
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isRead">read status</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize]
        [HttpPost("{id}")]
        [Produces(typeof(NotificationDto))]
        public async Task<IActionResult> MarkRead(string id, bool isRead, CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetNotificationByIdQuery(UnhashId(id)), cancellationToken);
            result.IsRead = isRead;
            await Mediator.Send(new AddEditNotificationCommand(result), cancellationToken);
            return Ok(result);
        }

    }
}