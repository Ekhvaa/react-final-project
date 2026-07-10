import React from 'react';

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-slate-800 text-slate-200 pt-10 pb-6 px-5 font-sans">
      <div className="max-w-6xl mx-auto flex flex-col md:flex-row flex-wrap justify-between gap-8">
        
        <div className="flex-1 min-w-[200px]">
          <h2 className="text-amber-500 text-2xl font-bold mb-3">Wanderlust Tours</h2>
          <p className="text-sm leading-relaxed text-slate-300">
            Discover your next adventure. We provide unforgettable guided tours and experiences around the globe.
          </p>
        </div>

        <div className="flex-1 min-w-[200px]">
          <h3 className="text-amber-500 text-lg font-bold mb-3">Explore</h3>
          <ul className="space-y-2">
            <li>
              <a href="/destinations" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Destinations
              </a>
            </li>
            <li>
              <a href="/tours" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Our Tours
              </a>
            </li>
            <li>
              <a href="/about" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                About Us
              </a>
            </li>
            <li>
              <a href="/blog" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Travel Blog
              </a>
            </li>
          </ul>
        </div>

        <div className="flex-1 min-w-[200px]">
          <h3 className="text-amber-500 text-lg font-bold mb-3">Support</h3>
          <ul className="space-y-2">
            <li>
              <a href="/contact" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Contact Us
              </a>
            </li>
            <li>
              <a href="/faq" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                FAQs
              </a>
            </li>
            <li>
              <a href="/terms" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Terms & Conditions
              </a>
            </li>
            <li>
              <a href="/privacy" className="hover:text-amber-500 hover:underline transition-colors duration-300">
                Privacy Policy
              </a>
            </li>
          </ul>
        </div>

      </div>

      <div className="text-center mt-10 pt-5 border-t border-slate-700 text-sm text-slate-400">
        <p>&copy; {currentYear} Wanderlust Tours. All rights reserved.</p>
      </div>
    </footer>
  );
};

export default Footer;