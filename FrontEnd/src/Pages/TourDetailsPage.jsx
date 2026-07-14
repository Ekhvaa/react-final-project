import React, { useState } from 'react'
import Header from '../components/Header'
import { useNavigate, useParams } from 'react-router-dom'
import useFetch from '../hooks/useFetch';
import { formatDate } from '../lib/formatDate';
import CheckmarkIcon from '../assets/circle-check-big.svg?react'
import TourCard from '../components/TourCard';
import Footer from '../components/Footer';
import { useAuth } from '../hooks/useAuth';

const API_BASE_URL = 'https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net';

// Change this if your real TravelAgent id is different.
const DEFAULT_TRAVEL_AGENT_ID = 5;

const TourDetailsPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { token, user } = useAuth();

  const [bookingMessage, setBookingMessage] = useState('');
  const [bookingError, setBookingError] = useState('');
  const [isBooking, setIsBooking] = useState(false);

  const tourDetailsEndpointUrl = `${API_BASE_URL}/api/tours/${id}`;
  const suggestedToursEndpointUrl = `${API_BASE_URL}/api/tours?PageSize=4`;

  const { data: tour, isLoading: tourIsLoading, error: tourError } = useFetch(tourDetailsEndpointUrl);
  const { data: suggestedTours } = useFetch(suggestedToursEndpointUrl, (resData) => resData?.items || []);

  const handleBookTour = async () => {
    setBookingMessage('');
    setBookingError('');

    if (!token) {
      navigate('/login');
      return;
    }

    if (user?.role !== 'Tourist') {
      setBookingError('Only tourists can book tours.');
      return;
    }

    try {
      setIsBooking(true);

      const response = await fetch(`${API_BASE_URL}/api/bookings`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          tourId: Number(id),
          travelAgentId: DEFAULT_TRAVEL_AGENT_ID,
        }),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);

        setBookingError(
          errorData?.detail ||
          errorData?.message ||
          'Could not book this tour.'
        );

        return;
      }

      setBookingMessage('Tour booked successfully!');
    } catch {
      setBookingError('Network error. Please try again.');
    } finally {
      setIsBooking(false);
    }
  };

  if (tourIsLoading || !tour || !suggestedTours) {
    return (
      <div>
        <Header />
        <main className="flex justify-center items-center h-96">
          <p className="text-xl text-slate-500 font-semibold">Loading adventure details...</p>
        </main>
      </div>
    );
  }

  if (tourError) {
    return (
      <div>
        <Header />
        <main className="flex justify-center items-center h-96">
          <p className="text-xl text-red-500 font-semibold">Failed to load this tour option.</p>
        </main>
      </div>
    );
  }

  return (
    <div>
      <Header />

      <main className='max-w-[1700px] px-4 py-8 md:p-10 mx-auto flex flex-col items-center w-full'>
        <div className='flex flex-col gap-8 md:gap-10 w-full'>

          <h1 className='font-bold text-3xl md:text-4xl pb-2 md:pb-7'>{tour?.name}</h1>

          <div className='flex flex-col lg:flex-row gap-8 lg:gap-10 w-full'>

            <div className='w-full lg:w-[60%] xl:w-[65%] rounded-2xl overflow-hidden'>
              <img
                src={`${API_BASE_URL}${tour?.images?.[0]?.url}`}
                alt="TourImage"
                className='w-full h-64 md:h-96 lg:h-full object-cover rounded-2xl'
              />
            </div>

            <div className='w-full lg:w-[40%] xl:w-[35%] p-5 md:p-7 border border-gray-300 rounded-2xl h-fit'>
              <p className='font-bold text-2xl mb-5'>From ${tour?.currentPrice} Per Person</p>

              <div className='border border-gray-300 flex justify-center rounded-xl text-sm md:text-base'>
                <div className='border-r border-r-gray-300 w-[50%] text-center p-2 md:p-3'>
                  <p className='font-semibold'>Start Date:</p>
                  <p>{formatDate(tour?.itinerary?.[0]?.estimatedArrivalDate)}</p>
                </div>
                <div className='w-[50%] text-center p-2 md:p-3'>
                  <p className='font-semibold'>End Date:</p>
                  <p>{formatDate(tour?.itinerary?.[0]?.estimatedDepartureDate)}</p>
                </div>
              </div>

              <button
                type="button"
                onClick={handleBookTour}
                disabled={isBooking}
                className='cursor-pointer w-full rounded-xl bg-[#8dfc9c] px-3 py-3 my-5 hover:bg-[#39bf00] font-semibold transition-all duration-300 disabled:opacity-60 disabled:cursor-not-allowed'
              >
                {isBooking ? 'Booking...' : 'Book Tour'}
              </button>

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
                Hotel: <span className='font-normal'>{tour?.itinerary?.[0]?.hotelName ?? "Hotel is not included. You are responsible for finding one."}</span>
              </p>

              <div className='bg-[#8dfc9c]/30 p-4 md:p-5 rounded-xl mt-6 md:mt-8'>
                <div className='flex gap-3 items-start'>
                  <CheckmarkIcon className="shrink-0 mt-1" />
                  <p className="text-sm md:text-base">
                    <span className='font-bold'>Free cancellation</span> up to 24 hours before the experience starts (local time)
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div className='border-y border-y-gray-300 py-8 md:py-10 mt-4'>
            <h2 className='font-bold text-2xl md:text-3xl mb-6 md:mb-10'>Similar Tours:</h2>

            <div className='grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 md:gap-10 w-full justify-items-center'>
              {suggestedTours.map((suggestedTour, index) => (
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

          <div className='border-b border-b-gray-300 py-8'>
            <h2 className='font-bold text-3xl mb-8'>Overview:</h2>
            <p className='text-xl'>{tour?.description}</p>
          </div>
        </div>
      </main>

      <Footer />
    </div>
  )
}

export default TourDetailsPage;