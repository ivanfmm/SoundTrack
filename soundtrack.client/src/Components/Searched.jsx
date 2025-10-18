import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import AASGrid from './Common/AASGrid';
import { searchSpotify } from '../api/spotify';
import './Searched.css';

const Searched = () => {
    const { query } = useParams();
    const [results, setResults] = useState({
        tracks: [],
        artists: [],
        albums: []
    });
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        searchData();
    }, [query]);

    const searchData = async () => {
        setLoading(true);
        
        const data = await searchSpotify(query);
        
        // Formatear resultados para AASGrid
        const formattedResults = {
            tracks: data.tracks.map(track => ({
                id: track.id,
                name: track.name,
                imageUrl: track.album.images[0]?.url,
                subtitle: track.artists.map(a => a.name).join(', '),
                score: 5
            })),
            artists: data.artists.map(artist => ({
                id: artist.id,
                name: artist.name,
                imageUrl: artist.images[0]?.url,
                subtitle: `${artist.followers.total.toLocaleString()} seguidores`
            })),
            albums: data.albums.map(album => ({
                id: album.id,
                name: album.name,
                imageUrl: album.images[0]?.url,
                subtitle: album.artists.map(a => a.name).join(', ')
            }))
        };
        
        setResults(formattedResults);
        setLoading(false);
    };

    if (loading) {
        return (
            <div className="loading-container">
                <div className="loading-spinner"></div>
                <p>Buscando...</p>
            </div>
        );
    }

    const totalResults = results.tracks.length + results.artists.length + results.albums.length;

    if (totalResults === 0) {
        return (
            <div className="no-results">
                <h2>No se encontraron resultados para "{query}"</h2>
                <p>Intenta con otros términos de búsqueda</p>
            </div>
        );
    }

    return (
        <div className="searched-container">
            <div className="search-header">
                <h1>Resultados para: <span className="query-text">"{query}"</span></h1>
                <p className="results-count">{totalResults} resultados encontrados</p>
            </div>

            {results.tracks.length > 0 && (
                <AASGrid
                    items={results.tracks}
                    type="song"
                    title="Canciones"
                    columns={5}
                />
            )}

            {results.artists.length > 0 && (
                <AASGrid
                    items={results.artists}
                    type="artist"
                    title="Artistas"
                    columns={5}
                />
            )}

            {results.albums.length > 0 && (
                <AASGrid
                    items={results.albums}
                    type="album"
                    title="Albumes"
                    columns={5}
                />
            )}
        </div>
    );
};

export default Searched;