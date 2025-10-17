import React from 'react';
import { Outlet, Link } from 'react-router-dom';
import './Layout.css';

const Layout = () => {
    return (
        <div className="app-container">
            <header className="app-header">
                <nav className="navbar">
                    
                    {
                    //Logo izq (Cambiar poe img)
                    }
                    <div className="navbar-left">
                        <Link to="/" className="nav-logo">
                            SoundTrack
                        </Link>
                    </div>

                    {
                    // Busqueda Centro
                    }
                    <div className="navbar-center">
                        <input
                            type="text"
                            placeholder="Search..."
                            className="search-input"
                        />
                    </div>

                    {
                    // Perfil right, trate de poner imagen arriba y nombre abajo pero no pude XD 
                    }
                    <div className="navbar-right">
                        <Link to="/" className="profile-link">
                            <img
                                src="/user_p.png"
                                alt="PUser Profile"
                                className="profile-image"
                            />
                            <span className="profile-name">Pepito43</span>
                        </Link>
                    </div>
                </nav>
            </header>

            <main className="main-content">
                <Outlet />
            </main>

            <footer className="app-footer">
                <p>&copy; 2025 SoundTrack</p>
            </footer>
        </div>
    );
};

export default Layout;