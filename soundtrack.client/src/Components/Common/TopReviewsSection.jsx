import React from 'react';
import { Link } from 'react-router-dom';
import StarRating from './StarRating';
import './TopReviewsSection.css'; // Asegúrate de que este CSS exista o usa el compartido

const TopReviewsSection = ({ reviews }) => {
    // Este componente espera 'reviews'
    if (!reviews || reviews.length === 0) {
        return null;
    }

    const truncateText = (text, maxLength = 150) => {
        if (!text) return '';
        if (text.length <= maxLength) return text;
        return text.substring(0, maxLength) + '...';
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('es-MX', { 
            year: 'numeric', month: 'short', day: 'numeric' 
        });
    };

    return (
        <section className="top-reviews-section">
            <h2 className="section-title">⭐ Reseñas Destacadas</h2>
            <div className="top-reviews-grid">
                {reviews.map((review, index) => (
                    <div key={review.id} className="top-review-card">
                        <div className="review-rank">{index + 1}</div>
                        
                        {/* Header */}
                        <div className="review-header">
                            <Link to={`/user/${review.user.id}`} className="user-info">
                                <img 
                                    src={review.user.imageUrl || '/user_p.png'} 
                                    alt={review.user.username}
                                    className="user-avatar"
                                />
                                <span className="username">{review.user.username}</span>
                            </Link>
                        </div>

                        {/* Contenido */}
                        <div className="review-content">
                            <div className="review-title-rating">
                                <h3 className="review-title">{review.title}</h3>
                                <StarRating rating={review.score} size="small" />
                            </div>
                            <p className="review-body">{truncateText(review.body)}</p>
                        </div>

                        {/* Footer */}
                        <div className="review-footer">
                            <div className="review-stats">
                                <span className="stat-item likes">👍 {review.likes}</span>
                            </div>
                            <span className="review-date">{formatDate(review.createdAt)}</span>
                        </div>
                    </div>
                ))}
            </div>
        </section>
    );
};

export default TopReviewsSection;