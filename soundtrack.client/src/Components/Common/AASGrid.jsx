import React from 'react';
import AASCard from './AASCard';

const AASGrid = ({ items, type, title, columns = 4 }) => {
    if (!items || items.length === 0) {
        return null;
    }

    return (
        <div className="aas-grid-section">
            {title && <h2 className="aas-grid-title">{title}</h2>}
            <div className={`aas-grid columns-${columns}`}>
                {items.map((item) => (
                    <AASCard
                        key={item.id}
                        id={item.id}
                        type={type}
                        imageUrl={item.imageUrl}
                        title={item.name}
                        subtitle={item.subtitle}
                        score={item.score}
                    />
                ))}
            </div>
        </div>
    );
};

export default AASGrid;