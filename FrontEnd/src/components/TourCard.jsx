import React, { useEffect } from 'react'
import photo from '../assets/TourCardPhoto.jpg'
import locationIcon from '../assets/LocationIcon.svg'
import starRating from '../assets/StarRating.svg'


const TourCard = ( { title, price, city, country } ) => {
  return (
    <div className='max-w-70 overflow-hidden cursor-pointer flex flex-col gap-2 rounded-3xl border border-gray-100 shadow-sm hover:shadow-xl transition-shadow duration-300'>
      <div>
        <img src={photo} alt="TourPhoto" />
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
          <p>dpljgndaso jfgdpao fgpojda fgpojda fpojda nfop daopfak jdgakda bgoda fopkiad opif adoip jfadopujf adopj hfoipjda hfoipade opfh </p>
        </div>
        <span className='font-bold text-2xl'>${price}</span>
      </div>
    </div>
  )
}

export default TourCard