import React from 'react'
import logo from '../assets/Logo.svg'
import profile from '../assets/Profile.svg'
import { Link, NavLink } from 'react-router-dom'

const navLinks = [
    { to: "/", label: "Home"},
    { to: "/tours", label: "Tours"},
    { to: "/contact", label: "Contact"},
    { to: "/profile", label: "Profile"}
]

const Header = () => {
  return (
    <div className='flex justify-between items-center w-full p-3 bg-[#8dfc9c] text-white'>
        <div>
            <Link to='/'>
                <img src={logo} alt='logo' className='h-15'></img>
            </Link>
        </div>
        <div className='flex gap-15'>
            <nav className='flex gap-6'>
                {navLinks.map(link => (
                    <NavLink key={link.to} to={link.to} end>
                        {link.label}
                    </NavLink>
                ))}
            </nav>
            <div>
                <Link to="/profile" className='flex gap-3'>
                    <img src={profile} alt="Profile" className='h-6'></img>
                    <p>Ekhva</p>
                </Link>
            </div>
        </div>
    </div>
  )
}

export default Header