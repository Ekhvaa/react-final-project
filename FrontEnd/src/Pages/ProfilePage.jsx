import React, { useEffect, useState } from "react";
import axios from "axios";
import Header from "../components/Header";
import Footer from "../components/Footer";
import { useAuth } from "../hooks/useAuth";

const API_BASE_URL =
    "https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net";

export default function ProfilePage() {
    const { user, token } = useAuth();

    const [profile, setProfile] = useState(null);
    const [bookings, setBookings] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [cancellingBookingId, setCancellingBookingId] = useState(null);
    const [errorMessage, setErrorMessage] = useState("");
    const [successMessage, setSuccessMessage] = useState("");

    const getAuthHeaders = () => ({
        Authorization: `Bearer ${token}`,
        Accept: "application/json",
    });

    useEffect(() => {
        const loadTouristData = async () => {
            if (!token || user?.role !== "Tourist") {
                return;
            }

            try {
                setIsLoading(true);
                setErrorMessage("");
                setSuccessMessage("");

                const profileResponse = await axios.get(`${API_BASE_URL}/api/users/me`, {
                    headers: getAuthHeaders(),
                });

                const bookingsResponse = await axios.get(`${API_BASE_URL}/api/bookings/mine`, {
                    headers: getAuthHeaders(),
                });

                const bookingsData =
                    bookingsResponse.data?.items ||
                    bookingsResponse.data?.data ||
                    bookingsResponse.data ||
                    [];

                setProfile(profileResponse.data);
                setBookings(Array.isArray(bookingsData) ? bookingsData : []);
            } catch {
                setErrorMessage("Could not load profile details.");
            } finally {
                setIsLoading(false);
            }
        };

        loadTouristData();
    }, [token, user?.role]);

    const getBookingStatusName = (status) => {
        const statusMap = {
            0: "Pending",
            1: "Confirmed",
            2: "Cancelled",
            3: "Completed",
            4: "Rejected",
        };

        if (status === null || status === undefined) {
            return "Booked";
        }

        if (typeof status === "string" && isNaN(Number(status))) {
            return status;
        }

        return statusMap[Number(status)] ?? "Unknown";
    };

    const canCancelBooking = (status) => {
        const normalizedStatus = Number(status);

        return normalizedStatus === 0 || normalizedStatus === 1;
    };

    const handleCancelBooking = async (bookingId) => {
        const confirmed = window.confirm("Are you sure you want to remove this booking?");

        if (!confirmed) {
            return;
        }

        try {
            setCancellingBookingId(bookingId);
            setErrorMessage("");
            setSuccessMessage("");

            await axios.put(
                `${API_BASE_URL}/api/bookings/${bookingId}/cancel`,
                null,
                {
                    headers: getAuthHeaders(),
                }
            );

            setBookings((previousBookings) =>
                previousBookings.filter((booking) => booking.id !== bookingId)
            );

            setSuccessMessage("Booking removed successfully.");
        } catch (error) {
            const message =
                error.response?.data?.detail ||
                error.response?.data?.message ||
                "Could not remove booking.";

            setErrorMessage(message);
        } finally {
            setCancellingBookingId(null);
        }
    };

    return (
        <>
            <Header />

            <main className="flex-1 max-w-[1200px] mx-auto px-6 py-12 w-full">
                <h1 className="text-3xl font-bold mb-6">Profile</h1>

                {successMessage && (
                    <p className="text-green-600 font-semibold mb-4">
                        {successMessage}
                    </p>
                )}

                {errorMessage && (
                    <p className="text-red-500 font-semibold mb-4">
                        {errorMessage}
                    </p>
                )}

                <div className="border border-gray-300 rounded-2xl p-6 max-w-xl mb-8">
                    {isLoading && (
                        <p className="text-gray-500">Loading profile...</p>
                    )}

                    {user?.role === "Tourist" && profile ? (
                        <>
                            <p className="mb-2">
                                <span className="font-semibold">Username:</span>{" "}
                                {profile.username}
                            </p>

                            <p className="mb-2">
                                <span className="font-semibold">First name:</span>{" "}
                                {profile.firstName}
                            </p>

                            <p className="mb-2">
                                <span className="font-semibold">Last name:</span>{" "}
                                {profile.lastName}
                            </p>

                            <p className="mb-2">
                                <span className="font-semibold">Email:</span>{" "}
                                {profile.email}
                            </p>

                            <p className="mb-2">
                                <span className="font-semibold">Phone:</span>{" "}
                                {profile.contactPhone}
                            </p>

                            <p className="mb-2">
                                <span className="font-semibold">Gender:</span>{" "}
                                {profile.gender}
                            </p>

                            <p>
                                <span className="font-semibold">National ID:</span>{" "}
                                {profile.nationalId}
                            </p>
                        </>
                    ) : (
                        <>
                            <p className="mb-2">
                                <span className="font-semibold">Username:</span>{" "}
                                {user?.username}
                            </p>

                            <p>
                                <span className="font-semibold">Role:</span>{" "}
                                {user?.role}
                            </p>
                        </>
                    )}
                </div>

                {user?.role === "Tourist" && (
                    <section>
                        <h2 className="text-2xl font-bold mb-4">My Bookings</h2>

                        {isLoading ? (
                            <p className="text-gray-500">Loading bookings...</p>
                        ) : bookings.length === 0 ? (
                            <div className="border border-gray-300 rounded-2xl p-6">
                                <p className="text-gray-500">No booked tours yet.</p>
                            </div>
                        ) : (
                            <div className="grid gap-4">
                                {bookings.map((booking, index) => (
                                    <div
                                        key={booking?.id || `${booking?.tourCode}-${index}`}
                                        className="border border-gray-300 rounded-2xl p-5"
                                    >
                                        <h3 className="text-xl font-semibold mb-2">
                                            {booking?.tourName ?? "Unknown tour"}
                                        </h3>

                                        <p className="mb-1">
                                            <span className="font-semibold">Tour code:</span>{" "}
                                            {booking?.tourCode ?? "N/A"}
                                        </p>

                                        <p className="mb-1">
                                            <span className="font-semibold">Travel agent:</span>{" "}
                                            {booking?.travelAgentFullName ?? "N/A"}
                                        </p>

                                        <p className="mb-4">
                                            <span className="font-semibold">Status:</span>{" "}
                                            {getBookingStatusName(booking?.status)}
                                        </p>

                                        {canCancelBooking(booking?.status) && (
                                            <button
                                                type="button"
                                                onClick={() => handleCancelBooking(booking.id)}
                                                disabled={cancellingBookingId === booking.id}
                                                className="rounded-xl bg-red-500 text-white px-4 py-2 font-semibold hover:bg-red-600 transition-all duration-300 disabled:opacity-60 disabled:cursor-not-allowed"
                                            >
                                                {cancellingBookingId === booking.id
                                                    ? "Removing..."
                                                    : "Remove booking"}
                                            </button>
                                        )}
                                    </div>
                                ))}
                            </div>
                        )}
                    </section>
                )}
            </main>

            <Footer />
        </>
    );
}