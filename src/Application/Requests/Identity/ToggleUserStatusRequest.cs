namespace Coworkee.Application.Requests.Identity
{
    public class ToggleUserStatusRequest
    {
        public bool ActivateUser { get; set; }
        public bool EmailConfirmed { get; set; }
        public string UserId { get; set; }
    }
}