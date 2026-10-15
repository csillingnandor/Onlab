import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './theme/themeDom.js'; // Bootstrap stíluslap (Darkly / Flatly) és data-theme a React előtt
import 'bootstrap-icons/font/bootstrap-icons.css';
import './theme/theme.css';
import App from './App.js'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
