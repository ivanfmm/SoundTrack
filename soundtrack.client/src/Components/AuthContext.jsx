import React, { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext();

// Hook personalizado para usar el contexto
export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth debe ser usado dentro de AuthProvider');
    }
    return context;
};

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    // Verificar si hay sesion activa al cargar la app
    useEffect(() => {
        checkAuthStatus();
    }, []);

    const checkAuthStatus = async () => {
        try {
            const response = await fetch('https://127.0.0.1:7232/api/auth/current', {
                credentials: 'include'
            });

            if (response.ok) {
                const userData = await response.json();
                setUser(userData);
                setIsAuthenticated(true);
            } else {
                setUser(null);
                setIsAuthenticated(false);
            }
        } catch (error) {
            console.error('Error al verificar autenticacion:', error);
            setUser(null);
            setIsAuthenticated(false);
        } finally {
            setLoading(false);
        }
    };

    const login = async (username, password, rememberMe = false) => {
        try {
            const response = await fetch('https://127.0.0.1:7232/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({ username, password, rememberMe })
            });

            if (response.ok) {
                const userData = await response.json();
                setUser(userData);
                setIsAuthenticated(true);
                return { success: true, data: userData };
            } else {
                const errorData = await response.json();
                return { success: false, error: errorData.message || 'Error al iniciar sesion' };
            }
        } catch (error) {
            console.error('Error en login:', error);
            return { success: false, error: 'Error de conexion con el servidor' };
        }
    };

    const register = async (username, email, password, birthDay = null) => {
        try {
            const response = await fetch('https://127.0.0.1:7232/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({ username, email, password, birthDay })
            });

            if (response.ok) {
                const userData = await response.json();
                setUser(userData);
                setIsAuthenticated(true);
                return { success: true, data: userData };
            } else {
                const errorData = await response.json();
                return { success: false, error: errorData.message || 'Error al registrarse' };
            }
        } catch (error) {
            console.error('Error en register:', error);
            return { success: false, error: 'Error de conexion con el servidor' };
        }
    };

    const logout = async () => {
        try {
            await fetch('https://127.0.0.1:7232/api/auth/logout', {
                method: 'POST',
                credentials: 'include'
            });
        } catch (error) {
            console.error('Error en logout:', error);
        } finally {
            setUser(null);
            setIsAuthenticated(false);
        }
    };

    const value = {
        user,
        isAuthenticated,
        loading,
        login,
        register,
        logout,
        checkAuthStatus
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
};