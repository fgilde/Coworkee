using System.ComponentModel.DataAnnotations;

namespace lib.Coworkee.Application.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}