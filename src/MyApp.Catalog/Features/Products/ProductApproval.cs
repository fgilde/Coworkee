using Coworkee.Workflows;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products;

/// <summary>
/// Products start as drafts; a product manager submits them and an administrator other than the submitter approves or rejects within three days
/// (reminder after one day, then the escalation reaches every administrator). An Elsa workflow (MyApp.Api) tells the product managers about approvals.
/// </summary>
public static class ProductApproval
{
    public const string EntityType = "Products";

    public const string Approver = "admin";

    public static IServiceCollection AddProductApproval(this IServiceCollection services) =>
        services.AddCoworkeeWorkflows(workflows => workflows.Entity<Product>(EntityType, product =>
        {
            product.Initial("Draft").Describe(p => p.Name).Link(_ => "/catalog/products");
            product.Transition("Submit", "Draft", "In review").RequiresPermission(CatalogPermissions.Products.Edit).When(p => p.Rate > 0);
            product.Transition("Withdraw", "In review", "Draft").RequiresPermission(CatalogPermissions.Products.Edit);
            product.Transition("Reopen", ["Approved", "Rejected"], "Draft").RequiresPermission(CatalogPermissions.Products.Edit).RequiresComment();
            product.State("In review").Approval("Approved", "Rejected", approval => approval
                .Role(Approver)
                .NotByRequester()
                .DueIn(TimeSpan.FromDays(3))
                .RemindAfter(TimeSpan.FromDays(1))
                .EscalateTo(Approver));
        }));
}
