import { useState, useEffect } from 'react';
import axios from 'axios';

const useFetch = (url, transformFn = null) => {
  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!url) return;

    const fetchData = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const response = await axios.get(url);
        // If a transformer function is provided, use it. Otherwise, return raw response.data
        const processedData = transformFn ? transformFn(response.data) : response.data;
        setData(processedData);
      } catch (err) {
        console.error(`Error fetching from ${url}:`, err);
        setError(err.message || "Something went wrong");
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [url]);

  return { data, isLoading, error };
};

export default useFetch;