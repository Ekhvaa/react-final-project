import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import './App.css'
import HomePage from './components/HomePage'
import TourPage from './Pages/TourPage'
import { Route, Routes } from 'react-router-dom'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
    <Routes>
      <Route path='/' element={<HomePage />}/>
      <Route path='/tours' element={<TourPage />}/>
    </Routes>
    </>
  )
}

export default App
