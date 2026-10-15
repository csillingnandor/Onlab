namespace IOMS.BLL.Auth;

public static class AppRoles
{
    // Rendszer-szerepkör: mindig minden oldalt és műveletet elér, nem nevezhető át és nem törölhető
    public const string Admin = "Adminisztrátor";

    public record DefaultRole(string[] Pages, string[] Actions);

    // Induló szerepkörök; csak akkor jönnek létre, ha még nincsenek (utána a Felhasználók oldalon szerkeszthetők)
    public static readonly IReadOnlyDictionary<string, DefaultRole> Defaults = new Dictionary<string, DefaultRole>
    {
        [Admin] = new([.. AppPages.All], [.. AppActions.All]),
        ["Raktárvezető"] = new(
            [AppPages.Dashboard, AppPages.Products, AppPages.Warehouses, AppPages.SupplierOrders],
            [AppActions.ProductsCreate, AppActions.ProductsEdit, AppActions.ProductsDelete, AppActions.ProductsAdjustStock,
             AppActions.ProductsImport, AppActions.WarehousesCreate, AppActions.WarehousesEdit, AppActions.WarehousesDelete,
             AppActions.SupplierOrdersReceive]),
        ["Értékesítő"] = new(
            [AppPages.Dashboard, AppPages.CustomerOrders, AppPages.Customers],
            [AppActions.CustomerOrdersCreate, AppActions.CustomerOrdersEdit, AppActions.CustomerOrdersShip,
             AppActions.CustomerOrdersDeliver, AppActions.CustomerOrdersCancel,
             AppActions.CustomersCreate, AppActions.CustomersEdit, AppActions.CustomersDelete]),
        ["Beszerző"] = new(
            [AppPages.Dashboard, AppPages.SupplierOrders, AppPages.Warehouses, AppPages.Suppliers],
            [AppActions.SupplierOrdersCreate, AppActions.SupplierOrdersEdit,
             AppActions.SuppliersCreate, AppActions.SuppliersEdit, AppActions.SuppliersDelete]),
    };
}
