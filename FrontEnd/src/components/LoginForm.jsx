import React, { useState } from 'react';
import { inputClass } from '../lib/formStyles';
import { useAuth } from '../hooks/useAuth';

const initialLoginState = { username: '', password: '' };

export default function LoginForm() {
    const [loginData, setLoginData] = useState(initialLoginState);
    const [loginErrors, setLoginErrors] = useState({});
    const { errorMessage, login } = useAuth();

    const handleLoginChange = (e) => {
        const { name, value } = e.target;
        setLoginData(prev => ({ ...prev, [name]: value }));
        if (loginErrors[name]) {
            setLoginErrors(prev => ({ ...prev, [name]: undefined }));
        }
    };

    const validateLogin = () => {
        const errors = {};
        if (!loginData.username.trim()) errors.username = 'Username is required.';
        if (!loginData.password) errors.password = 'Password is required.';
        return errors;
    };

    const handleLoginSubmit = async (e) => {
        e.preventDefault();

        const errors = validateLogin();
        setLoginErrors(errors);

        if (Object.keys(errors).length === 0)
            await login(loginData.username, loginData.password);
    };

    return (
        <form className="flex flex-col gap-4" onSubmit={handleLoginSubmit} noValidate>
            <div>
                <label htmlFor="username" className="block text-sm font-medium text-slate-700 mb-1">
                    Username
                </label>
                <input
                    id="username"
                    name="username"
                    type="text"
                    autoComplete="username"
                    value={loginData.username}
                    onChange={handleLoginChange}
                    className={inputClass(loginErrors.username)}
                    placeholder="your_username"
                />
                {loginErrors.username && (
                    <p className="mt-1 text-sm text-red-500">{loginErrors.username}</p>
                )}
            </div>
            <div>
                <label htmlFor="password" className="block text-sm font-medium text-slate-700 mb-1">
                    Password
                </label>
                <input
                    id="password"
                    name="password"
                    type="password"
                    autoComplete="current-password"
                    value={loginData.password}
                    onChange={handleLoginChange}
                    className={inputClass(loginErrors.password)}
                    placeholder="••••••••"
                />
                {loginErrors.password && (
                    <p className="mt-1 text-sm text-red-500">{loginErrors.password}</p>
                )}
            </div>

            <button
                type="submit"
                className="mt-2 w-full rounded-lg bg-[#8dfc9c] text-slate-900 font-semibold py-2.5 hover:opacity-90 transition-opacity"
            >
                Log In
            </button>
        </form>
    );
}