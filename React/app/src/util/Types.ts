import type { ActionKey, PageKey } from './pages.js';

export type Product = {
    id: number;
    name: string;
    sku: string;
    category: string;
    stockQuantity: number; // összkészlet: a raktáronkénti mennyiségek összege
    minStockLevel: number;
    price: number;
    status: 'In Stock' | 'Low Stock' | 'Out of Stock';
    stocks: ProductStock[]; // raktáronkénti bontás, raktárnév szerint (csak ahol van készlet)
};

// A backend ProductStockData-jának megfelelő alak
export type ProductStock = {
    warehouseId: number;
    warehouseName: string;
    quantity: number;
};

// A backend StockMovementData-jának megfelelő alak (készlettörténet egy sora)
export type StockMovementType = 'Opening' | 'Receipt' | 'Shipment' | 'Adjustment';

export type StockMovement = {
    id: number;
    warehouseId: number;
    warehouseName: string;
    quantityChange: number; // előjeles: + beérkezés / nyitókészlet, − kiszállítás
    type: StockMovementType;
    supplierOrderId: number | null; // Receipt esetén
    customerOrderId: number | null; // Shipment esetén
    createdAt: string; // ISO dátum szöveg
    note: string | null; // kézi korrekció (Adjustment) indoka
};

// Termék létrehozásakor / módosításakor küldött mezők; a készletet a beszállítói rendelések beérkezése állítja
export type ProductInput = Omit<Product, 'id' | 'status' | 'stockQuantity' | 'stocks'>;

export type ViewMode = 'list' | 'grid';

// A backend CustomerOrderItemDto-jának megfelelő alak
export type CustomerOrderItem = {
    id: number;
    customerOrderId: number;
    productId: number;
    productName: string;
    productPrice: number;
    quantity: number;
};

export type Category = {
    id: number;
    name: string;
    parentCategoryId: number | null;
}

export type OrderStatus = 'Pending' | 'Shipped' | 'Delivered' | 'Cancelled';

// A backend CustomerOrderDto-jának megfelelő alak
export type CustomerOrder = {
    id: number;
    customerId: number;
    customerName: string;
    warehouseId: number; // kiszolgáló raktár: kiszállításkor ennek a készlete csökken
    warehouseName: string;
    orderDate: string; // ISO dátum szöveg
    status: OrderStatus;
    totalAmount: number;
    items: CustomerOrderItem[];
};

// A backend CustomerData-jának megfelelő alak
export type Customer = {
    id: number;
    name: string;
    email: string;
    phone: string | null;
    orderCount: number;
    totalSpent: number; // csak a kiszállított (Delivered) rendelésekből
    lastOrderDate: string | null; // ISO dátum szöveg; null, ha még nem rendelt
};

// A backend CreateCustomerData-jának megfelelő alak (új vevő felvétele)
export type NewCustomer = {
    name: string;
    email: string;
    phone: string | null;
};

// A backend SupplierData-jának megfelelő alak
export type Supplier = {
    id: number;
    name: string;
    email: string | null;
    phone: string | null;
    orderCount: number; // amíg van rendelése, nem törölhető
};

// A backend SaveSupplierData-jának megfelelő alak (felvétel és módosítás)
export type SupplierInput = {
    name: string;
    email: string | null;
    phone: string | null;
};

// A backend SaveWarehouseData-jának megfelelő alak (felvétel és módosítás)
export type WarehouseInput = {
    name: string;
    address: string;
    capacity: number;
    latitude: number | null;
    longitude: number | null;
};

// A backend StockAdjustmentData-jának megfelelő alak: a megszámolt mennyiség, a különbség kerül a naplóba
export type StockAdjustment = {
    warehouseId: number;
    countedQuantity: number;
    note: string;
};

// Termékimport előnézete (backend ProductImportPreviewData)
export type ProductImportRow = {
    rowNumber: number;
    name: string | null;
    sku: string | null;
    category: string | null;
    price: number | null;
    minStockLevel: number | null;
    action: 'Create' | 'Update';
    errors: Record<string, string[]>; // mezőnként; '' a sor egészére
};

export type ProductImportPreview = {
    schemaErrors: string[];
    warnings: string[];
    rows: ProductImportRow[];
    createCount: number;
    updateCount: number;
    errorCount: number;
};

export type ProductImportResult = {
    created: number;
    updated: number;
};

// Új rendelés tétele; unitCost csak beszerzési rendelésnél (vevői rendelésnél a termék aktuális ára számít)
export type NewOrderItem = {
    productId: number;
    quantity: number;
    unitCost?: number;
};

