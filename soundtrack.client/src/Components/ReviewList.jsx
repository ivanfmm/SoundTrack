import React, { useState, useEffect } from 'react';
import StarRating from './Common/StarRating';
import './Common/Common.css';

const ReviewsList = () => {
    const [reviews, setReviews] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchReviews();
    }, []);

    const fetchReviews = async () => {
        try {
            const response = await fetch('https://localhost:7232/api/review');
            const data = await response.json();
            setReviews(data);
        } catch (error) {
            console.error('Error al cargar reviews:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <p>Cargando reviews...</p>;

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
                                <button className="like-btn">
                                    ❤️ {review.likes}
                                </button>
                                <button className="dislike-btn">
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