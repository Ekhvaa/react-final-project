import React from 'react';

const TourImage = ({ imageUrl }) => (
  <div className='w-full lg:w-[60%] xl:w-[65%] rounded-2xl overflow-hidden'>
    {imageUrl ? (
      <img
        src={imageUrl}
        alt="TourImage"
        className='w-full h-64 md:h-96 lg:h-full object-cover rounded-2xl'
      />
    ) : (
      <div className='w-full h-64 md:h-96 lg:h-full rounded-2xl bg-gray-200 flex items-center justify-center'>
        <p className='text-gray-500 font-semibold'>No image available</p>
      </div>
    )}
  </div>
);

export default TourImage;