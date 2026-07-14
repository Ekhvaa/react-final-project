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

const API_BASE_URL =
  'https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net';

const HomePage = () => {
  const apiUrl = `${API_BASE_URL}/api/tours`;
  const { data: tours, isLoading, error } = useFetch(apiUrl, (resData) => resData?.items || []);

  const toursList = Array.isArray(tours) ? tours : [];

  return (
    <>
      <Header />
      <Hero />

      <main className="max-w-[1700px] mx-auto px-6 py-12">
        {/* your existing sections */}

        {!isLoading && !error && (
          <div className="grid grid-cols-[repeat(auto-fill,minmax(250px,1fr))] gap-15 justify-items-center">
            {toursList.map((tour, index) => {
              const imageUrl = tour?.images?.[0]?.url
                ? `${API_BASE_URL}${tour.images[0].url}`
                : null;

              return (
                <TourCard
                  key={tour?.id || index}
                  id={tour?.id}
                  imageUrl={imageUrl}
                  title={tour?.name}
                  description={tour?.description}
                  price={tour?.currentPrice}
                  city={tour?.startingCity}
                  country={tour?.startingCountry}
                />
              );
            })}
          </div>
        )}
      </main>

      <Footer />
    </>
  );
};

export default HomePage;