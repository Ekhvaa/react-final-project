import axios from "axios";

export const useAuthRequest = async (url, data, setErrorMessage, setToken, navigate) => {
    try {
        const response = await axios.post(url, data);
        const authData = response.data;

        setToken(authData.token);
        localStorage.setItem("token", authData.token);

        if (authData.refreshToken) {
            localStorage.setItem("refreshToken", authData.refreshToken);
        }

        navigate('/');

        return authData;
    }
    catch (error) {
        if (error.response && error.response.data) {
            setErrorMessage(
                error.response.data.message ||
                error.response.data.detail ||
                "Authentication failed."
            );
        } else {
            setErrorMessage("Network error. Please try again later.");
        }

        return null;
    }
};