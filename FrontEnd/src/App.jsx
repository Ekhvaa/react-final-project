import './App.css';
import HomePage from './Pages/HomePage';
import TourPage from './Pages/TourPage';
import TourDetailsPage from './Pages/TourDetailsPage';
import LoginPage from './Pages/LoginPage';
import RegisterPage from './Pages/RegistrationPage';
import ProfilePage from './Pages/ProfilePage';
import ContactPage from './Pages/ContactPage';
import ProtectedRoute from './components/ProtectedRoute';
import { Route, Routes } from 'react-router-dom';

function App() {
  return (
    <Routes>
      <Route path='/' element={<HomePage />} />
      <Route path='/tours' element={<TourPage />} />
      <Route path='/tours/:id' element={<TourDetailsPage />} />
      <Route path='/contact' element={<ContactPage />} />
      <Route path='/login' element={<LoginPage />} />
      <Route path='/register' element={<RegisterPage />} />

      <Route element={<ProtectedRoute />}>
        <Route path='/profile' element={<ProfilePage />} />
      </Route>
    </Routes>
  );
}

export default App;