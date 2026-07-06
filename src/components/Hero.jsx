import React, { useEffect, useState } from 'react'
import heroImage1 from '../assets/HeroImage1.jpg'
import heroImage2 from '../assets/HeroImage2.jpg'
import heroImage3 from '../assets/HeroImage3.jpg'

const heroImages = [heroImage1, heroImage2, heroImage3];

const Hero = () => {
    const [currentIndex, setCurrentIndex] = useState(0);

    useEffect(() => {
        const interval = setInterval(() => {
            setCurrentIndex((prevIndex) => (prevIndex + 1) % heroImages.length);
        }, 5000);

        return () => clearInterval(interval);
    }, [])

  return (
    <>
        <div 
            className='w-full h-100 bg-cover bg-center bg-no-repeat flex items-center justify-center transition-all duration-1000'
            style={{ backgroundImage: `url(${heroImages[currentIndex]})` }}>
        </div>
    </>
  )
}

export default Hero