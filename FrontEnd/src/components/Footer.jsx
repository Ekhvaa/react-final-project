import React from 'react';
import { Link } from 'react-router-dom';
import Logo from './Logo';

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="mt-auto w-full bg-slate-800 text-slate-200 pt-10 pb-6 px-5 font-sans">
      <div className="max-w-6xl mx-auto flex flex-col md:flex-row flex-wrap justify-between gap-8">

        <div className="flex-1 min-w-[200px]">
          <Logo />
          <p className="text-sm leading-relaxed text-slate-300">
            Discover your next adventure. We provide unforgettable guided tours and experiences around the globe.
          </p>
        </div>

        <div className="flex-1 min-w-[200px]">
          <h3 className="text-amber-500 text-lg font-bold mb-3">Explore</h3>
          <ul className="space-y-2">
            <li>
              <Link to="/tours" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Our Tours
              </Link>
            </li>
            <li>
              <Link to="/contact" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Contact
              </Link>
            </li>
            <li>
              <Link to="/" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Home
              </Link>
            </li>
          </ul>
        </div>

        <div className="flex-1 min-w-[200px]">
          <h3 className="text-amber-500 text-lg font-bold mb-3">Support</h3>
          <ul className="space-y-2">
            <li>
              <Link to="/contact" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Contact Us
              </Link>
            </li>
            <li>
              <span className="text-slate-400">
                FAQs
              </span>
            </li>
            <li>
              <span className="text-slate-400">
                Terms & Conditions
              </span>
            </li>
            <li>
              <span className="text-slate-400">
                Privacy Policy
              </span>
            </li>
          </ul>
        </div>

      </div>

      <div className="text-center mt-10 pt-5 border-t border-slate-700 text-sm text-slate-400">
        <p>&copy; {currentYear} 1 კაციც და გავდივართ. All rights reserved.</p>
      </div>
    </footer>
  );
};

export default Footer;