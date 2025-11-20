import { useState, useRef, useEffect } from 'react';
import './AudioPlayer.css';

const AudioPlayer = ({ previewUrl, trackName }) => {
    const [isPlaying, setIsPlaying] = useState(false);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(30); // Previews son siempre 30 segundos
    const audioRef = useRef(null);

    useEffect(() => {
        const audio = audioRef.current;
        if (!audio) return;

        const updateTime = () => setCurrentTime(audio.currentTime);
        const updateDuration = () => setDuration(audio.duration);
        const handleEnded = () => setIsPlaying(false);

        audio.addEventListener('timeupdate', updateTime);
        audio.addEventListener('loadedmetadata', updateDuration);
        audio.addEventListener('ended', handleEnded);

        return () => {
            audio.removeEventListener('timeupdate', updateTime);
            audio.removeEventListener('loadedmetadata', updateDuration);
            audio.removeEventListener('ended', handleEnded);
        };
    }, []);

    const togglePlay = () => {
        const audio = audioRef.current;
        if (!audio) return;

        if (isPlaying) {
            audio.pause();
        } else {
            audio.play();
        }
        setIsPlaying(!isPlaying);
    };

    const handleSeek = (e) => {
        const audio = audioRef.current;
        if (!audio) return;

        const rect = e.currentTarget.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const percentage = x / rect.width;
        audio.currentTime = percentage * duration;
    };

    const formatTime = (seconds) => {
        const mins = Math.floor(seconds / 60);
        const secs = Math.floor(seconds % 60);
        return `${mins}:${secs.toString().padStart(2, '0')}`;
    };

    if (!previewUrl) {
        return (
            <div className="audio-player-unavailable">
                <p>🔒 Preview no disponible para esta canción</p>
                <p className="text-sm">Escúchala completa en Spotify</p>
            </div>
        );
    }

    return (
        <div className="audio-player">
            <audio ref={audioRef} src={previewUrl} />

            <div className="audio-player-controls">
                <button
                    onClick={togglePlay}
                    className="play-button"
                    aria-label={isPlaying ? 'Pausar' : 'Reproducir'}
                >
                    {isPlaying ? (
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                            <path d="M6 4h4v16H6V4zm8 0h4v16h-4V4z" />
                        </svg>
                    ) : (
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                            <path d="M8 5v14l11-7z" />
                        </svg>
                    )}
                </button>

                <div className="audio-player-info">
                    <div className="track-name">{trackName}</div>
                    <div className="preview-badge">Preview 30s</div>
                </div>
            </div>

            <div className="audio-player-progress">
                <span className="time-display">{formatTime(currentTime)}</span>

                <div
                    className="progress-bar"
                    onClick={handleSeek}
                >
                    <div
                        className="progress-fill"
                        style={{ width: `${(currentTime / duration) * 100}%` }}
                    />
                </div>

                <span className="time-display">{formatTime(duration)}</span>
            </div>
        </div>
    );
};

export default AudioPlayer;