import React, { useState, useEffect } from 'react';
import StarRating from './Common/StarRating';
import './Common/Common.css';

const ReviewsList = ({ profileId, profileType }) => {
    const [reviews, setReviews] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [userLikeStatuses, setUserLikeStatuses] = useState({});

    const CURRENT_USER_ID = 1; //userId hardcodeado para los likes

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
            
            await fetchLikeStatuses(filteredReviews);

        } catch (error) {
            console.error('Error al cargar reviews:', error);
            setError('No se pudieron cargar las reviews. Verifica que el servidor esté corriendo.');
        } finally {
            setLoading(false);
        }
    };
    
    const fetchLikeStatuses = async (reviewsList) => {
        const statuses = {};
        
        for (const review of reviewsList) {
            const response = await fetch(
                `https://localhost:7232/api/review/${review.id}/like-status/${CURRENT_USER_ID}`
            );
            
            if (response.ok) {
                const data = await response.json();
                statuses[review.id] = data.likeStatus;
            }
        }
        
        setUserLikeStatuses(statuses);
    };

    const handleToggleLike = async (reviewId, action) => {
        try {
            const response = await fetch(`https://localhost:7232/api/review/${reviewId}/${action}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    userId: CURRENT_USER_ID
                })
            });

            if (response.ok) {
                const data = await response.json();
                setReviews(reviews.map(review => 
                    review.id === reviewId ? data.review : review
                ));

                setUserLikeStatuses(prevStatuses => ({
                    ...prevStatuses,
                    [reviewId]: data.userLikeStatus
                }));

            } else {
                console.error(`Error al dar ${action}`);
            }
        } catch (error) {
            console.error(`Error al dar ${action}:`, error);
        }
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
                {reviews.map((review) => {
                    const userStatus = userLikeStatuses[review.id] || 'None';
                    const hasLiked = userStatus === 'Like';
                    const hasDisliked = userStatus === 'Dislike';
                    
                    return (
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
                                        className={`like-btn ${hasLiked ? 'active' : ''}`}
                                        onClick={() => handleToggleLike(review.id, 'like')}
                                    >
                                        ❤️ {review.likes}
                                    </button>
                                    <button 
                                        className={`dislike-btn ${hasDisliked ? 'active' : ''}`}
                                        onClick={() => handleToggleLike(review.id,'dislike')}
                                    >
                                        👎 {review.dislikes}
                                    </button>
                                </div>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
};

export default ReviewsList;