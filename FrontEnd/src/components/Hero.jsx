import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import heroImage1 from '../assets/HeroImage1.jpg'
import heroImage2 from '../assets/HeroImage2.jpg'
import heroImage3 from '../assets/HeroImage3.jpg'
import Searchbar from './Searchbar';

const heroImages = [heroImage1, heroImage2, heroImage3];

const Hero = () => {
    const [currentIndex, setCurrentIndex] = useState(0);
    const navigate = useNavigate();
      
    useEffect(() => {
        const interval = setInterval(() => {
            setCurrentIndex((prevIndex) => (prevIndex + 1) % heroImages.length);
        }, 5000);

        return () => clearInterval(interval);
    }, []);

    const handleChangeRedirect = (searchQuery) => {
        navigate(`/tours?search=${encodeURIComponent(searchQuery)}`)
    };

  return (
    <>
        <div className='relative w-full h-100 overflow-hidden bg-black flex items-center justify-center'>
            {heroImages.map((image, index) => (
                <img
                    key={index}
                    src={image}
                    alt={`Hero Slide ${index}`}
                    className={`absolute top-0 left-0 w-full h-full object-cover object-center transition-opacity duration-1000 ease-in-out ${
                        index == currentIndex ? 'opacity-100 z-10' : 'opacity-0 z-0'
                    }`}
                />
            ))}
            
            <div className='relative z-20 w-11/12 md:w-[40%] text-center text-white mt-10'>
                <h1 className='text-3xl md:text-5xl font-bold mb-4 drop-shadow-lg'>
                    Create new memories using COMPANYNAME
                </h1>
                <p className='text-lg md:text-xl font-light mb-8 drop-shadow-md'>
                    Find your favourite tours here
                </p>
                <Searchbar onChange={handleChangeRedirect}/>
            </div>
        </div>
    </>
  )
}

export default Hero