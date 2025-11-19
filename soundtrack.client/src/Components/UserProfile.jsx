import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import AASGrid from './Common/AASGrid';
import FavoritesSelector from './Common/FavoritesSelector';
import { getArtistById, getAlbumById, getTrackById } from '../api/spotify';
import './UserProfile.css';

const UserProfile = () => {
    const { id } = useParams(); // Si viene de una ruta, sino usar el usuario actual
    const CURRENT_USER_ID = 1; // Usuario hardcodeado (cambiar por el real)
    const userId = id || CURRENT_USER_ID;

    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isEditing, setIsEditing] = useState(false);
    const [isEditingFavorites, setIsEditingFavorites] = useState(false);

    // Estados de edición
    const [editedProfile, setEditedProfile] = useState({
        username: '',
        email: '',
        bio: ''
    });

    // Estados de favoritos
    const [favoriteArtists, setFavoriteArtists] = useState([]);
    const [favoriteAlbums, setFavoriteAlbums] = useState([]);
    const [favoriteSongs, setFavoriteSongs] = useState([]);

    useEffect(() => {
        fetchUserProfile();
    }, [userId]);

    const fetchUserProfile = async () => {
        try {
            setLoading(true);
            
            const response = await fetch(`https://localhost:7232/api/user/${userId}/profile`);
            
            if (!response.ok) {
                throw new Error('Error al cargar perfil');
            }

            const data = await response.json();
            setUser(data);
            
            setEditedProfile({
                username: data.username,
                email: data.email,
                bio: data.bio || ''
            });

            // Cargar datos de favoritos desde Spotify
            await loadFavorites(data.favorites);
            
        } catch (error) {
            console.error('Error fetching user profile:', error);
        } finally {
            setLoading(false);
        }
    };

    const loadFavorites = async (favorites) => {
        try {
            // Cargar artistas favoritos
            if (favorites.artists && favorites.artists.length > 0) {
                const artistsData = await Promise.all(
                    favorites.artists.map(id => getArtistById(id))
                );
                setFavoriteArtists(artistsData.filter(a => a).map(a => ({
                    id: a.id,
                    name: a.name,
                    imageUrl: a.images[0]?.url,
                    subtitle: `${a.followers.total.toLocaleString()} seguidores`
                })));
            }

            // Cargar álbumes favoritos
            if (favorites.albums && favorites.albums.length > 0) {
                const albumsData = await Promise.all(
                    favorites.albums.map(id => getAlbumById(id))
                );
                setFavoriteAlbums(albumsData.filter(a => a).map(a => ({
                    id: a.id,
                    name: a.name,
                    imageUrl: a.images[0]?.url,
                    subtitle: a.artists.map(artist => artist.name).join(', ')
                })));
            }

            // Cargar canciones favoritas
            if (favorites.songs && favorites.songs.length > 0) {
                const songsData = await Promise.all(
                    favorites.songs.map(id => getTrackById(id))
                );
                setFavoriteSongs(songsData.filter(s => s).map(s => ({
                    id: s.id,
                    name: s.name,
                    imageUrl: s.album.images[0]?.url,
                    subtitle: s.artists.map(artist => artist.name).join(', ')
                })));
            }
        } catch (error) {
            console.error('Error loading favorites:', error);
        }
    };

    const handleProfileUpdate = async () => {
        try {
            const response = await fetch(`https://localhost:7232/api/user/${userId}/profile`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(editedProfile)
            });

            if (response.ok) {
                await fetchUserProfile();
                setIsEditing(false);
            } else {
                alert('Error al actualizar perfil');
            }
        } catch (error) {
            console.error('Error updating profile:', error);
            alert('Error al actualizar perfil');
        }
    };

    const handleFavoritesUpdate = async () => {
        try {
            const artistIds = favoriteArtists.map(a => a.id).join(',');
            const albumIds = favoriteAlbums.map(a => a.id).join(',');
            const songIds = favoriteSongs.map(s => s.id).join(',');

            const response = await fetch(`https://localhost:7232/api/user/${userId}/favorites`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    favoriteArtistIds: artistIds,
                    favoriteAlbumIds: albumIds,
                    favoriteSongIds: songIds
                })
            });

            if (response.ok) {
                setIsEditingFavorites(false);
                alert('Favoritos actualizados exitosamente');
            } else {
                alert('Error al actualizar favoritos');
            }
        } catch (error) {
            console.error('Error updating favorites:', error);
            alert('Error al actualizar favoritos');
        }
    };

    const addFavorite = (item, type) => {
        const favoriteItem = {
            id: item.id,
            name: item.name,
            imageUrl: item.images?.[0]?.url || item.album?.images?.[0]?.url,
            subtitle: type === 'artist' 
                ? `${item.followers?.total?.toLocaleString() || 0} seguidores`
                : item.artists?.map(a => a.name).join(', ') || ''
        };

        if (type === 'artist' && favoriteArtists.length < 3) {
            setFavoriteArtists([...favoriteArtists, favoriteItem]);
        } else if (type === 'album' && favoriteAlbums.length < 3) {
            setFavoriteAlbums([...favoriteAlbums, favoriteItem]);
        } else if (type === 'song' && favoriteSongs.length < 3) {
            setFavoriteSongs([...favoriteSongs, favoriteItem]);
        }
    };

    const removeFavorite = (id, type) => {
        if (type === 'artist') {
            setFavoriteArtists(favoriteArtists.filter(a => a.id !== id));
        } else if (type === 'album') {
            setFavoriteAlbums(favoriteAlbums.filter(a => a.id !== id));
        } else if (type === 'song') {
            setFavoriteSongs(favoriteSongs.filter(s => s.id !== id));
        }
    };

    if (loading) {
        return (
            <div className="loading-container">
                <div className="loading-spinner"></div>
                <p>Cargando perfil...</p>
            </div>
        );
    }

    if (!user) {
        return (
            <div className="error-container">
                <h2>Usuario no encontrado</h2>
                <p>Lo sentimos, no pudimos encontrar este usuario.</p>
            </div>
        );
    }

    return (
        <div className="user-profile-container">
            {/* Header del Perfil */}
            <div className="user-profile-header">
                <div className="profile-avatar-section">
                    <img 
                        src={user.profilePictureUrl || '/user_p.png'} 
                        alt={user.username}
                        className="profile-avatar-large"
                    />
                </div>

                <div className="profile-info-section">
                    {!isEditing ? (
                        <>
                            <h1 className="profile-username">{user.username}</h1>
                            <p className="profile-email">{user.email}</p>
                            {user.bio && <p className="profile-bio">{user.bio}</p>}
                            
                            <button 
                                className="btn-edit-profile"
                                onClick={() => setIsEditing(true)}
                            >
                                ✎ Editar Perfil
                            </button>
                        </>
                    ) : (
                        <div className="profile-edit-form">
                            <input
                                type="text"
                                value={editedProfile.username}
                                onChange={(e) => setEditedProfile({...editedProfile, username: e.target.value})}
                                placeholder="Nombre de usuario"
                                className="profile-input"
                            />
                            <input
                                type="email"
                                value={editedProfile.email}
                                onChange={(e) => setEditedProfile({...editedProfile, email: e.target.value})}
                                placeholder="Email"
                                className="profile-input"
                            />
                            <textarea
                                value={editedProfile.bio}
                                onChange={(e) => setEditedProfile({...editedProfile, bio: e.target.value})}
                                placeholder="Bio (Cuéntanos sobre ti...)"
                                className="profile-textarea"
                                rows="3"
                            />
                            <div className="profile-edit-actions">
                                <button className="btn-save" onClick={handleProfileUpdate}>
                                    Guardar
                                </button>
                                <button className="btn-cancel" onClick={() => setIsEditing(false)}>
                                    Cancelar
                                </button>
                            </div>
                        </div>
                    )}
                </div>
            </div>

            {/* Estadísticas */}
            <div className="user-stats">
                <div className="stat-card">
                    <div className="stat-value">{user.statistics.totalReviews}</div>
                    <div className="stat-label">Reviews</div>
                </div>
                <div className="stat-card">
                    <div className="stat-value">{user.statistics.followedArtists}</div>
                    <div className="stat-label">Artistas Seguidos</div>
                </div>
                <div className="stat-card">
                    <div className="stat-value">{user.statistics.followers}</div>
                    <div className="stat-label">Seguidores</div>
                </div>
                <div className="stat-card">
                    <div className="stat-value">{user.statistics.following}</div>
                    <div className="stat-label">Siguiendo</div>
                </div>
            </div>

            {/* Sección de Favoritos */}
            <div className="favorites-section">
                <div className="favorites-header">
                    <h2>Mis Favoritos</h2>
                    {!isEditingFavorites ? (
                        <button 
                            className="btn-edit-favorites"
                            onClick={() => setIsEditingFavorites(true)}
                        >
                            ✎ Editar Favoritos
                        </button>
                    ) : (
                        <div className="favorites-edit-actions">
                            <button className="btn-save" onClick={handleFavoritesUpdate}>
                                Guardar
                            </button>
                            <button className="btn-cancel" onClick={() => setIsEditingFavorites(false)}>
                                Cancelar
                            </button>
                        </div>
                    )}
                </div>

                {/* Artistas Favoritos */}
                <div className="favorites-category">
                    <h3>Artistas Top 3</h3>
                    {isEditingFavorites && (
                        <FavoritesSelector
                            type="artist"
                            onSelect={(item) => addFavorite(item, 'artist')}
                            currentCount={favoriteArtists.length}
                            maxCount={3}
                        />
                    )}
                    {favoriteArtists.length > 0 ? (
                        <AASGrid
                            items={favoriteArtists}
                            type="artist"
                            columns={3}
                            showRemoveButton={isEditingFavorites}
                            onRemove={(id) => removeFavorite(id, 'artist')}
                        />
                    ) : (
                        <p className="no-favorites">No has seleccionado artistas favoritos</p>
                    )}
                </div>

                {/* Álbumes Favoritos */}
                <div className="favorites-category">
                    <h3>Álbumes Top 3</h3>
                    {isEditingFavorites && (
                        <FavoritesSelector
                            type="album"
                            onSelect={(item) => addFavorite(item, 'album')}
                            currentCount={favoriteAlbums.length}
                            maxCount={3}
                        />
                    )}
                    {favoriteAlbums.length > 0 ? (
                        <AASGrid
                            items={favoriteAlbums}
                            type="album"
                            columns={3}
                            showRemoveButton={isEditingFavorites}
                            onRemove={(id) => removeFavorite(id, 'album')}
                        />
                    ) : (
                        <p className="no-favorites">No has seleccionado álbumes favoritos</p>
                    )}
                </div>

                {/* Canciones Favoritas */}
                <div className="favorites-category">
                    <h3>Canciones Top 3</h3>
                    {isEditingFavorites && (
                        <FavoritesSelector
                            type="song"
                            onSelect={(item) => addFavorite(item, 'song')}
                            currentCount={favoriteSongs.length}
                            maxCount={3}
                        />
                    )}
                    {favoriteSongs.length > 0 ? (
                        <AASGrid
                            items={favoriteSongs}
                            type="song"
                            columns={3}
                            showRemoveButton={isEditingFavorites}
                            onRemove={(id) => removeFavorite(id, 'song')}
                        />
                    ) : (
                        <p className="no-favorites">No has seleccionado canciones favoritas</p>
                    )}
                </div>
            </div>


        </div>
    );
};

export default UserProfile;