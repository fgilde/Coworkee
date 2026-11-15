using Microsoft.AspNetCore.Components;

namespace MyApp.Web.Client.Pages.Documents;

/// <summary>The tab "Documents" of the admin user page: the documents the user owns.</summary>
public partial class UserDocuments
{
    [Parameter] public Guid UserId { get; set; }
}
