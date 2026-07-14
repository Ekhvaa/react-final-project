import React from 'react';
import TourCard from './TourCard';

const SimilarTours = ({ tours = [] }) => {
  if (!tours.length) {
    return null;
  }

  return (
    <div className='border-y border-y-gray-300 py-8 md:py-10 mt-4'>
      <h2 className='font-bold text-2xl md:text-3xl mb-6 md:mb-10'>Similar Tours:</h2>

      <div className='grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 md:gap-10 w-full justify-items-center'>
        {tours.map((suggestedTour, index) => (
          <TourCard
            key={suggestedTour?.id || index}
            id={suggestedTour?.id}
            imageUrl={suggestedTour?.images?.[0]?.url}
            title={suggestedTour?.name}
            description={suggestedTour?.description}
            price={suggestedTour?.currentPrice}
            city={suggestedTour?.startingCity}
            country={suggestedTour?.startingCountry}
          />
        ))}
      </div>
    </div>
  );
};

export default SimilarTours;