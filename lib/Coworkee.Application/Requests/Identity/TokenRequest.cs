using System.ComponentModel.DataAnnotations;

namespace lib.Coworkee.Application.Requests.Identity
{
    public class TokenRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}