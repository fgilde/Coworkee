using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Client.Infrastructure.Enums;
using CleanArchitectureBase.Client.Models;
using CleanArchitectureBase.Client.Models.Navigation;
using Microsoft.AspNetCore.Components;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Shared
{
    public partial class NavMenu
    {
        private ClaimsPrincipal _user;
        private ExpandMode _expandMode;

        [Parameter] public bool ShowUserCard { get; set; } = true;     
        
        [Parameter] public bool ShowApplicationLogo { get; set; } = false;

        [Parameter]
        public ExpandMode ExpandMode
        {
            get => _expandMode;
            set
            {
                if (value != _expandMode)
                {
                    _expandMode = value;
                    SetAllExpanded(ExpandMode != ExpandMode.SingleExpand);
                }
            }
        }

        [Parameter] public HashSet<NavigationEntry> Entries { get; set; } 
        
        [Parameter] public EventCallback Logout { get; set; }
        
        private bool IsAuthorized(NavigationEntry entry)
        {
            bool result = _authorizationService.HasPoliciesAsync(_user, entry.PolicyMatch, entry.Policies).GetAwaiter().GetResult();
            if (entry.HasChildren)
                return result && entry.Children.Any(IsAuthorized);
            return result;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                SetAllExpanded(ExpandMode != ExpandMode.SingleExpand);
            return base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            Entries ??= Navigations.Default(_config.BackendOrigin);
            _user = await _stateProvider.GetAuthenticationStateProviderUserAsync();
            ExpandToCurrentUrl();
        }

        private void ExpandToCurrentUrl()
        {
            var url = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
            if (ExpandMode != ExpandMode.None)
            {
                if (!string.IsNullOrWhiteSpace(url) && url != "/")
                {
                    FindEntriesForUrl(url)
                        .SelectMany(e => e.Path)
                        .Apply(e => e.IsExpanded = true);
                }
            }
        }

        public string Locale(string s)
        {
            return _localizer != null ? _localizer[s??""] : s??"";
        }

        public IEnumerable<NavigationEntry> FindEntriesForUrl(string url = null)
        {
            url = (url ?? _navigationManager.ToBaseRelativePath(_navigationManager.Uri)).EnsureStartsWith("/").ToLower();
            return Entries.Find(e => e.Href.EnsureStartsWith("/").ToLower() == url);
        }

        private void OnExpandCollapseClick(NavigationEntry entry)
        {
            if (ExpandMode != ExpandMode.None)
            {
                var state = !entry.IsExpanded;
                if (ExpandMode == ExpandMode.SingleExpand)
                    SetAllExpanded(false, e => e != entry && !e.ContainsChild(entry));
                entry.IsExpanded = state;
            }
        }

        private void SetAllExpanded(bool expand, Func<NavigationEntry, bool> predicate = null)
        {
            predicate ??= n => ExpandMode == ExpandMode.SingleExpand || n.Parent == null;
            Entries.Recursive(n => n.Children.EmptyIfNull()).Where(predicate).Apply(e => e.IsExpanded = expand);
        }


        private bool HasAction(NavigationEntry entry)
        {
            return !string.IsNullOrWhiteSpace(entry.Href);
        }

        private bool CanExpand(NavigationEntry context)
        {
            return context.HasChildren && ExpandMode != ExpandMode.None && (context.Parent == null || context.Parent.IsExpanded);
        }
    }

}