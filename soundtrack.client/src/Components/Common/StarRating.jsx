import React from 'react';
import './Common.css';

const StarRating = ({ score, size = 'medium', showScore = false }) => {
    return (
        <div className={`star-rating ${size}`}>
            <div className="stars">
                {[1, 2, 3, 4, 5].map((star) => (
                    <span key={star} className={star <= score ? 'star filled' : 'star'}>
                        ★
                    </span>
                ))}
            </div>
            {showScore && <span className="score-text">{score}/5</span>}
        </div>
    );
};

export default StarRating;