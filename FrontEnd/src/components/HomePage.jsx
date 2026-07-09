import React from 'react'
import Header from './Header'
import Hero from './Hero'
import TourCard from './TourCard'
import ValuePropositionCard from './ValuePropositionCard'
import phone from '../assets/Phone.svg'
import map from '../assets/Guide.svg'
import money from '../assets/Money.svg'
import refund from '../assets/Refund.svg'
import { useNavigate } from 'react-router-dom'

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
  return (
    <>
      <Header />
      <Hero />
      <main className="max-w-[1700px] mx-auto px-6 py-12">
        <h2 className='text-center text-3xl font-bold mb-18'>Why Choose Us?</h2>
        <div className='grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-40 justify-items-center mb-20'>
          {valuePropositionValues.map((item) => <ValuePropositionCard props={item}/>)}
        </div>
        <h2 className="text-3xl text-center font-bold mb-18 text-gray-900">Trending Tours</h2>
        <div className="grid grid-cols-[repeat(auto-fill,minmax(250px,1fr))] gap-15 justify-items-center">
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