import React, { useState } from 'react';
import InteractiveStars from './Common/InteractiveStars';
import './ReviewForm.css';

const ReviewForm = ({ onSubmit, onCancel, profileId, profileType }) => {
    const [review, setReview] = useState({
        score: 5,
        description: ''
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            // Perfil del usuario
            const reviewData = {
                author: "Pepito43",
                userId: 1,
                description: review.description,
                score: review.score,
                publicationDate: new Date().toISOString(),
                likes: 0,
                dislikes: 0
            };

            // Agregar el ID dependiendo si es cancion, artista o album
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
            setError('Error de conexion con el servidor');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="review-form-container">
            <h3>Escribe tu review</h3>

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
                    <label>Tu calificacion:</label>
                    <InteractiveStars
                        score={review.score}
                        onChange={(score) => setReview({ ...review, score })}
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="review-text">Tu opinion:</label>
                    <textarea
                        id="review-text"
                        value={review.description}
                        onChange={(e) => setReview({ ...review, description: e.target.value })}
                        placeholder="Que te parecio?"
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