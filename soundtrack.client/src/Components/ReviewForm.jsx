import React, { useState } from 'react';
import InteractiveStars from './Common/InteractiveStars';
import './ReviewForm.css';

const ReviewForm = ({ onSubmit, onCancel }) => {
    const [review, setReview] = useState({
        score: 5,
        description: ''
    });
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            const response = await fetch('https://localhost:7232/api/review', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    author: "Pepito43",
                    userId: 1,
                    description: review.description,
                    score: review.score,
                    publicationDate: new Date().toISOString(),
                    likes: 0,
                    dislikes: 0
                })
            });

            if (response.ok) {
                const savedReview = await response.json();
                onSubmit(savedReview);
                setReview({ score: 5, description: '' });
                alert('Review publicada exitosamente!');
            } else {
                alert('Error al publicar la review');
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Error de conexión con el servidor');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="review-form-container">
            <h3>Escribe tu review</h3>
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