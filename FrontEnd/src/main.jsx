import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import { BrowserRouter } from 'react-router-dom'
import { TourBookmarkProvider } from './contexts/TourBookmarkContext.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <TourBookmarkProvider>
        <App />
      </TourBookmarkProvider>
    </BrowserRouter>
  </StrictMode>,
)
