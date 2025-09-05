using lib.Coworkee.Application.Common.Models;
using MudBlazor;

namespace Coworkee.Client.Models;

public class AssistantCommandClientModel : AssistantCommandDto
{
    public bool Completed { get; set; }
    public Severity Severity => Owner == AssistantCommandOwner.Assistant ? Severity.Info : Severity.Normal;
    public Color Color => Owner == AssistantCommandOwner.Assistant ? Color.Info : Color.Default;
    public Variant Variant => Variant.Filled;
}
