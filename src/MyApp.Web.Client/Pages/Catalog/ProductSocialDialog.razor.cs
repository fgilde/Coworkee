using Microsoft.AspNetCore.Components;

namespace MyApp.Web.Client.Pages.Catalog;

public partial class ProductSocialDialog
{
    [Parameter] public Guid ProductId { get; set; }
}
