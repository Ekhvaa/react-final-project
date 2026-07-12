import React from 'react'
import logo from '../assets/Logo.svg'
import profile from '../assets/Profile.svg'
import { useState } from 'react'
import { Link, NavLink } from 'react-router-dom'

const navLinks = [
    { to: "/", label: "Home"},
    { to: "/tours", label: "Tours"},
    { to: "/contact", label: "Contact"},
    { to: "/profile", label: "Profile"}
]

export default function Header() {
    const [isOpen, setIsOpen] = useState(false);

    return (
        <header className='relative w-full p-3 bg-[#8dfc9c] text-white z-50'>
            <div className='flex justify-between items-center w-full'>
                <div>
                    <Link to='/'>
                        <img src={logo} alt='logo' className='h-15'></img>
                    </Link>
                </div>
                <div className='hidden md:flex gap-15 items-center'>
                    <nav className='flex gap-6'>
                        {navLinks.map(link => (
                            <NavLink key={link.to} to={link.to} end className="hover:opacity-80 transition-opacity">
                                {link.label}
                            </NavLink>
                        ))}
                    </nav>
                    <div>
                        <Link to="/profile" className='flex gap-3 items-center hover:opacity-80 transition-opacity'>
                            <img src={profile} alt="Profile" className='h-6'></img>
                            <p>Ekhva</p>
                        </Link>
                    </div>
                </div>
                <button 
                    onClick={() => setIsOpen(!isOpen)} 
                    className='block md:hidden focus:outline-none p-2'
                    aria-label="Toggle navigation menu"
                >
                    {isOpen ? (
                        <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    ) : (
                        <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h16M4 18h16" />
                        </svg>
                    )}
                </button>
            </div>
            {isOpen && (
                <div className='absolute top-full left-0 w-full bg-[#8dfc9c] border-t border-white/20 p-4 flex flex-col gap-4 shadow-lg md:hidden animate-fadeIn'>
                    <nav className='flex flex-col gap-4'>
                        {navLinks.map(link => (
                            <NavLink 
                                key={link.to} 
                                to={link.to} 
                                end 
                                onClick={() => setIsOpen(false)}
                                className="py-2 border-b border-white/10"
                            >
                                {link.label}
                            </NavLink>
                        ))}
                    </nav>
                    <div className='pt-2'>
                        <Link 
                            to="/profile" 
                            className='flex gap-3 items-center py-2' 
                            onClick={() => setIsOpen(false)}
                        >
                            <img src={profile} alt="Profile" className='h-6'></img>
                            <p>Ekhva</p>
                        </Link>
                    </div>
                </div>
            )}
        </header>
    );
}
