using System.ComponentModel.DataAnnotations;

namespace CleanArchitectureBase.Application.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}