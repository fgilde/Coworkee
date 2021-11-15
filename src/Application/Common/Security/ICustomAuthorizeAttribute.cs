namespace CleanArchitectureBase.Application.Common.Security
{
    public interface ICustomAuthorizeAttribute
    {
        /// <summary>
        /// If more than one role is set you can specify if you need all ore one to get access
        /// </summary>
        RoleMatch RoleMatch { get; }

        /// <summary>
        /// If more than one policy is set you can specify if you need all ore one to get access
        /// </summary>
        PolicyMatch PolicyMatch { get; }

        /// <summary>
        /// Gets or sets a comma delimited list of roles that are allowed to access the resource.
        /// </summary>
        string[] Roles { get; }

        /// <summary>
        /// Gets or sets the policy name or a a comma delimited list of that determines access to the resource.
        /// </summary>
        string[] Policies { get; }
    }
}