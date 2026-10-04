import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import SideBar from './main_components/SideBar.js';
import ProductPage from './main_components/product/ProductPage.js';
import CustomerOrderPage from './main_components/order/CustomerOrderPage.js';
import SupplierOrderPage from './main_components/order/SupplierOrderPage.js';
import './MainContent.css';
import DashboardPage from './main_components/dashboard/DashboardPage.js';
import WarehousePage from './main_components/warehouse/WarehousePage.js';
import CustomerPage from './main_components/customer/CustomerPage.js';

export default function MainContent() {
  return (
    <main className="app-main-content">
      <SideBar />
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/products" element={<ProductPage />} />
        <Route path="/warehouse" element={<WarehousePage />} />
        <Route path="/customer-orders" element={<CustomerOrderPage />} />
        <Route path="/supplier-orders" element={<SupplierOrderPage />} />
        <Route path="/customers" element={<CustomerPage />} />
      </Routes>
    </main>
  );
}