import React from 'react';

const TourOverview = ({ description }) => (
  <div className='border-b border-b-gray-300 py-8'>
    <h2 className='font-bold text-3xl mb-8'>Overview:</h2>
    <p className='text-xl'>{description || 'No description available for this tour yet.'}</p>
  </div>
);

export default TourOverview;