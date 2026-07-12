import axios from "axios";

export const useAuthRequest = async (url, data, setErrorMessage, setToken, navigate) => {
    console.log(data);
    try {
        const response = await axios.post(url, data);
        const token = await response.data.token;
        setToken(token);
        localStorage.setItem("token", token);
        navigate('/');
    }
    catch (error) {
        if (error.response && error.response.data) {
            setErrorMessage(error.response.data.message);
        } else {
            setErrorMessage("Network error. Please try again later.");
        }
    }
}