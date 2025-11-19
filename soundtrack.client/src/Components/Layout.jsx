import React, { useState } from 'react';
import { Outlet, Link } from 'react-router-dom';
import './Layout.css';
import { useNavigate } from 'react-router-dom';

const currentUserId = 1; 
const username = "Pepito43"; 

const Layout = () => {
    //Para poder usar navbar
    const navigate = useNavigate();
    const [searchQuery , setSearchQuery] = useState("");

    const handleSearch = (e) => {
    e.preventDefault();
    if (searchQuery.trim()) {
        navigate(`/search/${searchQuery}`);
        setSearchQuery("");
    }
    };
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
                        <form onSubmit={handleSearch}>
                            <input
                                type="text"
                                placeholder="Search..."
                                className="search-input"
                                value={searchQuery}
                                onChange={(e) => setSearchQuery(e.target.value)}
                            />
                        </form>
                      
                    </div>

                    {
                    // Perfil right, trate de poner imagen arriba y nombre abajo pero no pude XD 
                    }
                    <div className="navbar-right">
                        <Link to={`/profile/${currentUserId}`} className="profile-link">
                            <img
                                src="/user_p.png"
                                alt={`${username} Profile`}
                                className="profile-image"
                            />
                            <span className="profile-name">{username}</span>
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