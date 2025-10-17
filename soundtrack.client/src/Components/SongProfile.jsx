import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import ProfileHeader from './Common/ProfileHeader';
import ReviewForm from './ReviewForm';
import ReviewsList from './ReviewList';
import './Common/Common.css';
import './SongProfile.css';

//temporal se quitara cuando el api jale (solo el mockdata)
import {mockSongs } from '../data/mockData';

const SongProfile = () => {
    const { id } = useParams();
    const [song, setSong] = useState(null);
    const [reviews, setReviews] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showReviewForm, setShowReviewForm] = useState(false);


    {
        //NOSE PORQUE DA ERROR O ADVERTENCIA EL BENDITO ID
    }
    useEffect(() => {
        fetchSongData();
    }, [id]);

    /*
    //creo que si jalaria si estuviera el api (segun gemini esos son los formatos del data que necesitamos)
    //La nt esto si no se me rendi y nada mas deje lo que creo que funcionara
    const fetchSongData = async () => {
        try {
            const response = await fetch(`/Song/${id}`);
            const data = await response.json();
            setSong(data);
            setReviews(data.reviews || []);
            setLoading(false);
        } catch (error) {
            console.error('Error fetching song data:', error);
            setLoading(false);
        }
    };

    const handleSubmitReview = async (reviewData) => {
        try {
            const payload = {
                userId: 1,
                author: "Pepito43",
                description: reviewData.description,
                score: reviewData.score,
                publicationDate: new Date().toISOString(),
                likes: 0,
                dislikes: 0
            };

            const response = await fetch('/Review', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                fetchSongData();
                setShowReviewForm(false);
            }
        } catch (error) {
            console.error('Error submitting review:', error);
        }
    };
    */

    //quitar cuando el api este funcionando (TEMPORAL!!!!!!!!!!!!!!!!)
     const fetchSongData = async () => {
        try {
            // Simular una llamada a API con un delay
            await new Promise(resolve => setTimeout(resolve, 500));
            
            // Buscar la cancion en los datos mock
            const foundSong = mockSongs.find(s => s.id === id);
            
            if (foundSong) {
                setSong(foundSong);
                setReviews(foundSong.reviews || []);
            }
            
            setLoading(false);
        } catch (error) {
            console.error('Error fetching song data:', error);
            setLoading(false);
        }
    };

    const handleSubmitReview = async (reviewData) => {
        try {
            // Simular guardado de review
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

            // Agregar la nueva review al estado
            setReviews([newReview, ...reviews]);
            setShowReviewForm(false);

            // Recalcular el score promedio
            const allReviews = [newReview, ...reviews];
            const avgScore = Math.round(
                allReviews.reduce((sum, r) => sum + r.score, 0) / allReviews.length
            );
            setSong({ ...song, score: avgScore });

            console.log('Review creada:', newReview);
        } catch (error) {
            console.error('Error submitting review:', error);
        }
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
                />
            )}

            <ReviewsList reviews={reviews} />
        </div>
    );
};

export default SongProfile;