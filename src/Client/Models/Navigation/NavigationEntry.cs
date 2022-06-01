using System.Collections.Generic;
using System.Linq;
using CleanArchitectureBase.Application.Common.Security;
using Nextended.Core.Extensions;
using Nextended.Core.Types;

namespace CleanArchitectureBase.Client.Models.Navigation
{
    public class NavigationEntry : Hierarchical<NavigationEntry>
    {
        public NavigationEntry(string text = "", string icon = "", string href = "", string target = "")
        {
            Icon = icon;
            Text = text;
            Href = href;
            Target = target;
        }
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Href { get; set; }
        public string Target { get; set; }
        public string[] Policies { get; set; }
        public PolicyMatch PolicyMatch { get; set; }
        public string[] Roles { get; set; }
        public RoleMatch RoleMatch { get; set; }
        public bool IsAuthenticationRequired { get; set; }

        public NavigationEntry WithAuthentication()
        {
            IsAuthenticationRequired = true;
            return this;
        }

        public NavigationEntry WithPolicies(PolicyMatch match, params string[] policies)
        {
            PolicyMatch = match;
            return WithPolicies(policies);
        }

        public NavigationEntry WithPolicies(params string[] policies)
        {
            Policies = policies;
            return this;
        }

        public NavigationEntry WithRoles(RoleMatch match, params string[] roles)
        {
            RoleMatch = match;
            return WithRoles(roles);
        }

        public NavigationEntry WithRoles(params string[] roles)
        {
            Roles = roles;
            return this;
        }

    }
}