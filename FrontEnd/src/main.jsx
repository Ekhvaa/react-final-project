import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import { BrowserRouter } from 'react-router-dom'
import { TourBookmarkProvider } from './contexts/TourBookmarkContext.jsx'
import { AuthProvider } from './contexts/AuthContext.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <TourBookmarkProvider>
        <AuthProvider>
          <App />
        </AuthProvider>
      </TourBookmarkProvider>
    </BrowserRouter>
  </StrictMode>,
)
