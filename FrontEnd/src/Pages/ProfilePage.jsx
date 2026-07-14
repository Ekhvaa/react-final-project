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
    const [isLoading, setIsLoading] = useState(false);
    const [errorMessage, setErrorMessage] = useState("");

    useEffect(() => {
        const loadTouristProfile = async () => {
            if (!token || user?.role !== "Tourist") {
                return;
            }

            try {
                setIsLoading(true);

                const response = await axios.get(`${API_BASE_URL}/api/users/me`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                        Accept: "application/json",
                    },
                });

                setProfile(response.data);
            } catch {
                setErrorMessage("Could not load profile details.");
            } finally {
                setIsLoading(false);
            }
        };

        loadTouristProfile();
    }, [token, user?.role]);

    return (
        <>
            <Header />

            <main className="max-w-[1200px] mx-auto px-6 py-12">
                <h1 className="text-3xl font-bold mb-6">Profile</h1>

                <div className="border border-gray-300 rounded-2xl p-6 max-w-xl">
                    {isLoading && (
                        <p className="text-gray-500">Loading profile...</p>
                    )}

                    {errorMessage && (
                        <p className="text-red-500 mb-4">{errorMessage}</p>
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
            </main>

            <Footer />
        </>
    );
}