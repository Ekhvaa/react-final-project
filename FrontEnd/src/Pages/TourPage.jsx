import React from 'react';
import Header from '../components/Header';
import Searchbar from '../components/Searchbar';
import useFetch from '../hooks/useFetch';
import TourCard from '../components/TourCard';
import Footer from '../components/Footer';
import { useSearchParams } from 'react-router-dom';

const TourPage = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const currentQuery = searchParams.get('search') || '';

  const apiUrl = `https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net/api/tours?Keyword=${currentQuery}`;
  const { data: tours, isLoading, error } = useFetch(apiUrl, (resData) => resData?.items || []);
  
  const handleSearchSubmit = (newQuery) => {
    if (newQuery.trim() === '') {
      setSearchParams({});
    } else {
      setSearchParams({ search: newQuery });
    }
  };

  return (
    <div>
      <Header />
      <main className="max-w-[1700px] mx-auto px-5 py-10">
        <Searchbar value={currentQuery} onChange={handleSearchSubmit} styleMod={"w-full"}/>
        <h2 className='text-3xl font-bold mt-10'>
          {currentQuery == '' ? "All Tours:" : `Search Results for ${currentQuery}:`}
        </h2>
        {isLoading ? (
          <div className="flex justify-center items-center h-64">
            <p className="text-xl text-slate-500 font-semibold">Searching tours...</p>
          </div>
        ) : error ? (
          <div className="text-center mt-10">
            <p className="text-red-500 text-lg">{error}</p>
          </div>
        ) : (
          <div className="grid grid-cols-[repeat(auto-fill,minmax(250px,1fr))] gap-15 justify-items-center mt-8">
            {tours && tours.length > 0 ? (
              tours.map((tour, index) => (
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
              ))
            ) : (
              <p className="text-center col-span-full text-slate-500">
                No tours found matching "{currentQuery}".
              </p>
            )}
          </div>
        )}
      </main>
      <Footer />
    </div>
  );
}

export default TourPage;