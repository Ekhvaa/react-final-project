import React from 'react';
import Header from '../components/Header';
import Footer from '../components/Footer';
import AuthCard from '../components/AuthCard';
import LoginForm from '../components/LoginForm';

export default function LoginPage() {
    return (
        <>
            <Header />
            <AuthCard
                title="Welcome back"
                description="Sign in to keep planning your next adventure."
                footerText="Don't have an account?"
                footerLinkText="Sign up"
                footerLinkTo="/register"
            >
                <LoginForm />
            </AuthCard>
            <Footer />
        </>
    );
}