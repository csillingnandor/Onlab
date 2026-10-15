namespace IOMS.BLL.Auth;

// Az oldalakon végezhető műveletek, amelyeket szerepkörönként külön lehet engedélyezni
// (pl. a rendeléseket látja, de lemondani nem tudja). A megtekintést az oldal-jogosultság adja (AppPages),
// a művelet csak az oldalával együtt érvényes. Kulcs: "<oldal>.<művelet>"; tárolás: AspNetRoleClaims (ClaimType = "action").
// A kulcsokat a frontend is használja (util/pages.ts).
public static class AppActions
{
    public const string ClaimType = "action";

    public const string ProductsCreate = "products.create";
    public const string ProductsEdit = "products.edit";
    public const string ProductsDelete = "products.delete";
    public const string ProductsAdjustStock = "products.adjust-stock";
    public const string ProductsImport = "products.import";

    public const string WarehousesCreate = "warehouses.create";
    public const string WarehousesEdit = "warehouses.edit";
    public const string WarehousesDelete = "warehouses.delete";

    public const string CustomerOrdersCreate = "customer-orders.create";
    public const string CustomerOrdersEdit = "customer-orders.edit";
    public const string CustomerOrdersShip = "customer-orders.ship";
    public const string CustomerOrdersDeliver = "customer-orders.deliver";
    public const string CustomerOrdersCancel = "customer-orders.cancel";

    public const string SupplierOrdersCreate = "supplier-orders.create";
    public const string SupplierOrdersEdit = "supplier-orders.edit";
    public const string SupplierOrdersReceive = "supplier-orders.receive";

    public const string CustomersCreate = "customers.create";
    public const string CustomersEdit = "customers.edit";
    public const string CustomersDelete = "customers.delete";

    public const string SuppliersCreate = "suppliers.create";
    public const string SuppliersEdit = "suppliers.edit";
    public const string SuppliersDelete = "suppliers.delete";

    // Oldalanként a hozzá tartozó műveletek (a "users" oldal maga a teljes felhasználókezelés, külön műveletek nélkül)
    public static readonly IReadOnlyDictionary<string, string[]> ByPage = new Dictionary<string, string[]>
    {
        [AppPages.Products] = [ProductsCreate, ProductsEdit, ProductsDelete, ProductsAdjustStock, ProductsImport],
        [AppPages.Warehouses] = [WarehousesCreate, WarehousesEdit, WarehousesDelete],
        [AppPages.CustomerOrders] = [CustomerOrdersCreate, CustomerOrdersEdit, CustomerOrdersShip, CustomerOrdersDeliver, CustomerOrdersCancel],
        [AppPages.SupplierOrders] = [SupplierOrdersCreate, SupplierOrdersEdit, SupplierOrdersReceive],
        [AppPages.Customers] = [CustomersCreate, CustomersEdit, CustomersDelete],
        [AppPages.Suppliers] = [SuppliersCreate, SuppliersEdit, SuppliersDelete],
    };

    public static readonly IReadOnlyList<string> All = [.. AppPages.All.SelectMany(page => ByPage.GetValueOrDefault(page, []))];

    // "customer-orders.cancel" -> "customer-orders"
    public static string PageOf(string action) => action[..action.LastIndexOf('.')];
}
