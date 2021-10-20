using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Client.Infrastructure.Enums;
using CleanArchitectureBase.Client.Models.Navigation;
using Microsoft.AspNetCore.Components;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Shared
{
    public partial class NavMenu
    {
        [Parameter]
        public bool ShowUserCard { get; set; } = true;

        [Parameter]
        public ExpandMode ExpandMode { get; set; }

        [Parameter]
        public HashSet<NavigationEntry> Entries { get; set; } = Navigations.Default;

        private ClaimsPrincipal _user;


        private bool IsAuthorized(NavigationEntry entry)
        {
            bool result = _authorizationService.HasPoliciesAsync(_user, entry.PolicyMatch, entry.Policies).GetAwaiter().GetResult();
            if (entry.HasChildren)
                return result && entry.Entries.Any(IsAuthorized);
            return result;
        }

        protected override async Task OnParametersSetAsync()
        {
            _user = await _stateProvider.GetAuthenticationStateProviderUserAsync();
            Entries.Recursive(n => n.Entries ?? Enumerable.Empty<NavigationEntry>()).Apply(e => e.IsExpanded = ExpandMode != ExpandMode.SingleExpand);
        }

        private void ToggleExpand(NavigationEntry entry)
        {
            if (ExpandMode != ExpandMode.None)
            {
                bool isExpanded = !entry.IsExpanded;
                if (ExpandMode == ExpandMode.SingleExpand)
                    Entries.Recursive(n => n.Entries ?? Enumerable.Empty<NavigationEntry>()).Apply(e => e.IsExpanded = false);

                entry.IsExpanded = isExpanded;
            }
        }

        private bool HasAction(NavigationEntry entry)
        {
            return !string.IsNullOrWhiteSpace(entry.Href);
        }

        private bool CanExpand(NavigationEntry context)
        {
            return context.HasChildren && ExpandMode != ExpandMode.None;
        }

        private string SubHeaderCls(NavigationEntry context)
        {
            return $"{(CanExpand(context) ? "cursor-pointer" : "")} mt-2 mb-n2";
        }
    }

}