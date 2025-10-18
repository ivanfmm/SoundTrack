import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import ProfileHeader from './Common/ProfileHeader';
import ReviewForm from './ReviewForm';
import ReviewsList from './ReviewList';
import { getAlbumById } from '../api/spotify';
import './Common/Common.css';
import './AlbumProfile.css';
import StarRating from './Common/StarRating';

const AlbumProfile = () => {
    const { id } = useParams();
    const [album, setAlbum] = useState(null);
    const [reviews, setReviews] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showReviewForm, setShowReviewForm] = useState(false);

    useEffect(() => {
        fetchAlbumData();
    }, [id]);

    const fetchAlbumData = async () => {
        try {
            setLoading(true);
            
            const albumData = await getAlbumById(id);
            
            if (!albumData) {
                setLoading(false);
                return;
            }
            
         
            
            
          
            const formattedAlbum = {
                id: albumData.id,
                name: albumData.name,
                imageUrl: albumData.images[0]?.url || '/placeholder.png',
                score: 5, 
                description: `Álbum lanzado el ${new Date(albumData.release_date).toLocaleDateString('es-ES')} con ${albumData.total_tracks} canciones.`,
                genres: albumData.genres || [],
                tags: [],
                artists: albumData.artists,
                releaseDate: albumData.release_date,
                totalTracks: albumData.total_tracks,
                tracks: albumData.tracks.items,
                label: albumData.label,
                popularity: albumData.popularity
            };
            
            setAlbum(formattedAlbum);
            setLoading(false);
            
        } catch (error) {
            console.error('Error fetching album data:', error);
            setLoading(false);
        }
    };

    const handleSubmitReview = async (reviewData) => {
        try {
            const newReview = {
                id: reviews.length + 1,
                userId: 1,
                author: "Pepito43",
                description: reviewData.description,
                score: reviewData.score,
                publicationDate: new Date().toISOString(),
                likes: 0,
                dislikes: 0
            };

            setReviews([newReview, ...reviews]);
            setShowReviewForm(false);

            // Recalcular score promedio
            const allReviews = [newReview, ...reviews];
            const avgScore = Math.round(
                allReviews.reduce((sum, r) => sum + r.score, 0) / allReviews.length
            );
            setAlbum({ ...album, score: avgScore });

            console.log('Review creada:', newReview);
        } catch (error) {
            console.error('Error submitting review:', error);
        }
    };

  

    if (loading) {
        return (
            <div className="loading-container">
                <div className="loading-spinner"></div>
                <p>Cargando álbum...</p>
            </div>
        );
    }

    if (!album) {
        return (
            <div className="error-container">
                <h2>Álbum no encontrado</h2>
                <p>Lo sentimos, no pudimos encontrar este album.</p>
            </div>
        );
    }

    const metadata = [
        {
            label: 'Artista',
            value: album.artists.map(artist => artist.name).join(', ')
        },
        {
            label: 'Fecha de lanzamiento',
            value: new Date(album.releaseDate).toLocaleDateString('es-ES')
        },
        {
            label: 'Total de canciones',
            value: album.totalTracks
        },
        
        album.label && {
            label: 'Disquera',
            value: album.label
        }
    ].filter(Boolean);

    return (
        <div className="album-profile-container">
            <ProfileHeader
                imageUrl={album.imageUrl}
                title={album.name}
                subtitle={album.artists.map(a => a.name).join(', ')}
                metadata={metadata}
                score={album.score}
                genres={album.genres}
                tags={album.tags}
                description={album.description}
            />

            {/* Estadisticas del album */}
            <div className="album-stats">
                <div className="stat-card">
                    <div className="stat-value">{album.totalTracks}</div>
                    <div className="stat-label">Canciones</div>
                </div>
            
                <div className="stat-card">
                    <div className="stat-value">
                        {new Date(album.releaseDate).getFullYear()}
                    </div>
                    <div className="stat-label">Año</div>
                </div>
                {album.popularity && (
                    <div className="stat-card">
                        <div className="stat-value">{album.popularity}</div>
                        <div className="stat-label">Popularidad</div>
                    </div>
                )}
            </div>

          {/* Lista de canciones */}
<div className="album-tracks-section">
    <h2 className="album-tracks-title">🎵 Canciones</h2>
    <div className="track-list">
        {album.tracks.map((track, index) => (
            <Link 
                key={track.id} 
                to={`/song/${track.id}`}
                className="track-item"
            >
                <div className="track-left">
                    <div className="track-number">{index + 1}</div>
                    <div className="track-info">
                        <div className="track-name">{track.name}</div>
                        <div className="track-artists">
                            {track.artists.map(a => a.name).join(', ')}
                        </div>
                    </div>
                </div>
                
                <div className="track-right">
                    <div className="track-rating">
                        <StarRating score={4} size="small" />
                    </div>
                    
                </div>
            </Link>
        ))}
    </div>
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
                />
            )}

            <ReviewsList reviews={reviews} />
        </div>
    );
};

export default AlbumProfile;