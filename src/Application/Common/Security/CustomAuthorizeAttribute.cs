using System;

namespace CleanArchitectureBase.Application.Common.Security
{
    /// <summary>
    /// Specifies the class this attribute is applied to requires authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute, ICustomAuthorizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAuthorizeAttribute"/> class. 
        /// </summary>
        public CustomAuthorizeAttribute() { }

        /// <summary>
        /// If more than one role is set you can specify if you need all ore one to get access
        /// </summary>
        public RoleMatch RoleMatch { get; set; }

        /// <summary>
        /// If more than one policy is set you can specify if you need all ore one to get access
        /// </summary>
        public PolicyMatch PolicyMatch { get; set; }

        /// <summary>
        /// Gets or sets a comma delimited list of roles that are allowed to access the resource.
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// Gets or sets the policy name or a a comma delimited list of that determines access to the resource.
        /// </summary>
        public string[] Policies { get; set; }
    }


}
