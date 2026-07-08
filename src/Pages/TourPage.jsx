import React from 'react'
import Header from '../components/Header'
import Searchbar from '../components/Searchbar'
import { useSearchParams } from 'react-router-dom'

const TourPage = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const currentQuery = searchParams.get('search') || '';

  return (
    <div>
      <Header />
      <Searchbar value={currentQuery} searchbarWidth={'[60%]'}/>
    </div>
  )
}

export default TourPage