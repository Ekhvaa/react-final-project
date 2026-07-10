import React, { useEffect } from 'react'
import photo from '../assets/TourCardPhoto.jpg'
import locationIcon from '../assets/LocationIcon.svg'
import starRating from '../assets/StarRating.svg'
import { Link } from 'react-router-dom'


const TourCard = ( { id, title, price, city, country, imageUrl, description } ) => {
  return (
    <Link to={`/tours/${id}`} className='max-w-70 overflow-hidden cursor-pointer flex flex-col gap-2 rounded-3xl border border-gray-100 shadow-sm hover:shadow-xl transition-shadow duration-300'>
      <div>
        <img src={`https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net${imageUrl}`} alt="TourPhoto" />
      </div>
      <div className='px-4 pb-3 flex flex-col gap-2'>
        <div className='flex gap-2'>
          <img src={locationIcon} alt="location" />
          <span>{city}, {country}</span>
        </div>
        <p className='font-bold text-xl'>{title}</p>
        <div className='flex gap-2'>
          <img src={starRating} alt="" className='w-6'/>
          <span>4.8</span>
        </div>
        <div>
          <p>{description}</p>
        </div>
        <span className='font-bold text-2xl'>${price}</span>
      </div>
    </Link>

  )
}

export default TourCard