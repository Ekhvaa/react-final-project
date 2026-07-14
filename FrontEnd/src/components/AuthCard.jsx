import React from 'react';
import { Link } from 'react-router-dom';

export default function AuthCard({ title, description, children, footerText, footerLinkText, footerLinkTo }) {
    return (
        <main className="min-h-[calc(100vh-88px)] flex items-center justify-center bg-slate-50 px-4 py-16">
            <div className="w-full max-w-md">
                <div className="bg-white rounded-2xl shadow-lg border border-slate-100 overflow-hidden">
                    <div className="p-8">
                        <h1 className="text-2xl font-bold text-slate-900 mb-1">{title}</h1>
                        <p className="text-slate-500 mb-6">{description}</p>

                        {children}

                        <p className="text-center text-sm text-slate-500 mt-6">
                            {footerText}{' '}
                            <Link
                                to={footerLinkTo}
                                className="text-slate-900 font-semibold hover:opacity-80 transition-opacity"
                            >
                                {footerLinkText}
                            </Link>
                        </p>
                    </div>
                </div>
            </div>
        </main>
    );
}