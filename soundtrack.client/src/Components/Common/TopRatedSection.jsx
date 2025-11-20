import React from 'react';
import { Link } from 'react-router-dom';
import StarRating from './StarRating';
import './TopRatedSection.css';

const TopRatedSection = ({ title, items, type }) => {
    // Aquí usamos 'items', que es lo que manda Home.jsx
    if (!items || items.length === 0) {
        return null;
    }

    return (
        <section className="top-rated-section">
            <h2 className="section-title">{title}</h2>
            <div className="top-rated-grid">
                {items.map((item, index) => (
                    <Link 
                        key={item.id} 
                        to={`/${type}/${item.id}`} 
                        className="top-rated-card"
                    >
                        <div className="rank-badge">{index + 1}</div>
                        <div className="card-image-container">
                            <img 
                                src={item.imageUrl || '/user_p.png'} 
                                alt={item.name}
                                className="card-image"
                            />
                        </div>
                        <div className="card-content">
                            <h3 className="card-title">{item.name}</h3>
                            <div className="card-rating">
                                <StarRating rating={item.averageScore || 0} size="small" />
                                <span className="rating-text">
                                    {item.averageScore ? item.averageScore.toFixed(1) : 'N/A'}
                                </span>
                            </div>
                            <p className="review-count">
                                {item.reviewCount} {item.reviewCount === 1 ? 'reseña' : 'reseñas'}
                            </p>
                        </div>
                    </Link>
                ))}
            </div>
        </section>
    );
};

export default TopRatedSection;