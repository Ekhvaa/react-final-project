import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

export default function ProtectedRoute() {
    const { isAuthenticated, isLoadingAuth } = useAuth();

    if (isLoadingAuth) {
        return (
            <div className="flex justify-center items-center h-screen">
                <p className="text-xl text-slate-500 font-semibold">Checking authentication...</p>
            </div>
        );
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    return <Outlet />;
}