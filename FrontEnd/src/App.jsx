import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import './App.css'
import HomePage from './Pages/HomePage'
import TourPage from './Pages/TourPage'
import { Route, Routes } from 'react-router-dom'
import TourDetailsPage from './Pages/TourDetailsPage'
import LoginPage from './Pages/LoginPage'
import RegisterPage from './Pages/RegistrationPage'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
    <Routes>
      <Route path='/' element={<HomePage />}/>
      <Route path='/tours' element={<TourPage />}/>
      <Route path='/tours/:id' element={<TourDetailsPage />}/>
      <Route path='/login' element={<LoginPage />}/>
      <Route path='/register' element={<RegisterPage />}/>
    </Routes>
    </>
  )
}

export default App
