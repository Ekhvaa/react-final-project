import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import Header from '../components/Header';
import { useAuth } from '../hooks/useAuth';

vi.mock('../hooks/useAuth', () => ({
  useAuth: vi.fn(),
}));

const renderHeader = () =>
  render(
    <MemoryRouter>
      <Header />
    </MemoryRouter>
  );

describe('Header', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('shows a Login link and nav links, but no profile/logout, when logged out', () => {
    useAuth.mockReturnValue({ user: null, isAuthenticated: false, logout: vi.fn() });

    renderHeader();

    expect(screen.getByRole('link', { name: /login/i })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /home/i })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /tours/i })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /contact/i })).toBeInTheDocument();
    expect(screen.queryByText(/logout/i)).not.toBeInTheDocument();
  });

  it('shows the username and a working Logout button when logged in', () => {
    const logout = vi.fn();
    useAuth.mockReturnValue({
      user: { username: 'jane_doe' },
      isAuthenticated: true,
      logout,
    });

    renderHeader();

    expect(screen.getByText('jane_doe')).toBeInTheDocument();
    expect(screen.queryByRole('link', { name: /login/i })).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole('button', { name: /logout/i }));
    expect(logout).toHaveBeenCalledTimes(1);
  });

  it('toggles the mobile menu open and closed when the hamburger button is clicked', () => {
    useAuth.mockReturnValue({ user: null, isAuthenticated: false, logout: vi.fn() });

    renderHeader();

    expect(screen.getAllByRole('link', { name: /home/i })).toHaveLength(1);

    const toggleButton = screen.getByRole('button', { name: /toggle navigation menu/i });
    fireEvent.click(toggleButton);
    expect(screen.getAllByRole('link', { name: /home/i })).toHaveLength(2);

    fireEvent.click(toggleButton);
    expect(screen.getAllByRole('link', { name: /home/i })).toHaveLength(1);
  });
});