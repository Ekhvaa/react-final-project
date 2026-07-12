import React, { useState } from 'react';
import { inputClass } from '../lib/formStyles';

const initialRegisterState = {
    username: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    email: '',
    contactPhone: '',
    dateOfBirth: '',
    gender: '',
    nationalId: ''
};

export default function RegistrationForm() {
    const [registerData, setRegisterData] = useState(initialRegisterState);
    const [registerErrors, setRegisterErrors] = useState({});

    const handleRegisterChange = (e) => {
        const { name, value } = e.target;
        setRegisterData(prev => ({ ...prev, [name]: value }));
        if (registerErrors[name]) {
            setRegisterErrors(prev => ({ ...prev, [name]: undefined }));
        }
    };

    const validateRegister = () => {
        const errors = {};

        if (!registerData.username.trim()) errors.username = 'Username is required.';

        if (!registerData.password) {
            errors.password = 'Password is required.';
        } else if (registerData.password.length < 8) {
            errors.password = 'Password must be at least 8 characters.';
        }

        if (!registerData.confirmPassword) {
            errors.confirmPassword = 'Please confirm your password.';
        }
        else if (registerData.confirmPassword !== registerData.password) {
            errors.confirmPassword = 'Passwords do not match.';
        }

        if (!registerData.firstName.trim())
            errors.firstName = 'First name is required.';
        if (!registerData.lastName.trim())
            errors.lastName = 'Last name is required.';

        if (!registerData.email.trim()) {
            errors.email = 'Email is required.';
        } 
        else if (!/^\S+@\S+\.\S+$/.test(registerData.email)) {
            errors.email = 'Enter a valid email address.';
        }

        if (!registerData.contactPhone.trim()) {
            errors.contactPhone = 'Phone number is required.';
        }

        if (!registerData.dateOfBirth) {
            errors.dateOfBirth = 'Date of birth is required.';
        }

        if (!registerData.gender) {
            errors.gender = 'Please select a gender.';
        }

        if (!registerData.nationalId.trim()) {
            errors.nationalId = 'National ID is required.';
        } else if (!/^\d{11}$/.test(registerData.nationalId.trim())) {
            errors.nationalId = 'National ID must be exactly 11 digits.';
        }

        return errors;
    };

    const handleRegisterSubmit = (e) => {
        e.preventDefault();
        const errors = validateRegister();
        setRegisterErrors(errors);
        if (Object.keys(errors).length === 0) {
            const payload = {
                username: registerData.username,
                password: registerData.password,
                firstName: registerData.firstName,
                lastName: registerData.lastName,
                email: registerData.email,
                contactPhone: registerData.contactPhone,
                dateOfBirth: new Date(registerData.dateOfBirth).toISOString(),
                gender: registerData.gender,
                nationalId: registerData.nationalId
            };
            console.log('Register payload:', payload);
        }
    };

    return (
        <form className="flex flex-col gap-4" onSubmit={handleRegisterSubmit} noValidate>
            <div>
                <label htmlFor="registerUsername" className="block text-sm font-medium text-slate-700 mb-1">
                    Username
                </label>
                <input
                    id="registerUsername"
                    name="username"
                    type="text"
                    autoComplete="username"
                    value={registerData.username}
                    onChange={handleRegisterChange}
                    aria-invalid={Boolean(registerErrors.username)}
                    className={inputClass(registerErrors.username)}
                    placeholder="your_username"
                />
                {registerErrors.username && (
                    <p className="mt-1 text-sm text-red-500">{registerErrors.username}</p>
                )}
            </div>

            <div className="grid grid-cols-2 gap-4">
                <div>
                    <label htmlFor="firstName" className="block text-sm font-medium text-slate-700 mb-1">
                        First name
                    </label>
                    <input
                        id="firstName"
                        name="firstName"
                        type="text"
                        autoComplete="given-name"
                        value={registerData.firstName}
                        onChange={handleRegisterChange}
                        aria-invalid={Boolean(registerErrors.firstName)}
                        className={inputClass(registerErrors.firstName)}
                        placeholder="Nino"
                    />
                    {registerErrors.firstName && (
                        <p className="mt-1 text-sm text-red-500">{registerErrors.firstName}</p>
                    )}
                </div>
                <div>
                    <label htmlFor="lastName" className="block text-sm font-medium text-slate-700 mb-1">
                        Last name
                    </label>
                    <input
                        id="lastName"
                        name="lastName"
                        type="text"
                        autoComplete="family-name"
                        value={registerData.lastName}
                        onChange={handleRegisterChange}
                        aria-invalid={Boolean(registerErrors.lastName)}
                        className={inputClass(registerErrors.lastName)}
                        placeholder="Kapanadze"
                    />
                    {registerErrors.lastName && (
                        <p className="mt-1 text-sm text-red-500">{registerErrors.lastName}</p>
                    )}
                </div>
            </div>

            <div>
                <label htmlFor="registerEmail" className="block text-sm font-medium text-slate-700 mb-1">
                    Email
                </label>
                <input
                    id="registerEmail"
                    name="email"
                    type="email"
                    autoComplete="email"
                    value={registerData.email}
                    onChange={handleRegisterChange}
                    aria-invalid={Boolean(registerErrors.email)}
                    className={inputClass(registerErrors.email)}
                    placeholder="you@example.com"
                />
                {registerErrors.email && (
                    <p className="mt-1 text-sm text-red-500">{registerErrors.email}</p>
                )}
            </div>

            <div>
                <label htmlFor="contactPhone" className="block text-sm font-medium text-slate-700 mb-1">
                    Phone number
                </label>
                <input
                    id="contactPhone"
                    name="contactPhone"
                    type="tel"
                    autoComplete="tel"
                    value={registerData.contactPhone}
                    onChange={handleRegisterChange}
                    aria-invalid={Boolean(registerErrors.contactPhone)}
                    className={inputClass(registerErrors.contactPhone)}
                    placeholder="+995 555 12 34 56"
                />
                {registerErrors.contactPhone && (
                    <p className="mt-1 text-sm text-red-500">{registerErrors.contactPhone}</p>
                )}
            </div>

            <div className="grid grid-cols-2 gap-4">
                <div>
                    <label htmlFor="dateOfBirth" className="block text-sm font-medium text-slate-700 mb-1">
                        Date of birth
                    </label>
                    <input
                        id="dateOfBirth"
                        name="dateOfBirth"
                        type="date"
                        autoComplete="bday"
                        value={registerData.dateOfBirth}
                        onChange={handleRegisterChange}
                        aria-invalid={Boolean(registerErrors.dateOfBirth)}
                        className={inputClass(registerErrors.dateOfBirth)}
                    />
                    {registerErrors.dateOfBirth && (
                        <p className="mt-1 text-sm text-red-500">{registerErrors.dateOfBirth}</p>
                    )}
                </div>
                <div>
                    <label htmlFor="gender" className="block text-sm font-medium text-slate-700 mb-1">
                        Gender
                    </label>
                    <select
                        id="gender"
                        name="gender"
                        value={registerData.gender}
                        onChange={handleRegisterChange}
                        aria-invalid={Boolean(registerErrors.gender)}
                        className={inputClass(registerErrors.gender)}
                    >
                        <option value="">Select...</option>
                        <option value="female">Female</option>
                        <option value="male">Male</option>
                        <option value="other">Other</option>
                        <option value="prefer_not_to_say">Prefer not to say</option>
                    </select>
                    {registerErrors.gender && (
                        <p className="mt-1 text-sm text-red-500">{registerErrors.gender}</p>
                    )}
                </div>
            </div>

            <div>
                <label htmlFor="nationalId" className="block text-sm font-medium text-slate-700 mb-1">
                    National ID
                </label>
                <input
                    id="nationalId"
                    name="nationalId"
                    type="text"
                    value={registerData.nationalId}
                    onChange={handleRegisterChange}
                    aria-invalid={Boolean(registerErrors.nationalId)}
                    className={inputClass(registerErrors.nationalId)}
                    placeholder="01234567890"
                />
                {registerErrors.nationalId && (
                    <p className="mt-1 text-sm text-red-500">{registerErrors.nationalId}</p>
                )}
            </div>

            <div>
                <label htmlFor="registerPassword" className="block text-sm font-medium text-slate-700 mb-1">
                    Password
                </label>
                <input
                    id="registerPassword"
                    name="password"
                    type="password"
                    autoComplete="new-password"
                    value={registerData.password}
                    onChange={handleRegisterChange}
                    aria-invalid={Boolean(registerErrors.password)}
                    className={inputClass(registerErrors.password)}
                    placeholder="At least 8 characters"
                />
                {registerErrors.password && (
                    <p className="mt-1 text-sm text-red-500">{registerErrors.password}</p>
                )}
            </div>

            <div>
                <label htmlFor="confirmPassword" className="block text-sm font-medium text-slate-700 mb-1">
                    Confirm password
                </label>
                <input
                    id="confirmPassword"
                    name="confirmPassword"
                    type="password"
                    autoComplete="new-password"
                    value={registerData.confirmPassword}
                    onChange={handleRegisterChange}
                    aria-invalid={Boolean(registerErrors.confirmPassword)}
                    className={inputClass(registerErrors.confirmPassword)}
                    placeholder="••••••••"
                />
                {registerErrors.confirmPassword && (
                    <p className="mt-1 text-sm text-red-500">{registerErrors.confirmPassword}</p>
                )}
            </div>

            <button
                type="submit"
                className="mt-2 w-full rounded-lg bg-[#8dfc9c] text-slate-900 font-semibold py-2.5 hover:opacity-90 transition-opacity"
            >
                Sign Up
            </button>
        </form>
    );
}