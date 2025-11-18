
import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import ProfileHeader from './Common/ProfileHeader';
import AASGrid from './Common/AASGrid';
import ReviewForm from './ReviewForm';
import ReviewsList from './ReviewList';
import { getArtistById, getArtistTopTracks, getArtistAlbums } from '../api/spotify';
import './Common/Common.css';
import './ArtistProfile.css';

const ArtistProfile = () => {
    const { id } = useParams();
    const [artist, setArtist] = useState(null);
    const [topTracks, setTopTracks] = useState([]);
    const [albums, setAlbums] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showReviewForm, setShowReviewForm] = useState(false);

    useEffect(() => {
        fetchArtistData();
    }, [id]);

    const fetchArtistData = async () => {
        try {
            setLoading(true);
            
            // Obtener datos del artista
            const artistData = await getArtistById(id);
            
            if (!artistData) {
                setLoading(false);
                return;
            }
            
            // Transformar datos al formato esperado
            const formattedArtist = {
                id: artistData.id,
                name: artistData.name,
                imageUrl: artistData.images[0]?.url || '/placeholder.png',
                score: 5, 
                description: `${artistData.name} tiene ${artistData.followers.total.toLocaleString()} seguidores en Spotify.`,
                genres: artistData.genres || [],
                tags: [],
                followers: artistData.followers.total,
                popularity: artistData.popularity
            };
            
            setArtist(formattedArtist);
            
            // Obtener top tracks del artista
            const tracksData = await getArtistTopTracks(id);
            const formattedTracks = tracksData.slice(0, 5).map(track => ({
                id: track.id,
                name: track.name,
                imageUrl: track.album.images[0]?.url,
                subtitle: track.album.name,
                score: 5
            }));
            setTopTracks(formattedTracks);
            
            // Obtener albumes del artista
            const albumsData = await getArtistAlbums(id);
            const formattedAlbums = albumsData.map(album => ({
                id: album.id,
                name: album.name,
                imageUrl: album.images[0]?.url,
                subtitle: new Date(album.release_date).getFullYear().toString()
            }));
            setAlbums(formattedAlbums);
            
            setLoading(false);
            
        } catch (error) {
            console.error('Error fetching artist data:', error);
            setLoading(false);
        }
    };

    //copy paste del de caciones
    const handleSubmitReview = async (savedReview) => {
        console.log('Review guardada exitosamente:', savedReview);
        setShowReviewForm(false);
        // Forzar recarga de las reviews
        window.location.reload();
    };

    if (loading) {
        return (
            <div className="loading-container">
                <div className="loading-spinner"></div>
                <p>Cargando artista...</p>
            </div>
        );
    }

    if (!artist) {
        return (
            <div className="error-container">
                <h2>Artista no encontrado </h2>
                <p>Lo sentimos, no pudimos encontrar este artista.</p>
            </div>
        );
    }

    const metadata = [
        {
            label: 'Popularidad',
            value: `${artist.popularity}/100`
        },
        {
            label: 'Seguidores',
            value: artist.followers.toLocaleString()
        }
    ];

    return (
        <div className="artist-profile-container">
            <ProfileHeader
                imageUrl={artist.imageUrl}
                title={artist.name}
                metadata={metadata}
                score={artist.score}
                genres={artist.genres}
                tags={artist.tags}
                description={artist.description}
            />

            {/* Estadisticas del artista */}
            <div className="artist-stats">
                <div className="stat-card">
                    <div className="stat-value">{artist.followers.toLocaleString()}</div>
                    <div className="stat-label">Seguidores</div>
                </div>
                <div className="stat-card">
                    <div className="stat-value">{artist.popularity}</div>
                    <div className="stat-label">Popularidad</div>
                </div>
                <div className="stat-card">
                    <div className="stat-value">{topTracks.length}</div>
                    <div className="stat-label">Top Canciones</div>
                </div>
                <div className="stat-card">
                    <div className="stat-value">{albums.length}</div>
                    <div className="stat-label">Álbumes</div>
                </div>
            </div>

            {/* Secciones de contenido */}
            <div className="artist-sections">
                {topTracks.length > 0 && (
                    <AASGrid
                        items={topTracks}
                        type="song"
                        title="🎵 Canciones Populares"
                        columns={5}
                    />
                )}

                {albums.length > 0 && (
                    <AASGrid
                        items={albums}
                        type="album"
                        title="💿 Álbumes"
                        columns={5}
                    />
                )}
            </div>

            {/* Reviews */}
            <div className="review-actions">
                <button 
                    className="btn-create-review"
                    onClick={() => setShowReviewForm(!showReviewForm)}
                >
                    {showReviewForm ? '✕ Cancelar' : '✎ Escribir Review'}
                </button>
            </div>

            {showReviewForm && (
                <ReviewForm 
                    onSubmit={handleSubmitReview}
                    onCancel={() => setShowReviewForm(false)}
                    profileId={id}
                    profileType="artist"
                />
            )}

            <ReviewsList 
                profileId={id}
                profileType="artist"
            />
        </div>
    );
};

export default ArtistProfile;