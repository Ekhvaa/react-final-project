import axios from "axios";
import { createContext, useState } from "react";
import { useNavigate } from "react-router-dom";

export const AuthContext = createContext(null);

export const AuthProvider = ( {children} ) => {
    const navigate = useNavigate();
    const [token, setToken] = useState(localStorage.getItem("token"));
    const [errorMessage, setErrorMessage] = useState('');

    const login = async (username, password) => {
        const url = "https://tourist-platform-api-f9g6c9azezbvehf0.italynorth-01.azurewebsites.net/api/auth/login";
        try{
            const response = await axios.post(url, {username, password});
            const token = await response.data.token;
            localStorage.setItem("token", token);
            navigate('/');
        }
        catch{
            setErrorMessage(response.data.message);
        }   
    }

    const logout = () => localStorage.removeItem("token");

    const register = ()

    return (
        <AuthContext.Provider value={{errorMessage, token, login, logout}}>
            {children}
        </AuthContext.Provider>
    )
}