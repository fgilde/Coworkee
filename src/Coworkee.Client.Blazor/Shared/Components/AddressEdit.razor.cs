using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Coworkee.Application.Common.Models.Identity;

namespace Coworkee.Client.Shared.Components;

public partial class AddressEdit
{
    [Parameter] public AddressDto Address { get; set; }
    [Parameter] public bool ShowNameInput { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public EventCallback<AddressDto> AddressCreated { get; set; }

    [Parameter] public Variant Variant { get; set; } = Variant.Text;

    private MudTextField<string> _houseNumberField;
    private bool _autoFocusToNumberExecuted;

    private async Task StreetKeyDown(KeyboardEventArgs args)
    {
        if (!ReadOnly && !_autoFocusToNumberExecuted && int.TryParse(args.Key, out _))
        {
            await _houseNumberField.FocusAsync();
            _autoFocusToNumberExecuted = true;
        }
    }

    private Task CreateAddress()
    {
        Address = new AddressDto();
        return AddressCreated.InvokeAsync(Address);
    }
}