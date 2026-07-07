import React from 'react'
import { useState } from 'react';
import searchIcon from '../assets/SearchIcon.svg'

const Searchbar = () => {
    const [query, setQuery] = useState('');

    const handleInputChange = (e) => {
        const value = e.target.value;
        setQuery(value);
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        console.log(query);
        setQuery('');
    };

  return (
    <div className='w-full h-12 bg-white relative z-20 mg-20 rounded-full'>
        <form onSubmit={handleSubmit} className="flex shadow-lg rounded-lg overflow-hidden bg-white">
                <input 
                    type="text" 
                    value={query}
                    onChange={handleInputChange}
                    placeholder="Which city you want to go in..." 
                    className="w-full px-4 py-3 text-gray-700 focus:outline-none placeholder-gray-400"
                />
                <button 
                    type="submit" 
                    className="bg-[#fcfb9d] rounded-full text-white px-3 py-3 m-3 hover:bg-[#fcfc03] font-semibold transition-colors shrink-0"
                >
                    <img src={searchIcon} alt="Search" className='h-5'/>
                </button>
            </form>
    </div>
  )
}

export default Searchbar