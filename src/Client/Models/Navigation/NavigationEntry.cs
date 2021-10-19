using System.Collections.Generic;
using System.Linq;
using CleanArchitectureBase.Application.Security;

namespace CleanArchitectureBase.Client.Models.Navigation
{
    public class NavigationEntry
    {
        public NavigationEntry(string text = "", string icon = "", string href = "", string target = "")
        {
            Icon = icon;
            Text = text;
            Href = href;
            Target = target;
        }

        public string Icon { get; set; }
        public string Text { get; set; }
        public string Href { get; set; }
        public string Target { get; set; }
        public string[] Policies { get; set; }
        public PolicyMatch PolicyMatch { get; set; }
        public HashSet<NavigationEntry> Entries { get; set; }
        public bool IsExpanded { get; set; }
        public bool HasChildren => Entries?.Any() == true;

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
    }
}