import React from 'react';
import { formatDate } from '../lib/formatDate';
import CheckmarkIcon from '../assets/circle-check-big.svg?react';

const TourBookingCard = ({ tour, onBookTour, isBooking, bookingMessage, bookingError }) => {
  const startDate = tour?.itinerary?.[0]?.estimatedArrivalDate;
  const endDate = tour?.itinerary?.[0]?.estimatedDepartureDate;
  const hasTravelAgent = Boolean(tour?.assignedTravelAgentId);

  return (
    <div className='w-full lg:w-[40%] xl:w-[35%] p-5 md:p-7 border border-gray-300 rounded-2xl h-fit'>
      <p className='font-bold text-2xl mb-5'>From ${tour?.currentPrice} Per Person</p>

      <div className='border border-gray-300 flex justify-center rounded-xl text-sm md:text-base'>
        <div className='border-r border-r-gray-300 w-[50%] text-center p-2 md:p-3'>
          <p className='font-semibold'>Start Date:</p>
          <p>{formatDate(startDate)}</p>
        </div>
        <div className='w-[50%] text-center p-2 md:p-3'>
          <p className='font-semibold'>End Date:</p>
          <p>{formatDate(endDate)}</p>
        </div>
      </div>

      <button
        type="button"
        onClick={onBookTour}
        disabled={isBooking || !hasTravelAgent}
        className='cursor-pointer w-full rounded-xl bg-[#8dfc9c] px-3 py-3 my-5 hover:bg-[#39bf00] font-semibold transition-all duration-300 disabled:opacity-60 disabled:cursor-not-allowed'
      >
        {isBooking ? 'Booking...' : 'Book Tour'}
      </button>

      {!hasTravelAgent && (
        <p className='text-red-500 font-semibold mb-4'>
          No travel agent assigned to this tour yet.
        </p>
      )}

      {bookingMessage && (
        <p className='text-green-600 font-semibold mb-4'>{bookingMessage}</p>
      )}

      {bookingError && (
        <p className='text-red-500 font-semibold mb-4'>{bookingError}</p>
      )}

      <p className='text-lg md:text-xl font-semibold mt-2'>Additional Details:</p>

      <p className='font-semibold mt-2'>
        Guide: <span className='font-normal'>{tour?.assignedTourGuideFullName ?? "No guide assigned"}</span>
      </p>

      <p className='font-semibold mt-1'>
        Travel Agent: <span className='font-normal'>{tour?.assignedTravelAgentFullName ?? "No travel agent assigned"}</span>
      </p>

      <p className='font-semibold mt-1'>
        Hotel: <span className='font-normal'>{tour?.itinerary?.[0]?.hotelName ?? "Hotel is not included. You are responsible for finding one."}</span>
      </p>

      <div className='bg-[#8dfc9c]/30 p-4 md:p-5 rounded-xl mt-6 md:mt-8'>
        <div className='flex gap-3 items-start'>
          <CheckmarkIcon className="shrink-0 mt-1" />
          <p className="text-sm md:text-base">
            <span className='font-bold'>Free cancellation</span> up to 24 hours before the experience starts local time
          </p>
        </div>
      </div>
    </div>
  );
};

export default TourBookingCard;