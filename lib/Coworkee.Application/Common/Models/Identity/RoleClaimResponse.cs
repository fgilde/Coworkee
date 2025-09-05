namespace lib.Coworkee.Application.Common.Models.Identity
{
    public class RoleClaimResponse
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public bool Selected { get; set; }
    }
}