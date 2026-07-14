import { createContext, useEffect, useState } from "react";

export const TourBookmarkContext = createContext(null);

export const TourBookmarkProvider = ({ children }) => {
    const [bookmarkedTours, setBookmarkedTours] = useState(() => {
        const localData = localStorage.getItem("bookmarkedTours");

        return localData 
            ? JSON.parse(localData)
            : [];
    });

    useEffect(() => {
        localStorage.setItem("bookmarkedTours", JSON.stringify(bookmarkedTours));
    }, [bookmarkedTours]);

    const toggleBookmark = (title) => {
        setBookmarkedTours((prev) => {
            return prev.includes(title)
                ? prev.filter((t) => t != title)
                : [...prev, title];
        });
    };

    const checkIsBookmarked = (title) => bookmarkedTours.includes(title);

    return (
        <TourBookmarkContext.Provider value={{ bookmarkedTours, toggleBookmark, checkIsBookmarked}}>
            {children}
        </TourBookmarkContext.Provider>
    );
}