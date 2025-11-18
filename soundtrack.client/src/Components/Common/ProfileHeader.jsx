import React from 'react';
import StarRating from './StarRating';
import './Common.css';

const ProfileHeader = ({ 
    imageUrl, // se va a cambiar depende de la api de spotify
    title, 
    subtitle,
    metadata = [], // el artista, album y fecha de publicacion
    score, 
    totalReviews = 0,
    genres = [],
    tags = [],
    description,

}) => {
    return (
        <div className="profile-header">
            <div className="profile-image">
                <img 
                    src={imageUrl || '/placeholder.png'} 
                    alt={title}
                />
            </div>
            
            <div className="profile-info">
                <h1 className="profile-title">{title}</h1>
                
                {subtitle && (
                    <p className="profile-subtitle">{subtitle}</p>
                )}
                
                {/* la data de la cancion que se vea chido */}
                {metadata.length > 0 && (
                    <div className="profile-metadata">
                        {metadata.map((item, index) => (
                            <p key={index} className="metadata-item">
                                <span className="metadata-label">{item.label}:</span>
                                <span className="metadata-value">{item.value}</span>
                            </p>
                        ))}
                    </div>
                )}

                {/* Rating */}
                {score !== null && score !== undefined ? (
                    <div className="profile-rating">
                        <span className="tags-label">Score:</span>
                        <div className="rating-display">
                            <StarRating score={Math.round(score)} size="large" />
                            <div className="rating-info">
                                <span className="rating-number">{score.toFixed(1)}</span>
                                <span className="rating-total">/ 5.0</span>
                            </div>
                        </div>
                    </div>
                ) : (
                    totalReviews === 0 && (
                        <div className="profile-rating no-rating">
                            <p className="no-rating-text">
                                ⭐ Aún no hay calificaciones. ¡Sé el primero en escribir una review!
                            </p>
                        </div>
                    )
                )}

                {/* Generos (no se como venia en el api asi que lo meti con arreglo y el genero en si) */}
                {genres.length > 0 && (
                    <div className="profile-tags-section">
                        <span className="tags-label">Géneros:</span>
                        <div className="profile-genres">
                            {genres.map((genre, index) => (
                                <span key={index} className="genre-tag">{genre}</span>
                            ))}
                        </div>
                    </div>
                )}

                {/* Tags lo mismo que generos copy paste */}
                {tags.length > 0 && (
                    <div className="profile-tags-section">
                        <span className="tags-label">Tags:</span>
                        <div className="profile-tags">
                            {tags.map((tag, index) => (
                                <span key={index} className="tag-item">{tag}</span>
                            ))}
                        </div>
                    </div>
                )}

                {/* Descripcion no se que poner aqui se puede quitar easy */}
                {description && (
                    <div className="profile-description">
                        <p>{description}</p>
                    </div>
                )}


            </div>
        </div>
    );
};

export default ProfileHeader;