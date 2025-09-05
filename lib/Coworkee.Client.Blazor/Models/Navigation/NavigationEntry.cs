using lib.Coworkee.Application.Common.Security;
using Nextended.Core.Types;
using System;
using System.Security.Claims;

namespace lib.Coworkee.Client.Models.Navigation
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

        public NavigationEntry()
        { }

        public string Text { get; set; }
        public string Icon { get; set; }
        public string Href { get; set; }
        public string Target { get; set; }
        public string[] Policies { get; set; }
        public PolicyMatch PolicyMatch { get; set; }
        public string[] Roles { get; set; }
        public RoleMatch RoleMatch { get; set; }
        public bool IsAuthenticationRequired { get; set; }
        public Func<ClaimsPrincipal?, bool> Condition { get; set; }

        public bool MatchesConditions(ClaimsPrincipal? claimsPrincipal) => Condition == null || Condition(claimsPrincipal);

        public NavigationEntry WithAuthentication()
        {
            IsAuthenticationRequired = true;
            return this;
        }

        public NavigationEntry WithCondition(bool condition) => WithCondition((_) => condition);

        public NavigationEntry WithCondition(Func<ClaimsPrincipal?, bool> condition)
        {
            Condition = Condition != null ? claimsPrincipal => Condition(claimsPrincipal) && condition(claimsPrincipal) : condition;
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
        public override string ToString()
        {
            return Text;
        }
    }
}