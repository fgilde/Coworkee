using FluentValidation;
using System.Drawing;

namespace lib.Coworkee.Application.Common.Models;

public class AssistantCommandDto
{
    public AssistantCommandOwner Owner { get; set; }
    public string Message { get; set; }

    public string Role => Owner == AssistantCommandOwner.Assistant ? "assistant" : "user";
}

public enum AssistantCommandOwner
{
    Assistant,
    User
}