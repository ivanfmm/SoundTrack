
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
    const [averageScore, setAverageScore] = useState(null);
    const [totalReviews, setTotalReviews] = useState(0);

    useEffect(() => {
        fetchArtistData();
        fetchAverageScore();
    }, [id]);

    const fetchAverageScore = async () => {
        try {
            const response = await fetch(
                `/api/review/average-score/${id}?profileType=artist`
            );
            
            if (response.ok) {
                const data = await response.json();
                setAverageScore(data.averageScore);
                setTotalReviews(data.totalReviews);
            }
        } catch (error) {
            console.error('Error al cargar promedio:', error);
        }
    };

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
                // Spotify quito followers y popularity del artista (feb 2026)
                description: `${artistData.name} en Spotify.`,
                genres: artistData.genres || [],
                tags: [],
            };
            
            setArtist(formattedArtist);
            
            // Obtener top tracks del artista
            const tracksData = await getArtistTopTracks(id, artistData.name);
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
        await fetchAverageScore();
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

    // Popularidad y seguidores ya no vienen en la API de Spotify
    const metadata = [];

    return (
        <div className="artist-profile-container">
            <ProfileHeader
                imageUrl={artist.imageUrl}
                title={artist.name}
                metadata={metadata}
                score={averageScore}
                totalReviews={totalReviews}
                genres={artist.genres}
                tags={artist.tags}
                description={artist.description}
            />

            {/* Estadisticas del artista */}
            <div className="artist-stats">
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