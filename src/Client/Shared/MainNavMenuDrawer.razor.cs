using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions.Helper;
using System.Threading.Tasks;
using Coworkee.Client.Enums;
using System.Security.Claims;
using Coworkee.Application.Common.Extensions;

namespace Coworkee.Client.Shared;

public partial class MainNavMenuDrawer
{
    private bool _pinned = true;
    
    private bool _singleExpand;

    public NavMenu Menu { get; private set; }

    [CascadingParameter] 
    internal MainLayout Layout { get; set; }

    [CascadingParameter]
    internal ClaimsPrincipal User { get; set; }

    [Parameter]
    public EventCallback<bool> PinnedChanged { get; set; }

    [Parameter]
    public EventCallback<bool> SingleExpandChanged { get; set; }

    [Parameter]
    public bool Pinned
    {
        get => _pinned;
        set => UpdatePinned(value);
    }

    [Parameter]
    public bool SingleExpand
    {
        get => _singleExpand;
        set => UpdateSingleExpand(value);
    }

    public bool AllowSettingsChange => Layout?.CurrentTheme is not {CanPinDrawer: false, CanChangeDrawerExpandMode: false};

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var preference = await _clientPreferenceManager.GetPreference();
        Pinned = preference.IsDrawerPinned;
        SingleExpand = preference.DrawerSingleExpand;
    }

    protected override async Task OnParametersSetAsync()
    {
        SetVariantAndClipMode();
        await base.OnParametersSetAsync();
    }

    private void SetVariantAndClipMode()
    {
        if (Layout.CurrentTheme.CanPinDrawer)
        {
            Variant = Pinned ? DrawerVariant.Mini : DrawerVariant.Temporary;
            ClipMode = (Pinned ? DrawerClipMode.Always : DrawerClipMode.Never);
        }
        else
        {
            Variant = Layout.CurrentTheme.DrawerVariant;
            ClipMode = Layout.CurrentTheme.DrawerClipMode;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ChildContent = DrawerContent();
    }

    private bool ShowLogoInMenu() => (Open && (Layout?.CurrentTheme?.ShowLogoInNavMenu ?? !Pinned));
    private bool ShowUserInMenu() => (Open && (Layout?.CurrentTheme?.ShowUserCardInNavigation ?? !Pinned));

    private ExpandMode GetExpandMode() => Layout?.CurrentTheme?.CanChangeDrawerExpandMode == true ? (SingleExpand ? ExpandMode.SingleExpand : ExpandMode.Default) : (Layout?.CurrentTheme?.NavMenuExpandMode ?? ExpandMode.Default);

    private string SettingsContainerStyle()
    {
        return MudExStyleBuilder.Empty()
                .With("align-self", "flex-start")
                .WithMarginLeft("auto", !MiniAndClosed())
                .WithMarginLeft(-13, MiniAndClosed())
                .WithBorderBottomWidth(1)
                .WithBorderColor(Color.Secondary)
                .WithPadding(4, MiniAndClosed())
                .Build();
    }

    private bool MiniAndClosed() => !Open && Variant == DrawerVariant.Mini;

    private void Update<T>(T value, Action<T> s, EventCallback<T> toRaise)
    {
        s(value);
        toRaise.InvokeAsync(value);
        SavePreferences();
    }
    
    private void UpdateSingleExpand(bool value) => Update(value, b => _singleExpand = b, SingleExpandChanged);
    private void UpdatePinned(bool value)
    {
        if (!value && !Open)
        {
            Open = true;
            _popoverOpen = false;
        }

        Update(value, b => _pinned = b, PinnedChanged);
    }

    private void SavePreferences()
    {
        _ = _clientPreferenceManager.SetPreference(p =>
        {
            p.IsDrawerPinned = Pinned;
            p.DrawerSingleExpand = SingleExpand;
        });
    }
}