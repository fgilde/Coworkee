namespace lib.Coworkee.Application.Requests.Identity
{
    public class RoleClaimRequest
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