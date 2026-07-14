import { useContext } from "react"
import { TourBookmarkContext } from "../contexts/TourBookmarkContext";

export const useTourBookmark = () => {
    const context = useContext(TourBookmarkContext);

    if (!context)
        throw new Error("useTourBookmark should be used inside TourBookmarkProvider");

    return context;
}