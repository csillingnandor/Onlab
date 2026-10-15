import type { TranslationKey } from '../i18n/translations.js';

// Az alkalmazás oldalai és menüje. A kulcsok a backend IOMS.BLL.Auth.AppPages konstansai:
// a szerepkörök ezekhez kapnak hozzáférést, a bejelentkezett felhasználó a user.pages listában kapja meg őket.
export type PageKey =
  | 'dashboard'
  | 'products'
  | 'warehouses'
  | 'customer-orders'
  | 'supplier-orders'
  | 'customers'
  | 'users';

export type AppPage = {
  key: PageKey;
  path: string;
  label: TranslationKey; // fordítási kulcs (i18n/translations.ts)
  icon: string; // Bootstrap Icons osztály
};

export type NavSection = {
  id: string;
  title: TranslationKey;
  icon: string;
  pages: AppPage[];
};

export const navSections: NavSection[] = [
  {
    id: 'dashboard',
    title: 'nav.section.dashboard',
    icon: 'bi-speedometer2',
    pages: [{ key: 'dashboard', path: '/dashboard', label: 'nav.page.dashboard', icon: 'bi-graph-up' }],
  },
  {
    id: 'warehouse',
    title: 'nav.section.warehouse',
    icon: 'bi-boxes',
    pages: [
      { key: 'products', path: '/products', label: 'nav.page.products', icon: 'bi-box-seam' },
      { key: 'warehouses', path: '/warehouse', label: 'nav.page.warehouses', icon: 'bi-building' },
    ],
  },
  {
    id: 'sales',
    title: 'nav.section.sales',
    icon: 'bi-cart3',
    pages: [{ key: 'customer-orders', path: '/customer-orders', label: 'nav.page.customerOrders', icon: 'bi-receipt' }],
  },
  {
    id: 'purchasing',
    title: 'nav.section.purchasing',
    icon: 'bi-truck',
    pages: [{ key: 'supplier-orders', path: '/supplier-orders', label: 'nav.page.supplierOrders', icon: 'bi-file-earmark-text' }],
  },
  {
    id: 'master-data',
    title: 'nav.section.masterData',
    icon: 'bi-database',
    pages: [{ key: 'customers', path: '/customers', label: 'nav.page.customers', icon: 'bi-people' }],
  },
  {
    id: 'admin',
    title: 'nav.section.admin',
    icon: 'bi-shield-lock',
    pages: [{ key: 'users', path: '/users', label: 'nav.page.users', icon: 'bi-person-gear' }],
  },
];

// Menüsorrendben
export const appPages: AppPage[] = navSections.flatMap((section) => section.pages);

// Az oldalakon külön engedélyezhető műveletek; a kulcsok a backend IOMS.BLL.Auth.AppActions konstansai.
// A megtekintést az oldal-jogosultság adja, a művelet csak az oldalával együtt érvényes.
export type ActionKey =
  | 'products.create'
  | 'products.edit'
  | 'products.delete'
  | 'customer-orders.create'
  | 'customer-orders.ship'
  | 'customer-orders.deliver'
  | 'customer-orders.cancel'
  | 'supplier-orders.create'
  | 'supplier-orders.receive'
  | 'customers.create';

export type PageAction = {
  key: ActionKey;
  label: TranslationKey;
};

export const pageActions: Partial<Record<PageKey, PageAction[]>> = {
  products: [
    { key: 'products.create', label: 'action.products.create' },
    { key: 'products.edit', label: 'action.products.edit' },
    { key: 'products.delete', label: 'action.products.delete' },
  ],
  'customer-orders': [
    { key: 'customer-orders.create', label: 'action.customerOrders.create' },
    { key: 'customer-orders.ship', label: 'action.customerOrders.ship' },
    { key: 'customer-orders.deliver', label: 'action.customerOrders.deliver' },
    { key: 'customer-orders.cancel', label: 'action.customerOrders.cancel' },
  ],
  'supplier-orders': [
    { key: 'supplier-orders.create', label: 'action.supplierOrders.create' },
    { key: 'supplier-orders.receive', label: 'action.supplierOrders.receive' },
  ],
  customers: [{ key: 'customers.create', label: 'action.customers.create' }],
};

// Menüsorrendben, a felirattal együtt
export const allActions: PageAction[] = appPages.flatMap((page) => pageActions[page.key] ?? []);
