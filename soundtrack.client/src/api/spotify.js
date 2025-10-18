const CLIENT_ID = import.meta.env.VITE_SPOTIFY_CLIENT_ID;
const CLIENT_SECRET = import.meta.env.VITE_SPOTIFY_CLIENT_SECRET;

export const getSpotifyToken = async () => {
    
    
    const response = await fetch('https://accounts.spotify.com/api/token', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'Authorization': 'Basic ' + btoa(CLIENT_ID + ':' + CLIENT_SECRET)
        },
        body: 'grant_type=client_credentials'
    });
    
    const data = await response.json();
    
    
    return data.access_token;
};

export const getTopTracks = async () => {
    const token = await getSpotifyToken();
    
    if (!token) return [];
    
    // Usar la playlist de artistas que SÍ funciona
    const url = 'https://api.spotify.com/v1/playlists/5iwkYfnHAGMEFLiHFFGnP4/tracks?limit=10';
    
    
    const response = await fetch(url, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });
    
    const data = await response.json();
    
    
    return data.items;
};
// Top 10 Artistas de la playlist "Most Followed Artists"
export const getTopArtists = async () => {
    const token = await getSpotifyToken();
    
    
    if (!token) return [];
    
    // Playlist de artistas mas seguidos
    const url = 'https://api.spotify.com/v1/playlists/4i96DEnCkGkhBRcI9SYuc4/tracks?limit=10';
    
    
    const response = await fetch(url, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });
    
    const data = await response.json();
    
    
    // Extraer artistas unicos de las canciones
    const artistsMap = new Map();
    data.items.forEach(item => {
        if (item.track && item.track.artists) {
            const artist = item.track.artists[0];
            if (!artistsMap.has(artist.id)) {
                artistsMap.set(artist.id, artist);
            }
        }
    });
    
    const uniqueArtists = Array.from(artistsMap.values()).slice(0, 10);
    
    
    // Obtener detalles completos de cada artista (imagenes, seguidores)
    const artistDetailsPromises = uniqueArtists.map(artist => 
        fetch(`https://api.spotify.com/v1/artists/${artist.id}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        }).then(res => res.json())
    );
    
    const artistsDetails = await Promise.all(artistDetailsPromises);
    
    
    // Ordenar por seguidores (de mayor a menor)
    artistsDetails.sort((a, b) => b.followers.total - a.followers.total);
    
    return artistsDetails.slice(0, 10);
};

export const getArtistById = async (artistId) => {
    const token = await getSpotifyToken();
    
    if (!token) {
        console.error("No token disponible");
        return null;
    }
    
    try {
        console.log("Obteniendo artista, ID:", artistId);
        
        const response = await fetch(
            `https://api.spotify.com/v1/artists/${artistId}`,
            {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            }
        );
        
        const data = await response.json();
        
        if (data.error) {
            console.error(" Error obteniendo artista:", data.error);
            return null;
        }
        
        console.log("Artista obtenido:", data);
        return data;
        
    } catch (error) {
        console.error("Error en getArtistById:", error);
        return null;
    }
};

export const getArtistTopTracks = async (artistId) => {
    const token = await getSpotifyToken();
    
    if (!token) return [];
    
    try {
        const response = await fetch(
            `https://api.spotify.com/v1/artists/${artistId}/top-tracks?market=US`,
            {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            }
        );
        
        const data = await response.json();
        
        if (data.error) {
            console.error("Error obteniendo top tracks:", data.error);
            return [];
        }
        
        return data.tracks || [];
        
    } catch (error) {
        console.error("Error en getArtistTopTracks:", error);
        return [];
    }
};

export const getArtistAlbums = async (artistId) => {
    const token = await getSpotifyToken();
    
    if (!token) return [];
    
    try {
        const response = await fetch(
            `https://api.spotify.com/v1/artists/${artistId}/albums?limit=10&market=US`,
            {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            }
        );
        
        const data = await response.json();
        
        if (data.error) {
            console.error("Error obteniendo albums:", data.error);
            return [];
        }
        
        return data.items || [];
        
    } catch (error) {
        console.error("Error en getArtistAlbums:", error);
        return [];
    }
};

export const getAlbumById = async (albumId) => {
    const token = await getSpotifyToken();
    
    if (!token) {
        console.error("No token disponible");
        return null;
    }
    
    try {
        console.log("Obteniendo album, ID:", albumId);
        
        const response = await fetch(
            `https://api.spotify.com/v1/albums/${albumId}`,
            {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            }
        );
        
        const data = await response.json();
        
        if (data.error) {
            console.error("Error obteniendo album:", data.error);
            return null;
        }
        
        console.log("Album obtenido:", data);
        return data;
        
    } catch (error) {
        console.error("Error en getAlbumById:", error);
        return null;
    }
};


export const searchSpotify = async (query) => {
    const token = await getSpotifyToken();
    
    if (!token || !query) {
        console.error("No token o query vacio");
        return { tracks: [], artists: [], albums: [] };
    }
    
    try {
        console.log("Buscando:", query);
        
        
        const url = `https://api.spotify.com/v1/search?q=${encodeURIComponent(query)}&type=track,artist,album&limit=10`;
        
        const response = await fetch(url, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        
        const data = await response.json();
        
        if (data.error) {
            console.error("Error en busqueda:", data.error);
            return { tracks: [], artists: [], albums: [] };
        }
        
        console.log("Resultados encontrados:", {
            tracks: data.tracks?.items?.length || 0,
            artists: data.artists?.items?.length || 0,
            albums: data.albums?.items?.length || 0
        });
        
        return {
            tracks: data.tracks?.items || [],
            artists: data.artists?.items || [],
            albums: data.albums?.items || []
        };
        
    } catch (error) {
        console.error("Error en searchSpotify:", error);
        return { tracks: [], artists: [], albums: [] };
    }
};