import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Header from '../components/Header';
import Hero from '../components/Hero';
import TourCard from '../components/TourCard';
import ValuePropositionCard from '../components/ValuePropositionCard';
import Footer from '../components/Footer';
import phone from '../assets/Phone.svg';
import map from '../assets/Guide.svg';
import money from '../assets/Money.svg';
import refund from '../assets/Refund.svg';
import useFetch from '../hooks/useFetch';

const valuePropositionValues = [
  {
    icon: phone,
    title: "24/7 Global Support",
    description: "Our dedicated travel support team is available around the clock to assist you. Whether you need to adjust your itinerary mid-trip or have an urgent question on the ground, we are only a text or call away."
  },
  {
    icon: map,
    title: "Expert Local Guides",
    description: "Our tours are led by certified, passionate local experts who know the hidden stories, secret spots, and authentic flavors of their hometowns."
  },
  {
    icon: money,
    title: "Best Price Guarantee",
    description: "We work directly with local operators to cut out the middleman, ensuring you get the absolute lowest price for your adventure."
  },
  {
    icon: refund,
    title: "Flexible Cancellation",
    description: "Enjoy free cancellation and full refunds on most experiences up to 24 hours before your tour begins."
  }
];

const HomePage = () => {
  const apiUrl = 'https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net/api/tours';
  const { data: tours, isLoading, error } = useFetch(apiUrl, (resData) => resData?.items || []);

  return (
    <>
      <Header />
      <Hero />
      <main className="flex-1 max-w-[1200px] mx-auto px-6 py-12 w-full">
        <h2 className='text-center text-3xl font-bold mb-18'>Why Choose Us?</h2>
        <div className='grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-40 justify-items-center mb-20'>
          {valuePropositionValues.map((item, index) => (
            <ValuePropositionCard key={index} props={item} />
          ))}
        </div>
        <h2 className="text-3xl text-center font-bold mb-18 text-gray-900">Trending Tours</h2>
        {isLoading ? (
          <div className="flex justify-center items-center h-64">
            <p className="text-xl text-slate-500 font-semibold">Loading amazing adventures...</p>
          </div>
        ) : error ? (
          <div className="text-center mt-10">
            <p className="text-red-500 text-lg">{error}</p>
          </div>
        ) : (
          <div className="grid grid-cols-[repeat(auto-fill,minmax(250px,1fr))] gap-15 justify-items-center">
            {tours.map((tour, index) => (
              <TourCard 
                key={tour.id || index}
                id={tour.id}
                imageUrl={tour.images[0]?.url}
                title={tour.name}
                description={tour.description}
                price={tour.currentPrice} 
                city={tour.startingCity} 
                country={tour.startingCountry}
              />
            ))}
          </div>
        )}
      </main>
      <Footer />
    </>
  );
}

export default HomePage;