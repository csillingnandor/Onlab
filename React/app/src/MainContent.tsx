import type { ReactNode } from 'react';
import { Routes, Route } from 'react-router-dom';
import SideBar from './main_components/SideBar.js';
import ProductPage from './main_components/product/ProductPage.js';
import CustomerOrderPage from './main_components/order/CustomerOrderPage.js';
import SupplierOrderPage from './main_components/order/SupplierOrderPage.js';
import './MainContent.css';
import DashboardPage from './main_components/dashboard/DashboardPage.js';
import WarehousePage from './main_components/warehouse/WarehousePage.js';
import CustomerPage from './main_components/customer/CustomerPage.js';
import SupplierPage from './main_components/supplier/SupplierPage.js';
import UserAdminPage from './main_components/users/UserAdminPage.js';
import RequirePage, { HomeRedirect } from './auth/RequirePage.js';
import { appPages, type PageKey } from './util/pages.js';

// Oldalkulcsonként a megjelenítendő komponens; az útvonalakat a util/pages.ts adja
const pageElements: Record<PageKey, ReactNode> = {
  dashboard: <DashboardPage />,
  products: <ProductPage />,
  warehouses: <WarehousePage />,
  'customer-orders': <CustomerOrderPage />,
  'supplier-orders': <SupplierOrderPage />,
  customers: <CustomerPage />,
  suppliers: <SupplierPage />,
  users: <UserAdminPage />,
};

export default function MainContent() {
  return (
    <main className="app-main-content">
      <SideBar />
      <Routes>
        <Route path="/" element={<HomeRedirect />} />
        {appPages.map((page) => (
          <Route
            key={page.key}
            path={page.path}
            element={<RequirePage page={page.key}>{pageElements[page.key]}</RequirePage>}
          />
        ))}
        <Route path="*" element={<HomeRedirect />} />
      </Routes>
    </main>
  );
}
