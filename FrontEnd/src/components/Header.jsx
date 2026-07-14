import React, { useState } from 'react';
import profile from '../assets/Profile.svg';
import { Link, NavLink } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import Logo from './Logo';

const navLinks = [
    { to: "/", label: "Home" },
    { to: "/tours", label: "Tours" },
    { to: "/contact", label: "Contact" }
];

export default function Header() {
    const [isOpen, setIsOpen] = useState(false);
    const { user, isAuthenticated, logout } = useAuth();

    const handleLogout = () => {
        setIsOpen(false);
        logout();
    };

    return (
        <header className='relative w-full p-3 bg-[#8dfc9c] text-white z-50'>
            <div className='flex justify-between items-center w-full'>
                <div>
                    <Link to='/'>
                        <Logo />
                    </Link>
                </div>

                <div className='hidden md:flex gap-15 items-center'>
                    <nav className='flex gap-6'>
                        {navLinks.map(link => (
                            <NavLink
                                key={link.to}
                                to={link.to}
                                end
                                className="hover:opacity-80 transition-opacity"
                            >
                                {link.label}
                            </NavLink>
                        ))}
                    </nav>

                    {isAuthenticated ? (
                        <div className='flex items-center gap-4'>
                            <Link
                                to="/profile"
                                className='flex gap-3 items-center hover:opacity-80 transition-opacity'
                            >
                                <img src={profile} alt="Profile" className='h-6' />
                                <p>{user?.username}</p>
                            </Link>

                            <button
                                type="button"
                                onClick={handleLogout}
                                className='bg-[#fcfc03] hover:bg-[#fcfb9d] px-3 py-2 text-black rounded-xl'
                            >
                                Logout
                            </button>
                        </div>
                    ) : (
                        <Link
                            to='/login'
                            className='bg-[#fcfc03] hover:bg-[#fcfb9d] px-3 py-2 text-black rounded-xl'
                        >
                            Login
                        </Link>
                    )}
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
                        {isAuthenticated ? (
                            <div className='flex flex-col gap-3'>
                                <Link
                                    to="/profile"
                                    className='flex gap-3 items-center py-2'
                                    onClick={() => setIsOpen(false)}
                                >
                                    <img src={profile} alt="Profile" className='h-6' />
                                    <p>{user?.username}</p>
                                </Link>

                                <button
                                    type="button"
                                    onClick={handleLogout}
                                    className='bg-[#fcfc03] hover:bg-[#fcfb9d] px-3 py-2 text-black rounded-xl text-left'
                                >
                                    Logout
                                </button>
                            </div>
                        ) : (
                            <Link
                                to='/login'
                                className='block bg-[#fcfc03] hover:bg-[#fcfb9d] px-3 py-2 text-black rounded-xl'
                                onClick={() => setIsOpen(false)}
                            >
                                Login
                            </Link>
                        )}
                    </div>
                </div>
            )}
        </header>
    );
}