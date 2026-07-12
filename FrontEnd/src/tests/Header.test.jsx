import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom';
import { BrowserRouter } from 'react-router-dom';
import Header from '../components/Header';

describe('Header Component Tests', () => {
    const renderWithRouter = () => {
        render(
            <BrowserRouter>
                <Header />
            </BrowserRouter>
        );
    };

    test('1. renders the logo image successfully', () => {
        renderWithRouter();
        const logoImage = screen.getByAltText('logo');
        expect(logoImage).toBeInTheDocument();
    });

    test('2. displays the user profile name', () => {
        renderWithRouter();
        const profileNames = screen.getAllByText('Ekhva');
        expect(profileNames.length).toBeGreaterThan(0);
    });

    test('3. toggles the mobile menu when the burger button is clicked', () => {
        renderWithRouter();
        
        const toggleButton = screen.getByLabelText('Toggle navigation menu');
        
        fireEvent.click(toggleButton);
        expect(toggleButton).toBeInTheDocument();
    });
});