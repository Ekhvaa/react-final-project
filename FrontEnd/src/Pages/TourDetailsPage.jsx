import React from 'react'
import Header from '../components/Header'
import { useParams } from 'react-router-dom'
import useFetch from '../hooks/useFetch';
import Footer from '../components/Footer';
import TourImage from '../components/TourImage';
import TourBookingCard from '../components/TourBookingCard';
import SimilarTours from '../components/SimilarTours';
import TourOverview from '../components/TourOverview';
import { useBookTour } from '../hooks/useBookTour';

const API_BASE_URL = 'https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net';

const getFullImageUrl = (imageUrl) => {
  if (!imageUrl) {
    return null;
  }

  if (imageUrl.startsWith('http')) {
    return imageUrl;
  }

  return `${API_BASE_URL}${imageUrl}`;
};

const TourDetailsPage = () => {
  const { id } = useParams();

  const tourDetailsEndpointUrl = `${API_BASE_URL}/api/tours/${id}`;
  const suggestedToursEndpointUrl = `${API_BASE_URL}/api/tours?PageSize=4`;

  const { data: tour, isLoading: tourIsLoading, error: tourError } = useFetch(tourDetailsEndpointUrl);
  const { data: suggestedTours } = useFetch(suggestedToursEndpointUrl, (resData) => resData?.items || []);

  const { bookTour, bookingMessage, bookingError, isBooking } = useBookTour(id, tour);

  if (tourIsLoading || !tour) {
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

  const mainImageUrl = getFullImageUrl(tour?.images?.[0]?.url);

  return (
    <div>
      <Header />

      <main className='max-w-[1700px] px-4 py-8 md:p-10 mx-auto flex flex-col items-center w-full'>
        <div className='flex flex-col gap-8 md:gap-10 w-full'>

          <h1 className='font-bold text-3xl md:text-4xl pb-2 md:pb-7'>{tour?.name}</h1>

          <div className='flex flex-col lg:flex-row gap-8 lg:gap-10 w-full'>
            <TourImage imageUrl={mainImageUrl} />
            <TourBookingCard
              tour={tour}
              onBookTour={bookTour}
              isBooking={isBooking}
              bookingMessage={bookingMessage}
              bookingError={bookingError}
            />
          </div>

          <SimilarTours tours={suggestedTours || []} />

          <TourOverview description={tour?.description} />
        </div>
      </main>

      <Footer />
    </div>
  )
}

export default TourDetailsPage;