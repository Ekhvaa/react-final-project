import React from 'react';
import Header from '../components/Header';
import Footer from '../components/Footer';
import AuthCard from '../components/AuthCard';
import RegisterForm from '../components/RegistrationForm';

export default function RegisterPage() {
    return (
        <>
            <Header />
            <AuthCard
                title="Create your account"
                description="Join us and start booking unforgettable tours."
                footerText="Already have an account?"
                footerLinkText="Log in"
                footerLinkTo="/login"
            >
                <RegisterForm />
            </AuthCard>
            <Footer />
        </>
    );
}