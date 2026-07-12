import React, { useEffect } from 'react'
import photo from '../assets/TourCardPhoto.jpg'
import locationIcon from '../assets/LocationIcon.svg'
import starRating from '../assets/StarRating.svg'
import BookmarkedIcon from '../assets/bookmarkedIcon.svg?react'
import NotBookmarkedIcon from '../assets/notBookmarkedIcon.svg?react'
import { Link } from 'react-router-dom'
import { useTourBookmark } from '../hooks/useTourBookmark'


const TourCard = ( { id, title, price, city, country, imageUrl, description } ) => {
  const { bookmarkedTours, toggleBookmark, checkIsBookmarked} = useTourBookmark();

  const handleBookmarkClick = (e) => {
    e.preventDefault();
    e.stopPropagation();
    toggleBookmark(title);
  }

  return (
    <Link to={`/tours/${id}`} className='max-w-70 overflow-hidden cursor-pointer flex flex-col gap-2 rounded-3xl border border-gray-100 shadow-sm hover:shadow-xl transition-shadow duration-300'>
      <div className='relative'>
        <img src={`https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net${imageUrl}`} alt="TourPhoto" />
        <div className='absolute top-4 right-4 rounded-full p-3 bg-black/30 cursor-pointer' onClick={(e) => handleBookmarkClick(e)}>
          {checkIsBookmarked(title) ? <BookmarkedIcon /> : <NotBookmarkedIcon />}
        </div>
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