import React from 'react'
import phone from '../assets/Phone.svg'

const ValuePropositionCard = ( {props} ) => {
  return (
    <div className='flex flex-col items-center gap-2 max-w-120 text-center'>
        <img src={props.icon} alt="icon" className='w-10'/>
        <h3 className='font-bold'>{props.title}</h3>
        <p>{props.description}</p>
    </div>
  )
}

export default ValuePropositionCard