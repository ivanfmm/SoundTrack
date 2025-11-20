import React from 'react';
import './Common.css';

// Aceptar Rating y Score porque si no da error
const StarRating = ({ rating, score, size = 'medium', showScore = false }) => {
    // Usamos rating SI existe, si no usamos score, y si no 0
    const finalScore = rating || score || 0;

return (
        <div className={`star-rating ${size}`}>
            <div className="stars">
                {[1, 2, 3, 4, 5].map((star) => (
                    <span 
                        key={star} 
                        className={star <= Math.round(finalScore) ? 'star filled' : 'star'}
                    >
                        ★
                    </span>
                ))}
            </div>
            {showScore && <span className="score-text">{finalScore.toFixed(1)}/5</span>}
        </div>
    );
};

export default StarRating;