import React from 'react';
import { Link } from 'react-router-dom';
import StarRating from './StarRating';

const AASCard = ({ 
    id,
    type, // 'song', 'album', 'artist' (voy a ver el ejemplo de mijares para modificarlo si se necesita)
    imageUrl, //(se puede cambiar depende lo que de el api)
    title, 
    subtitle,
    score,
    onClick
}) => {
    const getLink = () => {
        switch(type) {
            case 'song': return `/song/${id}`;
            case 'album': return `/album/${id}`;
            case 'artist': return `/artist/${id}`;
            default: return '#';
        }
    };

    const content = (
        <>
            <div className="aas-card-image">
                <img src={imageUrl || '/placeholder.png'} alt={title} />
            </div>
            <div className="aas-card-info">
                <h3 className="aas-card-title">{title}</h3>
                {subtitle && <p className="aas-card-subtitle">{subtitle}</p>}
                {score && <StarRating score={score} size="small" />}
            </div>
        </>
    );

    if (onClick) {
        return (
            <div className="aas-card" onClick={onClick}>
                {content}
            </div>
        );
    }

    return (
        <Link to={getLink()} className="aas-card">
            {content}
        </Link>
    );
};

export default AASCard;