// A backend SupplierOrderItemData-jának megfelelő alak
export type SupplierOrderItem = {
    id: number;
    supplierOrderId: number;
    productId: number;
    productName: string;
    unitCost: number; // a beszállítónak fizetett egységár
    quantity: number;
};

// A backend SupplierOrderData-jának megfelelő alak
export type SupplierOrder = {
    id: number;
    supplierId: number;
    supplierName: string;
    warehouseId: number; // célraktár: beérkezéskor ennek a készlete nő
    warehouseName: string;
    orderDate: string; // ISO dátum szöveg
    status: OrderStatus;
    totalCost: number;
    items: SupplierOrderItem[];
};

// A backend WarehouseData-jának megfelelő alak
export type Warehouse = {
    id: number;
    name: string;
    address: string;
    capacity: number;
    latitude: number | null; // null, ha nincs megadva hely (a térképen nem jelenik meg)
    longitude: number | null;
    hasLayout: boolean; // van-e belső elrendezése (3D nézet)
};

export type WarehouseViewMode = 'table' | 'map';

// A backend WarehouseLayoutData-jának megfelelő alak. A rács egysége 1 m, az origó az alaprajz bal felső sarka,
// x jobbra, y a bejárat felé nő (a 3D jelenetben: x -> X, y -> Z, a szintek a Y tengelyen).
export type RackOrientation = 'Horizontal' | 'Vertical';

export type LocationStock = {
    productId: number;
    productName: string;
    productSku: string;
    quantity: number;
};

export type StorageLocation = {
    id: number;
    code: string; // pl. 'A-03-2': polc, rekesz, szint
    bay: number; // 1-től
    level: number; // 1-től, alulról
    maxQuantity: number;
    quantity: number; // a stocks összege
    stocks: LocationStock[];
};

export type Rack = {
    id: number;
    code: string;
    x: number;
    y: number;
    length: number; // rekeszek száma az orientation irányában
    levels: number;
    orientation: RackOrientation;
    locations: StorageLocation[];
};

export type WarehouseLayout = {
    warehouseId: number;
    warehouseName: string;
    hasLayout: boolean; // false: nincs megadva elrendezés (a méretek 0-k)
    width: number;
    depth: number;
    dockX: number;
    dockY: number;
    racks: Rack[];
    unplacedStock: LocationStock[]; // a depón lévő, helyhez még nem rendelt készlet
};

// A backend DailyProductSaleData-jának megfelelő alak
export type DailyProductSale = {
    date: string; // 'YYYY-MM-DD', időpont nélkül
    quantity: number;
    revenue: number;
    orderCount: number;
};

// A backend ProductSaleStatisticsData-jának megfelelő alak (csak a Delivered rendelések számítanak)
export type ProductSaleStatistics = {
    productId: number;
    productName: string;
    from: string; // 'YYYY-MM-DD', zárt intervallum: a to napja is benne van
    to: string; // 'YYYY-MM-DD'
    totalQuantity: number;
    totalRevenue: number;
    averageUnitPrice: number | null; // null, ha az időszakban nem volt eladás
    daily: DailyProductSale[];
};

// A backend CurrentUserData-jának megfelelő alak (a bejelentkezett felhasználó a token alapján)
export type CurrentUser = {
    id: string;
    userName: string;
    displayName: string;
    roles: string[];
    pages: PageKey[]; // a szerepkörei alapján elérhető oldalak
    actions: ActionKey[]; // az oldalakon engedélyezett műveletek
};

// A backend LoginResultData-jának megfelelő alak; a kliens ezt tárolja (auth/authStorage.ts)
export type AuthSession = {
    token: string;
    expiresAt: string; // ISO dátum szöveg
    user: CurrentUser;
};

// A backend UserData-jának megfelelő alak
export type User = {
    id: string;
    userName: string;
    displayName: string;
    email: string | null; // értesítésekhez; null: nem kap levelet
    roles: string[];
};

// A backend CreateUserData-jának megfelelő alak
export type NewUser = {
    userName: string;
    displayName: string;
    email: string | null;
    password: string;
    roles: string[];
};

// A backend ModifyUserData-jának megfelelő alak; a bejelentkezési név nem módosítható
export type UserUpdate = {
    displayName: string;
    email: string | null;
    newPassword: string | null; // null: a jelszó marad
    roles: string[];
};

// A backend RoleData-jának megfelelő alak
export type Role = {
    id: string;
    name: string;
    pages: PageKey[];
    actions: ActionKey[]; // csak a pages oldalaihoz tartozhatnak
    userCount: number;
    isSystem: boolean; // adminisztrátor: nem módosítható, nem törölhető, mindent elér
};

// A backend SaveRoleData-jának megfelelő alak
export type RoleInput = {
    name: string;
    pages: PageKey[];
    actions: ActionKey[];
};
