using Microsoft.AspNetCore.Components.Web;

namespace Coworkee.Client.JsInterop.Models;

public class JsAppData
{
    public MouseEventArgs MouseArgs { get; set; }
    public BrowserDimension BrowserDimensions { get; set; }
}
