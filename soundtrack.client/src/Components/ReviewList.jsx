import React, { useState, useEffect } from 'react';
import StarRating from './Common/StarRating';
import './Common/Common.css';

const ReviewsList = ({ profileId, profileType }) => {
    const [reviews, setReviews] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (profileId && profileType) {
            fetchReviews();
        }
    }, [profileId, profileType]);

    const fetchReviews = async () => {
        try {
            setLoading(true);
            setError(null);

            // Sacar todas las reviews
            const response = await fetch('https://localhost:7232/api/review');
            
            if (!response.ok) {
                throw new Error('Error al cargar reviews');
            }

            const allReviews = await response.json();
            
            // Filtrar reviews segun de que son
            let filteredReviews = [];
            if (profileType === 'song') {
                filteredReviews = allReviews.filter(r => r.songProfileId === profileId);
            } else if (profileType === 'artist') {
                filteredReviews = allReviews.filter(r => r.artistProfileId === profileId);
            } else if (profileType === 'album') {
                filteredReviews = allReviews.filter(r => r.albumProfileId === profileId);
            }
            
            setReviews(filteredReviews);
        } catch (error) {
            console.error('Error al cargar reviews:', error);
            setError('No se pudieron cargar las reviews. Verifica que el servidor esté corriendo.');
        } finally {
            setLoading(false);
        }
    };

    // Likes
    const handleLike = async (reviewId) => {
        console.log('Like review:', reviewId);
    };

    //Dislike
    const handleDislike = async (reviewId) => {
        console.log('Dislike review:', reviewId);
    };

    if (loading) return <p>Cargando reviews...</p>;

    if (error) {
        return (
            <div className="reviews-section">
                <h2>Reviews</h2>
                <div style={{
                    padding: '1rem',
                    backgroundColor: 'rgba(255, 0, 0, 0.1)',
                    border: '1px solid red',
                    borderRadius: '8px',
                    color: 'red',
                    textAlign: 'center'
                }}>
                    {error}
                </div>
            </div>
        );
    }

    if (reviews.length === 0) {
        return (
            <div className="reviews-section">
                <h2>Reviews (0)</h2>
                <p className="no-reviews">
                    No hay reviews. Aventurate con nosotros y se el primero en escribir una
                </p>
            </div>
        );
    }

    return (
        <div className="reviews-section">
            <h2>Reviews ({reviews.length})</h2>

            <div className="reviews-list">
                {reviews.map((review) => (
                    <div key={review.id} className="review-card">
                        <div className="review-header">
                            <div className="review-author">
                                <img
                                    src="/user_p.png"
                                    alt={review.author}
                                    className="author-avatar"
                                />
                                <span className="author-name">{review.author}</span>
                            </div>
                            <div className="review-rating">
                                <StarRating score={review.score} size="medium" />
                            </div>
                        </div>

                        <p className="review-text">{review.description}</p>

                        <div className="review-footer">
                            <span className="review-date">
                                {new Date(review.publicationDate).toLocaleDateString('es-ES')}
                            </span>
                            <div className="review-actions-buttons">
                                <button 
                                    className="like-btn"
                                    onClick={() => handleLike(review.id)}
                                >
                                    ❤️ {review.likes}
                                </button>
                                <button 
                                    className="dislike-btn"
                                    onClick={() => handleDislike(review.id)}
                                >
                                    👎 {review.dislikes}
                                </button>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default ReviewsList;