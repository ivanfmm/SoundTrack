import React from 'react';
import './Common.css';

const InteractiveStars = ({ score, onChange, showScore = true }) => {
    return (
        <div className="interactive-stars-container">
            <div className="interactive-stars">
                {[1, 2, 3, 4, 5].map((star) => (
                    <button
                        key={star}
                        type="button"
                        className={star <= score ? 'star-btn filled' : 'star-btn'}
                        onClick={() => onChange(star)}
                    >
                        ★
                    </button>
                ))}
            </div>
            {showScore && <span className="score-display">{score}/5</span>}
        </div>
    );
};

export default InteractiveStars;