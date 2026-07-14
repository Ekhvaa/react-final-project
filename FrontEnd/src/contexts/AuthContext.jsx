import { createContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { useAuthRequest } from "../hooks/useAuthRequest";

export const AuthContext = createContext(null);

const API_BASE_URL = "https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net";
const loginUrl = `${API_BASE_URL}/api/auth/login`;
const registerUrl = `${API_BASE_URL}/api/auth/register`;

export const AuthProvider = ({ children }) => {
    const navigate = useNavigate();

    const [token, setToken] = useState(localStorage.getItem("token"));
    const [user, setUser] = useState(null);
    const [errorMessage, setErrorMessage] = useState('');
    const [isLoadingAuth, setIsLoadingAuth] = useState(true);

    const isAuthenticated = Boolean(token && user);

    useEffect(() => {
        const loadCurrentUser = async () => {
            const savedToken = localStorage.getItem("token");

            if (!savedToken) {
                setToken(null);
                setUser(null);
                setIsLoadingAuth(false);
                return;
            }

            try {
                const response = await axios.get(`${API_BASE_URL}/api/auth/me`, {
                    headers: {
                        Authorization: `Bearer ${savedToken}`,
                        Accept: "application/json"
                    }
                });

                setToken(savedToken);
                setUser(response.data);
            }
            catch {
                localStorage.removeItem("token");
                localStorage.removeItem("refreshToken");
                setToken(null);
                setUser(null);
            }
            finally {
                setIsLoadingAuth(false);
            }
        };

        loadCurrentUser();
    }, []);

    const login = async (username, password) => {
        const authData = await useAuthRequest(
            loginUrl,
            { username, password },
            setErrorMessage,
            setToken,
            navigate
        );

        if (authData) {
            setUser({
                userId: authData.userId,
                username: authData.username,
                role: authData.role
            });
        }
    };

    const register = async (registrationData) => {
        const authData = await useAuthRequest(
            registerUrl,
            registrationData,
            setErrorMessage,
            setToken,
            navigate
        );

        if (authData) {
            setUser({
                userId: authData.userId,
                username: authData.username,
                role: authData.role
            });
        }
    };

    const logout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");

        setToken(null);
        setUser(null);

        navigate('/login');
    };

    return (
        <AuthContext.Provider
            value={{
                errorMessage,
                token,
                user,
                username: user?.username ?? null,
                isAuthenticated,
                isLoadingAuth,
                login,
                logout,
                register
            }}
        >
            {children}
        </AuthContext.Provider>
    );
};