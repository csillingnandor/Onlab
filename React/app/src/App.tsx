import { BrowserRouter } from 'react-router-dom';
import MainContent from './MainContent.js';
import Header from './Header.js';
import Footer from './Footer.js';

export default function App() {
  return (
    <BrowserRouter>
      <Header />
      <MainContent />
      <Footer />
    </BrowserRouter>
  );
}
