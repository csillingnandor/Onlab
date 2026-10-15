namespace IOMS.BLL.Auth;

// A frontend oldalai, amelyekhez szerepkörönként (csoportonként) hozzáférés adható.
// A szerepkörök AspNetRoleClaims soraiban tárolódnak (ClaimType = "page"), bejelentkezéskor bekerülnek a tokenbe,
// az API végpontjai pedig ezekre hivatkoznak ([RequirePage(...)]). A kulcsokat a frontend is használja (util/pages.ts).
public static class AppPages
{
    public const string ClaimType = "page";

    public const string Dashboard = "dashboard";
    public const string Products = "products";
    public const string Warehouses = "warehouses";
    public const string CustomerOrders = "customer-orders";
    public const string SupplierOrders = "supplier-orders";
    public const string Customers = "customers";
    public const string Users = "users";

    public static readonly IReadOnlyList<string> All =
        [Dashboard, Products, Warehouses, CustomerOrders, SupplierOrders, Customers, Users];
}
