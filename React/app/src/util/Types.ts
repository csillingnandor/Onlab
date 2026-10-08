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
};

export type WarehouseViewMode = 'table' | 'map';

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
