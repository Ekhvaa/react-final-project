import React, { useEffect } from 'react'
import { useState } from 'react';
import searchIcon from '../assets/SearchIcon.svg'
import { cn } from '../lib/utils'

const Searchbar = ( {onChange, styleMod, value} ) => {
    const [query, setQuery] = useState(value);

    useEffect(() => {
        setQuery(value);
    }, [value]);

    const handleInputChange = (e) => {
        const value = e.target.value;
        setQuery(value);
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        if (onChange) onChange(query);
    };

  return (
    <form onSubmit={handleSubmit} className={cn(`w-full flex shadow-lg rounded-3xl bg-white ${styleMod}`)}>
        <input 
            type="text" 
            value={query}
            onChange={handleInputChange}
            placeholder="Which city you want to go in..." 
            className="w-full px-4 py-3 text-gray-700 focus:outline-none placeholder-gray-400"
        />
        <button 
            type="submit" 
            className="bg-[#fcfc03] rounded-full text-white px-3 py-3 m-3 hover:bg-[#fcfb9d] font-semibold transition-colors shrink-0"
        >
            <img src={searchIcon} alt="Search" className='h-5'/>
        </button>
    </form>
  )
}

export default Searchbar