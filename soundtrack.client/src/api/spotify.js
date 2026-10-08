
//Para pedir el token de Spotify al backend
export const getSpotifyToken = async () => {
    try {
        
        const response = await fetch('/api/Spotify/token'); 

        if (!response.ok) throw new Error('Error del servidor');

        const data = await response.json();
        return data.access_token;
    } catch (error) {
        console.error(error);
        return null;
    }
};

export const getUserSpotifyToken = async () => {
    try {
        const response = await fetch('/api/Spotify/user-token', {
            credentials: 'include' // Envia cookies de autenticacion
        });

        if (!response.ok) {
            console.error('Error obteniendo token de usuario');
            return null;
        }

        const data = await response.json();
        return data.access_token;
    } catch (error) {
        console.error('Error:', error);
        return null;
    }
};

//Para usar el token de usuario si esta autenticado, sino el de la app
export const getToken = async () => {
    try {
        // Primero intentar con token de usuario
        const response = await fetch('/api/Spotify/user-token', {
            credentials: 'include'
        });

        if (response.ok) {
            const data = await response.json();
            console.log('Usando token de usuario');
            return data.access_token;
        }

        if (response.status === 401) {
            console.log('Usario no autenticado: Usando token de app');
            const appResponse = await fetch('/api/Spotify/token');

            if (!appResponse.ok) {
                throw new Error('Error del servidor al obtener token de app');
            }

            const appData = await appResponse.json();
            return appData.access_token;
        }
    } catch (error) {
        console.error('Error obteniendo token:', error);
        
    }
    try {// Fallback al token de la app
        console.log('Usando app token: Usuario autenticado sin cuenta Spotify');
        const appResponse = await fetch('/api/Spotify/token');
        const appData = await appResponse.json();
        return appData.access_token;
    }
    catch(fallbackError) {
        console.error('No se obtuvo ningun token de spotify', fallbackError);
        return null;
    }
};

   


// Lee la respuesta de Spotify. Si hubo error regresa null en vez de tronar con JSON.parse
// (Spotify a veces responde texto plano, por ejemplo el 403 de "Active premium subscription required")
const readSpotifyResponse = async (response) => {
    if (!response.ok) {
        const text = await response.text();
        console.error(`Spotify respondio ${response.status}:`, text);
        return null;
    }
    return response.json();
};

// Canciones populares del anio actual
// Desde feb 2026 Spotify solo deja leer playlists propias, por eso se usa la busqueda
export const getTopTracks = async () => {
    const token = await getToken();
    if (!token) return [];

    const year = new Date().getFullYear();
    const url = `https://api.spotify.com/v1/search?q=${encodeURIComponent(`year:${year}`)}&type=track&limit=10`;

    try {
        const response = await fetch(url, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const data = await readSpotifyResponse(response);
        return data?.tracks?.items || [];
    } catch (error) {
        console.error("Error en getTopTracks:", error);
        return [];
    }
};

// Top artistas sacados de las canciones populares
export const getTopArtists = async () => {
    const tracks = await getTopTracks();
    const token = await getToken();
    if (!token) return [];

    // Extraer artistas unicos de las canciones
    const artistIds = [...new Set(tracks.map(track => track.artists[0]?.id).filter(Boolean))].slice(0, 10);

    try {
        // Spotify quito el endpoint por lotes (/artists?ids=), se piden uno por uno
        const artists = await Promise.all(artistIds.map(id =>
            fetch(`https://api.spotify.com/v1/artists/${id}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            }).then(readSpotifyResponse)
        ));

        // Ya no se ordena por seguidores: Spotify quito el campo followers
        return artists.filter(Boolean);
    } catch (error) {
        console.error("Error en getTopArtists:", error);
        return [];
    }
};

export const getArtistById = async (artistId) => {
    const token = await getToken();
    
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
        
        const data = await readSpotifyResponse(response);
        
        if (!data) {
            console.error(" Error obteniendo artista:");
            return null;
        }
        
        console.log("Artista obtenido:", data);
        return data;
        
    } catch (error) {
        console.error("Error en getArtistById:", error);
        return null;
    }
};

// Spotify elimino /artists/{id}/top-tracks (feb 2026), se usa la busqueda por nombre
export const getArtistTopTracks = async (artistId, artistName) => {
    const token = await getToken();

    if (!token || !artistName) return [];

    try {
        const url = `https://api.spotify.com/v1/search?q=${encodeURIComponent(`artist:"${artistName}"`)}&type=track&limit=10`;
        const response = await fetch(url, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        const data = await readSpotifyResponse(response);

        if (!data) return [];

        // Solo canciones donde participa este artista (la busqueda puede traer homonimos)
        return (data.tracks?.items || []).filter(track =>
            track.artists.some(a => a.id === artistId)
        );

    } catch (error) {
        console.error("Error en getArtistTopTracks:", error);
        return [];
    }
};

export const getArtistAlbums = async (artistId) => {
    const token = await getToken();
    
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
        
        const data = await readSpotifyResponse(response);
        
        if (!data) {
            console.error("Error obteniendo albums:");
            return [];
        }
        
        return data.items || [];
        
    } catch (error) {
        console.error("Error en getArtistAlbums:", error);
        return [];
    }
};

export const getAlbumById = async (albumId) => {
    const token = await getToken();
    
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
        
        const data = await readSpotifyResponse(response);
        
        if (!data) {
            console.error("Error obteniendo album:");
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
    const token = await getToken();
    
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
        
        const data = await readSpotifyResponse(response);
        
        if (!data) {
            console.error("Error en busqueda:");
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

//Para los Trending tracks de los usuario
export const getTrackById = async (trackId) => {
    const token = await getToken();
    
    if (!token) {
        console.error("No token disponible");
        return null;
    }
    
    try {
        console.log("Obteniendo track, ID:", trackId);
        
        const response = await fetch(
            `https://api.spotify.com/v1/tracks/${trackId}`,
            {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            }
        );
        
        const data = await readSpotifyResponse(response);
        
        if (!data) {
            console.error("Error obteniendo track:");
            return null;
        }
        
        console.log("Track obtenido:", data);
        return data;
        
    } catch (error) {
        console.error("Error en getTrackById:", error);
        return null;
    }
};