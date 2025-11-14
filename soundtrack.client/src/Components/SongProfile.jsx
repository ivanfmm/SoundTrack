import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import ProfileHeader from './Common/ProfileHeader';
import ReviewForm from './ReviewForm';
import ReviewsList from './ReviewList';
import './Common/Common.css';
import './SongProfile.css';
import { getSpotifyToken } from '../api/spotify';

const SongProfile = () => {
    const { id } = useParams();
    const [song, setSong] = useState(null);
    const [loading, setLoading] = useState(true);
    const [showReviewForm, setShowReviewForm] = useState(false);


    {
        //NOSE PORQUE DA ERROR O ADVERTENCIA EL BENDITO ID
    }
    useEffect(() => {
        fetchSongData();
    }, [id]);

    //quitar cuando el api este funcionando (TEMPORAL!!!!!!!!!!!!!!!!)
    //Se cancela borre el otro y este es el chido :)
     const fetchSongData = async () => {
        try {
            setLoading(true);
            
            //Buscar el token de spotify (no se si sea necesario aqui)
            const token = await getSpotifyToken();

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
                        score: 5,
                        publicationDate: data.album.release_date,
                        generes:[],
                        tags:[],
                        artists: data.artists,
                        album: data.album,
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

    return (
        <div className="song-profile-container">
            <ProfileHeader
                imageUrl={song.imageUrl}
                title={song.name}
                metadata={metadata}
                score={song.score}
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