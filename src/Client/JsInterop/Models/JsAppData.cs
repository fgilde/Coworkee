using Microsoft.AspNetCore.Components.Web;

namespace CleanArchitectureBase.Client.JsInterop.Models;

public class JsAppData
{
    public MouseEventArgs MouseArgs { get; set; }
    public BrowserDimension BrowserDimensions { get; set; }
}
