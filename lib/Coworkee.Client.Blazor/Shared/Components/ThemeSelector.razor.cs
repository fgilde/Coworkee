using System;
using lib.Coworkee.Client.Extensions;
using lib.Coworkee.Client.Theming;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Helper;

namespace lib.Coworkee.Client.Shared.Components;

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
        if(preset?.Theme?.PreferDarkMode.HasValue == true)
            await OnDarkChange(preset.Theme.PreferDarkMode.Value);
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

        var res = await _dialogService.ShowComponentInDialogAsync<MudExThemeEdit<ClientTheme>>("Customize", "",
            themeEdit =>
            {
                themeEdit.AllowPresetsEdit = false;
                themeEdit.AllowModeToggle = false;
                themeEdit.EditMode = ThemeEditMode.Simple;
                themeEdit.Theme = ClientThemes.LastUsedTheme.CloneTheme();
            },
            dialog =>
            {
                dialog.ClassActions = MudExCss.Classes.Dialog.DialogActionsSticky;
                dialog.Icon = Icons.Material.Filled.Palette;
                dialog.Buttons = MudExDialogResultAction.OkCancel();
            }, defaultDialogOptionsEx);

        if (!res.DialogResult.Canceled)
        {
            await _clientPreferenceManager.SaveCurrentThemeChangesAsync(res.Component.Theme);
            StateHasChanged();
        }
    }

    private async Task OnDarkChange(bool arg)
    {
        IsDark = arg;
        await _clientPreferenceManager.SetPreference(p => p.DarkMode = arg);
    }

}