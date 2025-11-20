import React, { useState } from 'react';
import { useAuth } from './AuthContext';
import './AuthForm.css';

const AuthForm = ({ onSuccess, onCancel }) => {
    const { login, register, checkAuthStatus } = useAuth(); // Importar funciones del contexto
    const [isLogin, setIsLogin] = useState(true);
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
        birthDay: '',
        rememberMe: false
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData({
            ...formData,
            [name]: type === 'checkbox' ? checked : value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        // Validaciones
        if (!isLogin && formData.password !== formData.confirmPassword) {
            setError('Las contraseñas no coinciden');
            setLoading(false);
            return;
        }

        try {
            let result;

            if (isLogin) {
                // Usar la función login del contexto
                result = await login(
                    formData.username,
                    formData.password,
                    formData.rememberMe
                );
            } else {
                // Usar la función register del contexto
                result = await register(
                    formData.username,
                    formData.email,
                    formData.password,
                    formData.birthDay || null
                );
            }

            if (result.success) {
                console.log('Autenticación exitosa:', result.data);

                // Verificar el estado de autenticación para asegurar actualización
                await checkAuthStatus();

                // Limpiar formulario
                setFormData({
                    username: '',
                    email: '',
                    password: '',
                    confirmPassword: '',
                    birthDay: '',
                    rememberMe: false
                });

                // Llamar callback de éxito
                if (onSuccess) {
                    onSuccess(result.data);
                }

                alert(isLogin ? '¡Sesión iniciada exitosamente!' : '¡Registro exitoso! Sesión iniciada.');
            } else {
                // Mostrar error del servidor
                setError(result.error || 'Error en la autenticación');
            }
        } catch (error) {
            console.error('Error inesperado:', error);
            setError('Error de conexión con el servidor');
        } finally {
            setLoading(false);
        }
    };

    const toggleMode = () => {
        setIsLogin(!isLogin);
        setError(null);
        setFormData({
            username: '',
            email: '',
            password: '',
            confirmPassword: '',
            birthDay: '',
            rememberMe: false
        });
    };

    return (
        <div className="auth-form-container">
            <h3>{isLogin ? 'Iniciar Sesión' : 'Crear Cuenta'}</h3>

            {error && (
                <div className="auth-error">
                    {error}
                </div>
            )}

            <form onSubmit={handleSubmit} className="auth-form">
                <div className="form-group">
                    <label htmlFor="username">Usuario:</label>
                    <input
                        type="text"
                        id="username"
                        name="username"
                        value={formData.username}
                        onChange={handleChange}
                        placeholder="Ingresa tu usuario"
                        required
                    />
                </div>

                {!isLogin && (
                    <div className="form-group">
                        <label htmlFor="email">Email:</label>
                        <input
                            type="email"
                            id="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            placeholder="correo@ejemplo.com"
                            required
                        />
                    </div>
                )}

                <div className="form-group">
                    <label htmlFor="password">Contraseña:</label>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        value={formData.password}
                        onChange={handleChange}
                        placeholder="Ingresa tu contraseña"
                        required
                    />
                </div>

                {!isLogin && (
                    <>
                        <div className="form-group">
                            <label htmlFor="confirmPassword">Confirmar Contraseña:</label>
                            <input
                                type="password"
                                id="confirmPassword"
                                name="confirmPassword"
                                value={formData.confirmPassword}
                                onChange={handleChange}
                                placeholder="Confirma tu contraseña"
                                required
                            />
                        </div>

                        <div className="form-group">
                            <label htmlFor="birthDay">Fecha de Nacimiento (opcional):</label>
                            <input
                                type="date"
                                id="birthDay"
                                name="birthDay"
                                value={formData.birthDay}
                                onChange={handleChange}
                            />
                        </div>
                    </>
                )}

                {isLogin && (
                    <div className="form-group-checkbox">
                        <input
                            type="checkbox"
                            id="rememberMe"
                            name="rememberMe"
                            checked={formData.rememberMe}
                            onChange={handleChange}
                        />
                        <label htmlFor="rememberMe">Recordarme</label>
                    </div>
                )}

                <div className="form-actions">
                    <button
                        type="submit"
                        className="btn-submit-auth"
                        disabled={loading}
                    >
                        {loading
                            ? (isLogin ? 'Iniciando...' : 'Registrando...')
                            : (isLogin ? 'Iniciar Sesión' : 'Registrarse')
                        }
                    </button>
                    {onCancel && (
                        <button
                            type="button"
                            className="btn-cancel-auth"
                            onClick={onCancel}
                        >
                            Cancelar
                        </button>
                    )}
                </div>
            </form>

            <div className="auth-toggle">
                <span>
                    {isLogin ? '¿No tienes cuenta? ' : '¿Ya tienes cuenta? '}
                </span>
                <button
                    type="button"
                    className="btn-toggle-mode"
                    onClick={toggleMode}
                >
                    {isLogin ? 'Regístrate' : 'Inicia Sesión'}
                </button>
            </div>
        </div>
    );
};

export default AuthForm;