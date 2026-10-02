import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import SideBar from './main_components/SideBar.js';
import ProductPage from './main_components/product/ProductPage.js';
import CustomerOrderPage from './main_components/order/CustomerOrderPage.js';
import './MainContent.css';

export default function MainContent() {
  return (
    <main className="app-main-content">
      <SideBar />
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<div>Dashboard Nézet</div>} />
        <Route path="/products" element={<ProductPage />} />
        <Route path="/warehouse" element={<div>Raktár Nézet</div>} />
        <Route path="/customer-orders" element={<CustomerOrderPage />} />
        <Route path="/supplier-orders" element={<div>Beszállítói Rendelések</div>} />
        <Route path="/customers" element={<div>Ügyfelek</div>} />
      </Routes>
    </main>
  );
}