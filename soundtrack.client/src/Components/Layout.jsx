import React, { useState, useEffect } from 'react';
import { Outlet, Link, useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import AuthForm from './AuthForm';
import './Layout.css';

const Layout = () => {
    const navigate = useNavigate();
    const { user, isAuthenticated, logout, loading, checkAuthStatus } = useAuth();
    const [searchQuery, setSearchQuery] = useState("");
    const [showAuthModal, setShowAuthModal] = useState(false);

    // Efecto para forzar actualización cuando cambia el estado de autenticación
    useEffect(() => {
        console.log('Estado de autenticación cambió:', { isAuthenticated, user });
    }, [isAuthenticated, user]);

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

    const handleAuthSuccess = async (userData) => {
        console.log('Usuario autenticado:', userData);

        // Cerrar el modal
        setShowAuthModal(false);

        // Forzar actualización del estado de autenticación
        await checkAuthStatus();

        // Opcionalmente redirigir a perfil después de un pequeño delay
        setTimeout(() => {
            // navigate(`/profile/${userData.userId}`);
        }, 100);
    };

    const handleOpenAuthModal = () => {
        setShowAuthModal(true);
    };

    const handleCloseAuthModal = () => {
        setShowAuthModal(false);
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
                        ) : isAuthenticated && user ? (
                            // Usuario LOGEADO - Mostrar imagen de perfil
                            <div className="user-menu">
                                <Link to={`/profile/${user.userId}`} className="profile-link">
                                    <img
                                        src="/user_p.png"
                                        alt={`${user.username} Profile`}
                                        className="profile-image"
                                        onError={(e) => {
                                            e.target.src = "/user_p.png"; // Fallback
                                        }}
                                    />
                                    <span className="profile-name">{user.username}</span>
                                </Link>
                                <button
                                    className="btn-logout"
                                    onClick={handleLogout}
                                    title="Cerrar sesión"
                                >
                                    X
                                </button>
                            </div>
                        ) : (
                            // Usuario NO logeado - Mostrar botón de login
                            <button
                                className="btn-login"
                                onClick={handleOpenAuthModal}
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
                <div className="auth-modal-overlay" onClick={handleCloseAuthModal}>
                    <div className="auth-modal-content" onClick={(e) => e.stopPropagation()}>
                        <button
                            className="btn-close-modal"
                            onClick={handleCloseAuthModal}
                        >
                            X
                        </button>
                        <AuthForm
                            onSuccess={handleAuthSuccess}
                            onCancel={handleCloseAuthModal}
                        />
                    </div>
                </div>
            )}
        </div>
    );
};

export default Layout;