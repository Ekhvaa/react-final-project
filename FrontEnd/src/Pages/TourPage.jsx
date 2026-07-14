import React, { useMemo, useState } from 'react';
import Header from '../components/Header';
import Footer from '../components/Footer';
import TourCard from '../components/TourCard';
import useFetch from '../hooks/useFetch';
import SearchIcon from '../assets/SearchIcon.svg?react'

const API_BASE_URL =
  'https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net';

const TourPage = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [sortBy, setSortBy] = useState('price-asc');

  const toursEndpointUrl = `${API_BASE_URL}/api/tours?page=1&pageSize=100`;

  const { data: tours, isLoading, error } = useFetch(
    toursEndpointUrl,
    (resData) => resData?.items || []
  );

  const toursList = Array.isArray(tours) ? tours : [];

  const filteredTours = useMemo(() => {
    const normalizedSearch = searchTerm.trim().toLowerCase();

    let result = toursList.filter((tour) => {
      if (!normalizedSearch) {
        return true;
      }

      return (
        tour?.name?.toLowerCase().includes(normalizedSearch) ||
        tour?.description?.toLowerCase().includes(normalizedSearch) ||
        tour?.startingCity?.toLowerCase().includes(normalizedSearch) ||
        tour?.startingCountry?.toLowerCase().includes(normalizedSearch)
      );
    });

    result = [...result].sort((a, b) => {
      if (sortBy === 'price-asc') {
        return Number(a?.currentPrice || 0) - Number(b?.currentPrice || 0);
      }

      if (sortBy === 'price-desc') {
        return Number(b?.currentPrice || 0) - Number(a?.currentPrice || 0);
      }

      if (sortBy === 'name-asc') {
        return String(a?.name || '').localeCompare(String(b?.name || ''));
      }

      return 0;
    });

    return result;
  }, [toursList, searchTerm, sortBy]);

  return (
    <>
      <Header />

        <main className="flex-1 max-w-[1200px] mx-auto px-6 py-12 w-full">
        <div className="text-center mb-12">
            <p className="text-[#39bf00] font-semibold uppercase tracking-[0.2em] mb-3">
                Explore Adventures
            </p>

            <h1 className="text-4xl md:text-5xl font-bold mb-4">
                Find Your Perfect Tour
            </h1>

            <p className="text-gray-500 text-lg max-w-2xl mx-auto">
                Search destinations, compare prices, and discover your next unforgettable trip.
            </p>
            </div>

            <div className="max-w-[1000px] mx-auto mb-14 bg-white border border-gray-200 rounded-3xl shadow-md p-4 md:p-5">
            <div className="grid grid-cols-1 md:grid-cols-[1fr_240px] gap-4">
                <div className="relative">
                <span className="absolute left-5 top-1/2 -translate-y-1/2 text-gray-400 text-xl">
                    <SearchIcon className='w-5'/>
                </span>

                <input
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    placeholder="Search Tbilisi, Rome, hiking..."
                    className="w-full border border-gray-300 rounded-2xl pl-14 pr-5 py-4 text-lg outline-none focus:border-[#39bf00] focus:ring-2 focus:ring-[#8dfc9c]/50 transition-all"
                />
                </div>

                <select
                value={sortBy}
                onChange={(e) => setSortBy(e.target.value)}
                className="w-full border border-gray-300 rounded-2xl px-5 py-4 text-lg outline-none bg-white focus:border-[#39bf00] focus:ring-2 focus:ring-[#8dfc9c]/50 transition-all cursor-pointer"
                >
                <option value="price-asc">Price: low to high</option>
                <option value="price-desc">Price: high to low</option>
                <option value="name-asc">Name A-Z</option>
                </select>
            </div>
            </div>

        {isLoading ? (
          <div className="flex justify-center items-center h-64">
            <p className="text-xl text-slate-500 font-semibold">
              Loading tours...
            </p>
          </div>
        ) : error ? (
          <div className="text-center mt-10">
            <p className="text-red-500 text-lg">{error}</p>
          </div>
        ) : filteredTours.length === 0 ? (
          <div className="text-center mt-10">
            <p className="text-gray-500 text-lg">
              No tours found.
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-[repeat(auto-fill,minmax(250px,1fr))] gap-15 justify-items-center">
            {filteredTours.map((tour, index) => (
              <TourCard
                key={tour?.id || index}
                id={tour?.id}
                imageUrl={tour?.images?.[0]?.url}
                title={tour?.name}
                description={tour?.description}
                price={tour?.currentPrice}
                city={tour?.startingCity}
                country={tour?.startingCountry}
              />
            ))}
          </div>
        )}
      </main>

      <Footer />
    </>
  );
};

export default TourPage;