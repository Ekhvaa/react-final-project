import React from 'react'
import Header from './Header'
import Hero from './Hero'
import TourCard from './TourCard'

const HomePage = () => {
  return (
    <>
      <Header />
      <Hero />
{/* Page Wrapper */}
      <main className="max-w-[1700px] mx-auto px-6 py-12">
        <h2 className="text-3xl text-center font-bold mb-18 text-gray-900">Trending Tours</h2>
        <div className="grid grid-cols-[repeat(auto-fill,minmax(250px,1fr))] gap-8">
          <TourCard />
          <TourCard />
          <TourCard />
          <TourCard />
          <TourCard />
          <TourCard />
        </div>
      </main>
    </>
  )
}

export default HomePage