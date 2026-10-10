namespace MyApp.Contracts.Sales;

public static class SalesPermissions
{
    public const string GroupName = "Sales";

    public static class Orders
    {
        public const string View = "SalesOrders.View";
        public const string Create = "SalesOrders.Create";
        public const string Edit = "SalesOrders.Edit";
        public const string Delete = "SalesOrders.Delete";
    }
}
