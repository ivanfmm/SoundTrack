import React, { useEffect, useState } from 'react';
import AASGrid from './Common/AASGrid';
import TopRatedSection from './Common/TopRatedSection';
import TopReviewsSection from './Common/TopReviewsSection';
import { getTopTracks, getTopArtists } from '../api/spotify';

const Home = () => {
    const [topSongs, setTopSongs] = useState([]);
    const [topArtists, setTopArtists] = useState([]);
    const [loading, setLoading] = useState(true);
    
    // Nuevos estados para contenido de SoundTrack
    const [topRatedArtists, setTopRatedArtists] = useState([]);
    const [topRatedAlbums, setTopRatedAlbums] = useState([]);
    const [topRatedSongs, setTopRatedSongs] = useState([]);
    const [topReviews, setTopReviews] = useState([]);

    useEffect(() => {
        fetchSpotifyData();
        fetchSoundTrackData();
    }, []);

    // Fetch data de Spotify (original)
    const fetchSpotifyData = async () => {
        await Promise.all([fetchTopSongs(), fetchTopArtists()]);
    };

    // Fetch data de SoundTrack (nuevo)
    const fetchSoundTrackData = async () => {
        try {
            // Fetch top rated artists
            const artistsRes = await fetch('https://127.0.0.1:7232/api/Home/top-artists?count=5');
            if (artistsRes.ok) {
                const artistsData = await artistsRes.json();
                console.log("Artistas recibidos:", artistsData);
                setTopRatedArtists(artistsData);
            }

            // Fetch top rated albums
            const albumsRes = await fetch('https://127.0.0.1:7232/api/Home/top-albums?count=5');
            if (albumsRes.ok) {
                const albumsData = await albumsRes.json();
                console.log("Albums recibidos:", albumsRes);
                setTopRatedAlbums(albumsData);
            }

            // Fetch top rated songs
            const songsRes = await fetch('https://127.0.0.1:7232/api/Home/top-songs?count=5');
            if (songsRes.ok) {
                const songsData = await songsRes.json();
                console.log("Canciones recibidos:", songsRes);
                setTopRatedSongs(songsData);
            }

            // Fetch top reviews
            const reviewsRes = await fetch('https://127.0.0.1:7232/api/Home/top-reviews?count=5');
            if (reviewsRes.ok) {
                const reviewsData = await reviewsRes.json();
                setTopReviews(reviewsData);
            }
        } catch (error) {
            console.error('Error fetching SoundTrack data:', error);
        }
    };

    const fetchTopSongs = async () => {
        const check = localStorage.getItem("topSongs");
        
        if (check) {
            setTopSongs(JSON.parse(check));
        } else {
            try {
                const tracksData = await getTopTracks();
                const formattedSongs = tracksData.map(item => ({
                    id: item.track.id,
                    name: item.track.name,
                    imageUrl: item.track.album.images[0]?.url,
                    subtitle: item.track.artists[0].name,
                    score: 5
                }));
                
                localStorage.setItem("topSongs", JSON.stringify(formattedSongs));
                setTopSongs(formattedSongs);
            } catch (error) {
                console.error('Error fetching top songs:', error);
            }
        }
        setLoading(false);
    };

    const fetchTopArtists = async () => {
        const check = localStorage.getItem("topArtists");
        
        if (check) {
            setTopArtists(JSON.parse(check));
        } else {
            try {
                const artistsData = await getTopArtists();
                const formattedArtists = artistsData.map(artist => ({
                    id: artist.id,
                    name: artist.name,
                    imageUrl: artist.images[0]?.url,
                    subtitle: `${artist.followers.total.toLocaleString()} seguidores`
                }));
                
                localStorage.setItem("topArtists", JSON.stringify(formattedArtists));
                setTopArtists(formattedArtists);
            } catch (error) {
                console.error('Error fetching top artists:', error);
            }
        }
    };

    if (loading) {
        return (
            <div className="loading-container">
                <div className="loading-spinner"></div>
                <p>Cargando música...</p>
            </div>
        );
    }

    return (
        <div style={{ padding: '2rem 0' }}>
            {/* Sección de bienvenida */}
            <div style={{ marginBottom: '3rem' }}>
                <h1 style={{ fontSize: '2.5rem', fontWeight: '700', marginBottom: '0.5rem' }}>
                    Bienvenido a SoundTrack
                </h1>
                <p style={{ fontSize: '1.1rem', color: '#888' }}>
                    Descubre y comparte tu pasión por la música
                </p>
            </div>

            {/* Top Rated de SoundTrack */}
            <TopRatedSection 
                title="🎤 Top Artistas en SoundTrack"
                items={topRatedArtists}
                type="artist"
            />

            <TopRatedSection 
                title="💿 Top Álbumes en SoundTrack"
                items={topRatedAlbums}
                type="album"
            />

            <TopRatedSection 
                title="🎵 Top Canciones en SoundTrack"
                items={topRatedSongs}
                type="song"
            />

            {/* Top Reviews */}
            <TopReviewsSection reviews={topReviews} />

            {/* Contenido de Spotify (original) */}
            <div style={{ marginTop: '4rem', paddingTop: '2rem', borderTop: '2px solid rgba(255,255,255,0.1)' }}>
                <h2 style={{ fontSize: '2rem', fontWeight: '700', marginBottom: '1rem' }}>
                    🔥 Tendencias de Spotify
                </h2>
            </div>

             <AASGrid 
                items={topArtists}
                type="artist"
                title="🎤 Top Artistas en Spotify"
                columns={5}
            />

            <AASGrid 
                items={topSongs}
                type="song"
                title="🎵 Top Canciones en Spotify"
                columns={5}
            />
        </div>
    );
};

export default Home;