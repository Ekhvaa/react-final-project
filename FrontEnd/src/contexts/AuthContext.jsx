import { createContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthRequest } from "../hooks/useAuthRequest";

export const AuthContext = createContext(null);
const loginUrl = "https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net/api/auth/login";
const registerUrl = "https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net/api/auth/register";

export const AuthProvider = ( {children} ) => {
    const navigate = useNavigate();
    const [token, setToken] = useState(localStorage.getItem("token"));
    const [errorMessage, setErrorMessage] = useState('');

    const login = async (username, password) => 
        useAuthRequest(loginUrl, { username, password }, setErrorMessage, setToken, navigate);

    const logout = () => {
        localStorage.removeItem("token");
        setToken(null);
    };

    const register = async (registrationData) => 
        useAuthRequest(registerUrl, registrationData, setErrorMessage, setToken, navigate);

    return (
        <AuthContext.Provider value={{errorMessage, token, login, logout, register}}>
            {children}
        </AuthContext.Provider>
    )
}