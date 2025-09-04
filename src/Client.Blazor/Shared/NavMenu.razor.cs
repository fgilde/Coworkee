using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Client.Enums;
using Coworkee.Client.Extensions;
using Coworkee.Client.JsInterop;
using Coworkee.Client.Models.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions.Components;
using MudBlazor.Extensions.Core.Enums;
using Nextended.Core.Extensions;
using Nextended.Core.Types;


namespace Coworkee.Client.Shared
{
    public partial class NavMenu
    {
        [CascadingParameter]
        internal ClaimsPrincipal User { get; set; }

  
        [Parameter] public bool IsMini { get; set; }

        [Parameter] public bool ShowUserCard { get; set; } = true;

        [Parameter] public bool ShowApplicationLogo { get; set; } = false;

        private NavigationEntry _selectedNavEntry;
        private TreeViewExpandBehaviour _expandBehaviour;
        private TreeViewMode _viewMode = TreeViewMode.Default;

        public NavigationEntry SelectedNavEntry
        {
            get => _selectedNavEntry;
            set
            {
                if (_selectedNavEntry != value)
                {
                    _selectedNavEntry = value;
                    if (HasAction(value))
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(value.Target))
                                _ = ServiceAccessor.Get<IJSRuntime>().InvokeVoidAsync(JsNamespace.Get("BrowserHelper", "navigateToExternalUrl"), value.Href, value.Target);
                            else
                                _navigationManager.NavigateTo(value.Href);
                        }
                        catch
                        { }
                    }
                }
            }
        }

        [Parameter]
        public TreeViewMode ViewMode
        {
            get => IsMini ? TreeViewMode.FlatList : _viewMode;
            set
            {
                if (IsMini || _viewMode == value)
                    return;
                _viewMode = value;
                InvokeAsync(StateHasChanged);
            }
        }

        [Parameter]
        public TreeViewExpandBehaviour ExpandBehaviour
        {
            get => _expandBehaviour;
            set
            {
                if (value != _expandBehaviour)
                {
                    _expandBehaviour = value;
                    InvokeAsync(StateHasChanged);
                }
            }
        }

        [Parameter] public HashSet<NavigationEntry> Entries { get; set; }

        [Parameter] public EventCallback Logout { get; set; }
        
        public HashSet<NavigationEntry> AuthorizedEntries => Entries.Where(IsAuthorized).ToHashSet();

        private bool IsAuthorized(TreeViewItemContext<NavigationEntry> entry) => IsAuthorized(entry.Value);
        private bool IsAuthorized(NavigationEntry entry)
        {
            bool result = (!entry.IsAuthenticationRequired || (User?.Identity?.IsAuthenticated == true && !User.IsGuest()))
                          && User?.HasRoles(entry.RoleMatch, entry.Roles) == true
                          && entry?.MatchesConditions(User) == true
                          && _authorizationService.HasPoliciesAsync(User, entry.PolicyMatch, entry.Policies).GetAwaiter().GetResult();
            _navigationManager.EnsureUrlIsAccessible(User, entry.Href).ContinueWith(task =>
            {
                entry.Href = task.Result;
            });
            if (entry.HasChildren)
                return result && entry.Children.Any(IsAuthorized);
            return result;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _navigationManager.LocationChanged += (s, e) => ExpandToCurrentUrl();
            }

            return base.OnAfterRenderAsync(firstRender);
        }


        protected override void OnParametersSet()
        {
            Entries ??= Navigations.Default(_config);
            ExpandToCurrentUrl();
            base.OnParametersSet();
        }

        private void ExpandToCurrentUrl()
        {
            var current = SelectedNavEntry;
            var url = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
            SelectedNavEntry = FindEntriesForUrl(url)?.FirstOrDefault();
            if (SelectedNavEntry != null && SelectedNavEntry != current)
                InvokeAsync(StateHasChanged);
        }

        public string Locale(string s)
        {
            return _localizer != null ? _localizer[s ?? ""] : s ?? "";
        }

        public IEnumerable<NavigationEntry> FindEntriesForUrl(string url = null)
        {
            url = (url ?? _navigationManager.ToBaseRelativePath(_navigationManager.Uri)).EnsureStartsWith("/").ToLower();
            return Entries.Find(e => e.Href?.EnsureStartsWith("/")?.ToLower() == url);
        }

        
        private bool HasAction(NavigationEntry entry) => !string.IsNullOrWhiteSpace(entry?.Href);
    }

}