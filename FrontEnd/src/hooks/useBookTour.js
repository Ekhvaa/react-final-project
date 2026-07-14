import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './useAuth';

const API_BASE_URL = 'https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net';

export function useBookTour(tourId, tour) {
  const navigate = useNavigate();
  const { token, user } = useAuth();

  const [bookingMessage, setBookingMessage] = useState('');
  const [bookingError, setBookingError] = useState('');
  const [isBooking, setIsBooking] = useState(false);

  const bookTour = async () => {
    setBookingMessage('');
    setBookingError('');

    if (!token) {
      navigate('/login');
      return;
    }

    if (user?.role !== 'Tourist') {
      setBookingError('Only tourists can book tours.');
      return;
    }

    if (!tour?.assignedTravelAgentId) {
      setBookingError('This tour does not have a travel agent assigned yet.');
      return;
    }

    try {
      setIsBooking(true);

      const response = await fetch(`${API_BASE_URL}/api/bookings`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          tourId: Number(tourId),
          travelAgentId: tour.assignedTravelAgentId,
        }),
      });

      if (!response.ok) {
        const errorText = await response.text();
        let errorMessage = 'Could not book this tour.';

        try {
          const errorData = JSON.parse(errorText);
          errorMessage = errorData?.detail || errorData?.message || errorMessage;
        } catch {
          if (errorText) {
            errorMessage = errorText;
          }
        }

        setBookingError(errorMessage);
        return;
      }

      setBookingMessage('Tour booked successfully!');
    } catch {
      setBookingError('Network error. Please try again.');
    } finally {
      setIsBooking(false);
    }
  };

  return { bookTour, bookingMessage, bookingError, isBooking };
}