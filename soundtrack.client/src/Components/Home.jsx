import React, { useEffect, useState } from 'react';
import AASGrid from './Common/AASGrid';
import { getTopTracks, getTopArtists } from '../api/spotify';

const Home = () => {
    const [topSongs, setTopSongs] = useState([]);
    const [topArtists, setTopArtists] = useState([]);
    const [loading, setLoading] = useState(true);

    
    useEffect(() => {
        fetchTopSongs();
        fetchTopArtists();
    }, []);

    const fetchTopSongs = async () => {
        // Revisar si ya estan en localStorage
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
                
                // Guardar en localStorage
                localStorage.setItem("topSongs", JSON.stringify(formattedSongs));
                setTopSongs(formattedSongs);
            } catch (error) {
                console.error('Error fetching top songs:', error);
            }
        }
        setLoading(false);
    };

    const fetchTopArtists = async () => {
        // Revisar si ya estan en localStorage
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
                
                // Guardar en localStorage
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
        {console.log(topSongs)}
            <AASGrid 
                items={topSongs}
                type="song"
                title="Top Canciones"
                columns={5}
            />
            {console.log(topArtists)}
            <AASGrid 
                items={topArtists}
                type="artist"
                title="Top Artistas"
                columns={5}
            />
        </div>
    );
};

export default Home;