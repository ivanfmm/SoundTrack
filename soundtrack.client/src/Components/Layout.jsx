import React, { useState } from 'react';
import { Outlet, Link, useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import AuthForm from './AuthForm';
import './Layout.css';

const Layout = () => {
    const navigate = useNavigate();
    const { user, isAuthenticated, logout, loading } = useAuth();
    const [searchQuery, setSearchQuery] = useState("");
    const [showAuthModal, setShowAuthModal] = useState(false);

    const handleSearch = (e) => {
        e.preventDefault();
        if (searchQuery.trim()) {
            navigate(`/search/${searchQuery}`);
            setSearchQuery("");
        }
    };

    const handleLogout = async () => {
        if (window.confirm('¿Estás seguro que quieres cerrar sesión?')) {
            await logout();
            navigate('/');
        }
    };

    const handleAuthSuccess = (userData) => {
        console.log('Usuario autenticado:', userData);
        setShowAuthModal(false);
        // Opcionalmente redirigir a perfil
        // navigate(`/profile/${userData.userId}`);
    };

    return (
        <div className="app-container">
            <header className="app-header">
                <nav className="navbar">
                    {/* Logo izquierda */}
                    <div className="navbar-left">
                        <Link to="/" className="nav-logo">
                            SoundTrack
                        </Link>
                    </div>

                    {/* Búsqueda centro */}
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

                    {/* Perfil derecha - Cambia según autenticación */}
                    <div className="navbar-right">
                        {loading ? (
                            // Mientras carga
                            <div className="loading-auth">Cargando...</div>
                        ) : isAuthenticated ? (
                            // Usuario LOGEADO
                            <div className="user-menu">
                                <Link to={`/profile/${user.userId}`} className="profile-link">
                                    <img
                                        src={user.profilePictureUrl || "/user_p.png"}
                                        alt={`${user.username} Profile`}
                                        className="profile-image"
                                    />
                                    <span className="profile-name">{user.username}</span>
                                </Link>
                                <button
                                    className="btn-logout"
                                    onClick={handleLogout}
                                    title="Cerrar sesión"
                                >
                                   
                                </button>
                            </div>
                        ) : (
                            // Usuario NO logeado
                            <button
                                className="btn-login"
                                onClick={() => setShowAuthModal(true)}
                            >
                                Iniciar Sesión
                            </button>
                        )}
                    </div>
                </nav>
            </header>

            <main className="main-content">
                <Outlet />
            </main>

            <footer className="app-footer">
                <p>&copy; 2025 SoundTrack</p>
            </footer>

            {/* Modal de autenticación */}
            {showAuthModal && (
                <div className="auth-modal-overlay" onClick={() => setShowAuthModal(false)}>
                    <div className="auth-modal-content" onClick={(e) => e.stopPropagation()}>
                        <button
                            className="btn-close-modal"
                            onClick={() => setShowAuthModal(false)}
                        >
                            
                        </button>
                        <AuthForm
                            onSuccess={handleAuthSuccess}
                            onCancel={() => setShowAuthModal(false)}
                        />
                    </div>
                </div>
            )}
        </div>
    );
};

export default Layout;