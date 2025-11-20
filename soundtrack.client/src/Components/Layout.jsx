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
        if (window.confirm('¿Estas seguro que quieres cerrar sesion?')) {
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

    //Para login con Spotify
    const handleSpotifyLogin = () => {
        const backendPort = "7232";
        window.location.href = `https://127.0.0.1:${backendPort}/api/AuthSpotify/login`;
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

                    {/* Busqueda centro */}
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

                    {/* Perfil derecha - Cambia segun autenticacion */}
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
                                    title="Cerrar sesion"
                                >
                                   
                                </button>
                            </div>
                        ) : (
                            // Usuario NO logeado
                            <button
                                className="btn-login"
                                onClick={() => setShowAuthModal(true)}
                            >
                                Iniciar Sesion
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

            {/* Modal de autenticacion */}
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

                        <div style={{
                            marginTop: '20px',
                            paddingTop: '20px',
                            borderTop: '1px solid #444', 
                            textAlign: 'center'
                        }}>
                            <p style={{ marginBottom: '10px', fontSize: '0.9em', color: '#888' }}>O continua con</p>
                            <button
                                onClick={handleSpotifyLogin}
                                style={{
                                    backgroundColor: '#1DB954', 
                                    color: 'white',
                                    padding: '12px 24px',
                                    border: 'none',
                                    borderRadius: '50px',
                                    fontSize: '16px',
                                    fontWeight: 'bold',
                                    cursor: 'pointer',
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    width: '100%',
                                    transition: 'transform 0.2s'
                                }}
                                onMouseOver={(e) => e.currentTarget.style.transform = 'scale(1.05)'}
                                onMouseOut={(e) => e.currentTarget.style.transform = 'scale(1)'}
                            >
                                <span style={{ marginRight: '10px' }}></span> Iniciar Sesion con Spotify
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Layout;