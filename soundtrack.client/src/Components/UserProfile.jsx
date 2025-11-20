import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useAuth } from './AuthContext';
import AASGrid from './Common/AASGrid';
import FavoritesSelector from './Common/FavoritesSelector';
import { getArtistById, getAlbumById, getTrackById } from '../api/spotify';
import './UserProfile.css';

const UserProfile = () => {
    const { id } = useParams();
    const { user: currentUser, isAuthenticated } = useAuth();

    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isEditing, setIsEditing] = useState(false);
    const [isEditingFavorites, setIsEditingFavorites] = useState(false);

    const [editedProfile, setEditedProfile] = useState({
        username: '',
        email: '',
        bio: ''
    });

    const [favoriteArtists, setFavoriteArtists] = useState([]);
    const [favoriteAlbums, setFavoriteAlbums] = useState([]);
    const [favoriteSongs, setFavoriteSongs] = useState([]);

    // Estados para el sistema de seguimiento
    const [isFollowing, setIsFollowing] = useState(false);
    const [followLoading, setFollowLoading] = useState(false);

    const canEdit = isAuthenticated && currentUser && user &&
        (currentUser.userId === user.id || currentUser.userId === parseInt(id));

    useEffect(() => {
        const profileId = id || currentUser?.userId;
        if (profileId) {
            fetchUserProfile(profileId);
        }
    }, [id, currentUser]);

    // Verificar si el usuario actual sigue a este perfil
    useEffect(() => {
        if (isAuthenticated && currentUser && user && !canEdit) {
            checkIfFollowing();
        }
    }, [user, currentUser, isAuthenticated, canEdit]);

    const fetchUserProfile = async (profileId) => {
        try {
            setLoading(true);

            const response = await fetch(`https://127.0.0.1:7232/api/user/${profileId}/profile`, {
                credentials: 'include'
            });

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

            await loadFavorites(data.favorites);

        } catch (error) {
            console.error('Error fetching user profile:', error);
        } finally {
            setLoading(false);
        }
    };

    const loadFavorites = async (favorites) => {
        try {
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

    const checkIfFollowing = async () => {
        if (!currentUser || !user) return;

        try {
            const response = await fetch(
                `https://127.0.0.1:7232/api/user/${currentUser.userId}/is-following/${user.id}`,
                { credentials: 'include' }
            );

            if (response.ok) {
                const data = await response.json();
                setIsFollowing(data.isFollowing);
            }
        } catch (error) {
            console.error('Error checking follow status:', error);
        }
    };

    const handleFollowToggle = async () => {
        if (!isAuthenticated || !currentUser) {
            alert('Debes iniciar sesión para seguir usuarios');
            return;
        }

        if (!user) return;

        setFollowLoading(true);

        try {
            const endpoint = isFollowing
                ? `https://127.0.0.1:7232/api/user/${currentUser.userId}/unfollow/${user.id}`
                : `https://127.0.0.1:7232/api/user/${currentUser.userId}/follow/${user.id}`;

            const method = isFollowing ? 'DELETE' : 'POST';

            const response = await fetch(endpoint, {
                method: method,
                credentials: 'include'
            });

            if (response.ok) {
                setIsFollowing(!isFollowing);
                // Actualizar el contador de seguidores
                await fetchUserProfile(user.id);
            } else {
                const error = await response.json();
                alert(error.error || 'Error al procesar la solicitud');
            }
        } catch (error) {
            console.error('Error toggling follow:', error);
            alert('Error de conexión');
        } finally {
            setFollowLoading(false);
        }
    };

    const handleProfileUpdate = async () => {
        if (!canEdit) {
            alert('No tienes permiso para editar este perfil');
            return;
        }

        try {
            const response = await fetch(`https://127.0.0.1:7232/api/user/${user.id}/profile`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify(editedProfile)
            });

            if (response.ok) {
                await fetchUserProfile(user.id);
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
        if (!canEdit) {
            alert('No tienes permiso para editar este perfil');
            return;
        }

        try {
            const artistIds = favoriteArtists.map(a => a.id).join(',');
            const albumIds = favoriteAlbums.map(a => a.id).join(',');
            const songIds = favoriteSongs.map(s => s.id).join(',');

            const response = await fetch(`https://127.0.0.1:7232/api/user/${user.id}/favorites`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
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

                            <div className="profile-actions">
                                {canEdit && (
                                    <button
                                        className="btn-edit-profile"
                                        onClick={() => setIsEditing(true)}
                                    >
                                        ✎ Editar Perfil
                                    </button>
                                )}

                                {!canEdit && isAuthenticated && (
                                    <button
                                        className={`btn-follow ${isFollowing ? 'following' : ''}`}
                                        onClick={handleFollowToggle}
                                        disabled={followLoading}
                                    >
                                        {followLoading ? '...' : (isFollowing ? '✓ Siguiendo' : '+ Seguir')}
                                    </button>
                                )}
                            </div>
                        </>
                    ) : (
                        <div className="profile-edit-form">
                            <input
                                type="text"
                                value={editedProfile.username}
                                onChange={(e) => setEditedProfile({ ...editedProfile, username: e.target.value })}
                                placeholder="Nombre de usuario"
                                className="profile-input"
                            />
                            <input
                                type="email"
                                value={editedProfile.email}
                                onChange={(e) => setEditedProfile({ ...editedProfile, email: e.target.value })}
                                placeholder="Email"
                                className="profile-input"
                            />
                            <textarea
                                value={editedProfile.bio}
                                onChange={(e) => setEditedProfile({ ...editedProfile, bio: e.target.value })}
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

            <div className="favorites-section">
                <div className="favorites-header">
                    <h2>{canEdit ? 'Mis Favoritos' : 'Favoritos'}</h2>
                    {canEdit && !isEditingFavorites && (
                        <button
                            className="btn-edit-favorites"
                            onClick={() => setIsEditingFavorites(true)}
                        >
                            ✎ Editar Favoritos
                        </button>
                    )}
                    {canEdit && isEditingFavorites && (
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

                <div className="favorites-category">
                    <h3>Artistas Top 3</h3>
                    {isEditingFavorites && canEdit && (
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
                            showRemoveButton={isEditingFavorites && canEdit}
                            onRemove={(id) => removeFavorite(id, 'artist')}
                        />
                    ) : (
                        <p className="no-favorites">
                            {canEdit ? 'No has seleccionado artistas favoritos' : 'No tiene artistas favoritos'}
                        </p>
                    )}
                </div>

                <div className="favorites-category">
                    <h3>Álbumes Top 3</h3>
                    {isEditingFavorites && canEdit && (
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
                            showRemoveButton={isEditingFavorites && canEdit}
                            onRemove={(id) => removeFavorite(id, 'album')}
                        />
                    ) : (
                        <p className="no-favorites">
                            {canEdit ? 'No has seleccionado álbumes favoritos' : 'No tiene álbumes favoritos'}
                        </p>
                    )}
                </div>

                <div className="favorites-category">
                    <h3>Canciones Top 3</h3>
                    {isEditingFavorites && canEdit && (
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
                            showRemoveButton={isEditingFavorites && canEdit}
                            onRemove={(id) => removeFavorite(id, 'song')}
                        />
                    ) : (
                        <p className="no-favorites">
                            {canEdit ? 'No has seleccionado canciones favoritas' : 'No tiene canciones favoritas'}
                        </p>
                    )}
                </div>
            </div>
        </div>
    );
};

export default UserProfile;