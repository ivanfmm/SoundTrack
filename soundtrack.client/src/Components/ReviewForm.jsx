import React, { useState } from 'react';
import { useAuth } from './AuthContext';
import InteractiveStars from './Common/InteractiveStars';
import './ReviewForm.css';

const ReviewForm = ({ onSubmit, onCancel, profileId, profileType }) => {
    const { user, isAuthenticated } = useAuth();
    const [review, setReview] = useState({
        score: 5,
        description: ''
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    // Si el usuario no está logeado, mostrar mensaje
    if (!isAuthenticated) {
        return (
            <div className="review-form-container">
                <h3>Escribe tu review</h3>
                <div className="auth-required-message">
                    <p> Debes iniciar sesión para escribir una review</p>
                    <button
                        className="btn-submit-review"
                        onClick={() => {
                            // Podrías emitir un evento o mostrar el modal de login
                            alert('Por favor inicia sesión primero');
                        }}
                    >
                        Iniciar Sesión
                    </button>
                </div>
            </div>
        );
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            // Usar los datos REALES del usuario autenticado
            const reviewData = {
                author: user.username,      //  Usuario real del Context
                userId: user.userId,        //  ID real del Context
                description: review.description,
                score: review.score,
                publicationDate: new Date().toISOString(),
                likes: 0,
                dislikes: 0
            };

            // Agregar el ID dependiendo si es canción, artista o álbum
            if (profileType === 'song') {
                reviewData.songProfileId = profileId;
            } else if (profileType === 'artist') {
                reviewData.artistProfileId = profileId;
            } else if (profileType === 'album') {
                reviewData.albumProfileId = profileId;
            }

            console.log('Enviando review:', reviewData);

            const response = await fetch('https://localhost:7232/api/review', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include', //  IMPORTANTE: enviar cookies de sesión
                body: JSON.stringify(reviewData)
            });

            if (response.ok) {
                const savedReview = await response.json();
                onSubmit(savedReview);
                setReview({ score: 5, description: '' });
                alert('Review publicada exitosamente!');
            } else {
                const errorData = await response.text();
                console.error('Error del servidor:', errorData);
                setError('Error al publicar la review');
            }
        } catch (error) {
            console.error('Error:', error);
            setError('Error de conexión con el servidor');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="review-form-container">
            <h3>Escribe tu review</h3>
            <p className="review-author-info">
                Como: <strong style={{ color: 'var(--color-neon-green)' }}>
                    {user.username}
                </strong>
            </p>

            {error && (
                <div style={{
                    padding: '1rem',
                    marginBottom: '1rem',
                    backgroundColor: 'rgba(255, 0, 0, 0.1)',
                    border: '1px solid red',
                    borderRadius: '8px',
                    color: 'red'
                }}>
                    {error}
                </div>
            )}

            <form onSubmit={handleSubmit} className="review-form">
                <div className="form-group">
                    <label>Tu calificación:</label>
                    <InteractiveStars
                        score={review.score}
                        onChange={(score) => setReview({ ...review, score })}
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="review-text">Tu opinión:</label>
                    <textarea
                        id="review-text"
                        value={review.description}
                        onChange={(e) => setReview({ ...review, description: e.target.value })}
                        placeholder="¿Qué te pareció?"
                        rows="5"
                        required
                    />
                </div>

                <div className="form-actions">
                    <button
                        type="submit"
                        className="btn-submit-review"
                        disabled={loading}
                    >
                        {loading ? 'Publicando...' : 'Publicar Review'}
                    </button>
                    {onCancel && (
                        <button type="button" className="btn-cancel-review" onClick={onCancel}>
                            Cancelar
                        </button>
                    )}
                </div>
            </form>
        </div>
    );
};

export default ReviewForm;