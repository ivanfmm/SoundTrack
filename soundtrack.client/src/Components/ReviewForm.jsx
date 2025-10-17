import React, { useState } from 'react';
import InteractiveStars from './Common/InteractiveStars';
import './ReviewForm.css';

const ReviewForm = ({ onSubmit, onCancel }) => {
    const [review, setReview] = useState({
        score: 5,
        description: ''
    });

    //manda formulario (cambiara cuando se conecte DB..... probablemente)
    const handleSubmit = (e) => {
        e.preventDefault();
        onSubmit(review);
        setReview({ score: 5, description: '' });
    };

    return (
        <div className="review-form-container">
            {
                //formato del cuadrito de comentario
            }
            <h3>Escribe tu review</h3>
            <form onSubmit={handleSubmit} className="review-form">
                <div className="form-group">
                    <label>Tu calificacion:</label>
                    <InteractiveStars 
                        score={review.score}
                        onChange={(score) => setReview({...review, score})}
                    />
                </div>

                {
                    //ya el cuadrito donde van a escribir
                }
                <div className="form-group">
                    <label htmlFor="review-text">Tu opinion:</label>
                    <textarea
                        id="review-text"
                        value={review.description}
                        onChange={(e) => setReview({...review, description: e.target.value})}
                        placeholder="Que te parecio?"
                        rows="5"
                        required
                    />
                </div>


                {
                    //la publica o nel
                }
                <div className="form-actions">
                    <button type="submit" className="btn-submit-review">
                        Publicar Review
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