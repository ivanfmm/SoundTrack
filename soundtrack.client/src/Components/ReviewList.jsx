import React, { useState, useEffect } from 'react';
import { useAuth } from './AuthContext';
import { useNavigate } from 'react-router-dom'; // Agregar esto
import StarRating from './Common/StarRating';
import './Common/Common.css';

const ReviewsList = ({ profileId, profileType }) => {
    const { user, isAuthenticated } = useAuth();
    const navigate = useNavigate(); // Agregar esto
    const [reviews, setReviews] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [userLikeStatuses, setUserLikeStatuses] = useState({});

    useEffect(() => {
        if (profileId && profileType) {
            fetchReviews();
        }
    }, [profileId, profileType]);

    useEffect(() => {
        if (isAuthenticated && user && reviews.length > 0) {
            fetchLikeStatuses(reviews);
        }
    }, [user, isAuthenticated]);

    const fetchReviews = async () => {
        try {
            setLoading(true);
            setError(null);

            const response = await fetch('https://127.0.0.1:7232/api/review', {
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error('Error al cargar reviews');
            }

            const allReviews = await response.json();

            let filteredReviews = [];
            if (profileType === 'song') {
                filteredReviews = allReviews.filter(r => r.songProfileId === profileId);
            } else if (profileType === 'artist') {
                filteredReviews = allReviews.filter(r => r.artistProfileId === profileId);
            } else if (profileType === 'album') {
                filteredReviews = allReviews.filter(r => r.albumProfileId === profileId);
            }

            setReviews(filteredReviews);

            if (isAuthenticated && user) {
                await fetchLikeStatuses(filteredReviews);
            }

        } catch (error) {
            console.error('Error al cargar reviews:', error);
            setError('No se pudieron cargar las reviews. Verifica que el servidor esté corriendo.');
        } finally {
            setLoading(false);
        }
    };

    const fetchLikeStatuses = async (reviewsList) => {
        if (!user || !isAuthenticated) return;

        const statuses = {};

        for (const review of reviewsList) {
            try {
                const response = await fetch(
                    `https://127.0.0.1:7232/api/review/${review.id}/like-status/${user.userId}`,
                    { credentials: 'include' }
                );

                if (response.ok) {
                    const data = await response.json();
                    statuses[review.id] = data.likeStatus;
                }
            } catch (error) {
                console.error('Error fetching like status:', error);
            }
        }

        setUserLikeStatuses(statuses);
    };

    const handleToggleLike = async (reviewId, action) => {
        if (!isAuthenticated || !user) {
            alert('Debes iniciar sesión para dar like/dislike');
            return;
        }

        try {
            const response = await fetch(`https://127.0.0.1:7232/api/review/${reviewId}/${action}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({
                    userId: user.userId
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

    // Nueva función para navegar al perfil del usuario
    const handleAuthorClick = (authorId) => {
        navigate(`/profile/${authorId}`);
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
                    No hay reviews. Aventúrate con nosotros y sé el primero en escribir una
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
                                <div
                                    className="review-author"
                                    onClick={() => handleAuthorClick(review.userId)}
                                    style={{ cursor: 'pointer' }}
                                >
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
                                        disabled={!isAuthenticated}
                                        title={!isAuthenticated ? 'Inicia sesión para dar like' : ''}
                                    >
                                        ❤️ {review.likes}
                                    </button>
                                    <button
                                        className={`dislike-btn ${hasDisliked ? 'active' : ''}`}
                                        onClick={() => handleToggleLike(review.id, 'dislike')}
                                        disabled={!isAuthenticated}
                                        title={!isAuthenticated ? 'Inicia sesión para dar dislike' : ''}
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