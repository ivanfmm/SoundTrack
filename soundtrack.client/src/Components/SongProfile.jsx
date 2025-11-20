import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import ProfileHeader from './Common/ProfileHeader';
import ReviewForm from './ReviewForm';
import ReviewsList from './ReviewList';
import './Common/Common.css';
import './SongProfile.css';
import { getToken } from '../api/spotify';
import AudioPlayer from './Common/AudioPlayer';

const SongProfile = () => {
    const { id } = useParams();
    const [song, setSong] = useState(null);
    const [loading, setLoading] = useState(true);
    const [showReviewForm, setShowReviewForm] = useState(false);
    const [averageScore, setAverageScore] = useState(null);
    const [totalReviews, setTotalReviews] = useState(0);


    {
        //NOSE PORQUE DA ERROR O ADVERTENCIA EL BENDITO ID
    }
    useEffect(() => {
        fetchSongData();
        fetchAverageScore();
    }, [id]);

    const fetchAverageScore = async () => {
        try {
            const response = await fetch(
                `https://localhost:7232/api/review/average-score/${id}?profileType=song`
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

    //quitar cuando el api este funcionando (TEMPORAL!!!!!!!!!!!!!!!!)
    //Se cancela borre el otro y este es el chido :)
     const fetchSongData = async () => {
        try {
            setLoading(true);
            
            //Buscar el token de spotify (no se si sea necesario aqui)
            const token = await getToken();

            if(token){
                const response = await fetch(`https://api.spotify.com/v1/tracks/${id}`,{
                    headers: {
                        'Authorization': `Bearer ${token}`
                }
                });
             
                const data = await response.json();
                if(data && !data.error){
                    const cancionSpotify={
                        id: data.id,
                        name: data.name,
                        imageUrl: data.album.images[0]?.url,
                        score: 0,
                        publicationDate: data.album.release_date,
                        generes:[],
                        tags:[],
                        artists: data.artists,
                        album: data.album,
                        preview_url: data.preview_url,
                        external_urls: data.external_urls,
                        description: `Canción del álbum ${data.album.name} lanzado el ${new Date(data.album.release_date).toLocaleDateString('es-ES')}`
                    };

                    setSong(cancionSpotify);
                } else {
                    console.error('Error al obtener datos de Spotify:', data.error);
                }
            }
            
            setLoading(false);
        } catch (error) {
            console.error('Error fetching song data:', error);
            setLoading(false);
        }
    };
    const handleSubmitReview = async (savedReview) => {
        console.log('Review guardada exitosamente:', savedReview);
        setShowReviewForm(false);
        //Volver a calcular el promedio
        await fetchAverageScore();
        // Forzar recarga de las reviews
        window.location.reload();
    };

    //HASTA AQUI, esto ya si es parte de mi chamba

    if (loading) {
        return (
            <div className="loading-container">
                <div className="loading-spinner"></div>
                <p>Cargando cancion...</p>
            </div>
                //Diego ya te la sabes loop infinito si no esta en la base de datos
            
        );
    }

    //version aburrida decir que no la tenemos
    if (!song) {
        return (
            <div className="error-container">
                <h2>Cancion no encontrada ;(</h2>
                <p>Lo sentimos, no pudimos encontrar esta cancion :( </p>
            </div>
        );
    }

    {
        //estructura del meta data
    }
    const metadata = [
        song.artists?.length > 0 && {
            label: 'Artista',
            value: song.artists.map(artist => artist.name).join(', ')
        },
        song.album && {
            label: 'Album',
            value: song.album.name
        },
        {
            label: 'Fecha de publicacion',
            value: new Date(song.publicationDate).toLocaleDateString('es-ES')
        }
    ].filter(Boolean);

         //Reproductor preview
    <AudioPlayer
        previewUrl={song.preview_url}
                trackName={song.name}
            />

            {/* Boton para abrir en Spotify */}
            <div className="spotify-actions">
                <a 
                    href={song.external_urls?.spotify}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="spotify-button"
                >
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M12 0C5.4 0 0 5.4 0 12s5.4 12 12 12 12-5.4 12-12S18.66 0 12 0zm5.521 17.34c-.24.359-.66.48-1.021.24-2.82-1.74-6.36-2.101-10.561-1.141-.418.122-.779-.179-.899-.539-.12-.421.18-.78.54-.9 4.56-1.021 8.52-.6 11.64 1.32.42.18.479.659.301 1.02zm1.44-3.3c-.301.42-.841.6-1.262.3-3.239-1.98-8.159-2.58-11.939-1.38-.479.12-1.02-.12-1.14-.6-.12-.48.12-1.021.6-1.141C9.6 9.9 15 10.561 18.72 12.84c.361.181.54.78.241 1.2zm.12-3.36C15.24 8.4 8.82 8.16 5.16 9.301c-.6.179-1.2-.181-1.38-.721-.18-.601.18-1.2.72-1.381 4.26-1.26 11.28-1.02 15.721 1.621.539.3.719 1.02.419 1.56-.299.421-1.02.599-1.559.3z"/>
                    </svg>
                    Escuchar completa en Spotify
                </a>
                
                {!song.preview_url && (
                    <p className="premium-note">
                        Esta canción solo esta disponible completa en Spotify Premium
                    </p>
                )}
            </div>

    return (
        <div className="song-profile-container">
            <ProfileHeader
                imageUrl={song.imageUrl}
                title={song.name}
                metadata={metadata}
                score={averageScore}
                totalReviews={totalReviews}
                genres={song.genres || []}
                tags={song.tags || []}
                description={song.description}
            />

            <div className="review-actions">
                <button 
                    className="btn-create-review"
                    onClick={() => setShowReviewForm(!showReviewForm)}
                >
                    {
                        //si me la pase 5 min buscando el lapiz pa que se viera perro
                    }
                    {showReviewForm ? '✕ Cancelar' : '✎ Escribir Review'}
                </button>
            </div>

            {showReviewForm && (
                <ReviewForm 
                    onSubmit={handleSubmitReview}
                    onCancel={() => setShowReviewForm(false)}
                    profileId={id}
                    profileType="song"
                />
            )}

            <ReviewsList 
                profileId={id}
                profileType="song"
            />
        </div>
    );
}

export default SongProfile;