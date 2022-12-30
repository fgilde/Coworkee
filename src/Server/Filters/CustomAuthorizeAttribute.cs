using System;
using Coworkee.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace Coworkee.Server.Filters
{
    public class CustomAuthorizeAttribute: TypeFilterAttribute, ICustomAuthorizeAttribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAuthorizeAttribute"/> class. 
        /// </summary>
        public CustomAuthorizeAttribute() : base(typeof(CustomAuthorizeAttributeFilter))
        {
            Arguments = new object[] {this};
        }
        

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