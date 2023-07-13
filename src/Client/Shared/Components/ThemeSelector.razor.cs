using System;
using Coworkee.Client.Extensions;
using Coworkee.Client.Theming;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor.Extensions;

namespace Coworkee.Client.Shared.Components;

public partial class ThemeSelector
{
    private bool _charmOpen;
    private ICollection<ThemePreset<ClientTheme>> _themes;
    private bool _rtl;


    internal bool IsDark;
    private ThemePreset<ClientTheme> _selected;

    [Parameter]
    public EventCallback<ClientTheme> OnThemeSelected { get; set; }
    
    [Parameter]
    public string Icon { get; set; } = Icons.Material.Filled.Brightness4;

    [Parameter]
    public ThemePreset<ClientTheme> Selected
    {
        get => _selected;
        set => _ = Select(value);
    }

    [Parameter] public bool ToggleOnly { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        _rtl = await _clientPreferenceManager.IsRTL();
    }

    protected override async Task OnInitializedAsync()
    {
        var preference = await _clientPreferenceManager.GetPreference();
        IsDark = preference.DarkMode ?? await _themeManager.BrowserPrefersDarkMode();
        _themes = ThemePreset.Create(await _themeManager.ThemesAsync());
    }


    private async Task Select(ThemePreset<ClientTheme> preset)
    {
        _selected = preset;
        await _clientPreferenceManager.SetCurrentThemeAsync(preset.Name, preset);
        await OnThemeSelected.InvokeAsync(preset);
    }

    private void ButtonClicked()
    {
        if (ToggleOnly)
        {
            var index = Selected != null ? Array.IndexOf(_themes.ToArray(), Selected) + 1 : 0;
            var toSelect = _themes.ElementAt(index > _themes.Count - 1 ? 0 : index);
            _=Select(toSelect);
            return;
        }
        _charmOpen = true;
    }


    private async void EditTheme()
    {
        var defaultDialogOptionsEx = await DialogServiceExtensions.DefaultDialogOptionsEx();

        var res = await _dialogService.ShowComponentInDialogAsync<MudExThemeEdit<ClientTheme>>("Edit Theme", "", edit =>
        {
            edit.Theme = ClientTheme.LastUsedTheme.Clone();
            edit.AllowPresetsEdit = false;
        }, defaultDialogOptionsEx);

        if (!res.DialogResult.Canceled)
        {
            await _clientPreferenceManager.SaveCurrentThemeChangesAsync(res.Component.Theme);
            StateHasChanged();
        }
        //var toEdit = ClientTheme.LastUsedTheme.Clone();
        //var res = await _dialogService.EditObject(toEdit, "Edit Theme", defaultDialogOptionsEx);
        //if (!res.Cancelled)
        //{
        //    await _clientPreferenceManager.SaveCurrentThemeChangesAsync(toEdit);
        //}

        //_charmOpen = false;
        //StateHasChanged();
    }

    private async Task OnDarkChange(bool arg)
    {
        IsDark = arg;
        await _clientPreferenceManager.SetPreference(p => p.DarkMode = arg);
    }

}