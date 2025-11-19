import React, { useState } from 'react';
import { searchSpotify } from '../../api/spotify';
import './FavoritesSelector.css';

/**
 * Componente para buscar y seleccionar favoritos
 * @param {string} type - 'artist', 'album', o 'song'
 * @param {function} onSelect - Callback cuando se selecciona un item
 * @param {number} currentCount - Cantidad actual de favoritos
 * @param {number} maxCount - Máximo permitido (default: 3)
 */
const FavoritesSelector = ({ type, onSelect, currentCount, maxCount = 3 }) => {
    const [searchQuery, setSearchQuery] = useState('');
    const [searchResults, setSearchResults] = useState([]);
    const [isSearching, setIsSearching] = useState(false);

    const handleSearch = async (e) => {
        e.preventDefault();
        
        if (!searchQuery.trim() || currentCount >= maxCount) return;

        setIsSearching(true);
        try {
            const results = await searchSpotify(searchQuery);
            
            if (type === 'artist') {
                setSearchResults(results.artists);
            } else if (type === 'album') {
                setSearchResults(results.albums);
            } else if (type === 'song') {
                setSearchResults(results.tracks);
            }
        } catch (error) {
            console.error('Error searching:', error);
        } finally {
            setIsSearching(false);
        }
    };

    const handleSelect = (item) => {
        onSelect(item);
        setSearchResults([]);
        setSearchQuery('');
    };

    if (currentCount >= maxCount) {
        return (
            <div className="favorites-selector-disabled">
                <p>✓ Ya tienes {maxCount} {type === 'artist' ? 'artistas' : type === 'album' ? 'álbumes' : 'canciones'}</p>
            </div>
        );
    }

    return (
        <div className="favorites-selector">
            <form onSubmit={handleSearch} className="search-form">
                <input
                    type="text"
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    placeholder={`Buscar ${type === 'artist' ? 'artista' : type === 'album' ? 'álbum' : 'canción'}...`}
                    className="search-input"
                />
                <button type="submit" className="search-btn" disabled={isSearching}>
                    {isSearching ? '🔍' : '🔎'}
                </button>
            </form>

            {searchResults.length > 0 && (
                <div className="search-results">
                    {searchResults.map((item) => (
                        <div 
                            key={item.id} 
                            className="search-result-item"
                            onClick={() => handleSelect(item)}
                        >
                            <img 
                                src={item.images?.[0]?.url || item.album?.images?.[0]?.url || '/placeholder.png'} 
                                alt={item.name}
                                className="result-image"
                            />
                            <div className="result-info">
                                <p className="result-name">{item.name}</p>
                                <p className="result-subtitle">
                                    {type === 'artist' 
                                        ? `${item.followers?.total?.toLocaleString() || 0} seguidores`
                                        : item.artists?.map(a => a.name).join(', ') || ''
                                    }
                                </p>
                            </div>
                            <button className="btn-add">+ Agregar</button>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default FavoritesSelector;