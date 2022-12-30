using System;
using System.Threading.Tasks;
using MudBlazor;

namespace Coworkee.Client.Shared.Components;

public partial class EditableDataTable
{
    public class CustomAction<T>
    {
        public CustomAction()
        { }

        public CustomAction(string text, string icon, Func<T[], Task> action, CustomActionAvailability availability = CustomActionAvailability.Always)
        {
            Text = text;
            Icon = icon;
            Action = action;
            Availability = availability;
        }

        public string Text { get; set; }
        public Color Color { get; set; } = Color.Primary;
        public string Icon { get; set; }
        public Func<T[], Task> Action { get; set; }
        public CustomActionAvailability Availability { get; set; }
    }

    public enum CustomActionAvailability
    {
        Always,
        SingleOnly,
        MultipleOnly
    }
}