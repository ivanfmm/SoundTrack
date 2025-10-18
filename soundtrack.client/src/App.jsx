import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './Components/Layout';
import SongProfile from "./Components/SongProfile";
import './App.css';
import Home from './Components/Home';
import ArtistProfile from './Components/ArtistProfile';
import AlbumProfile from './Components/AlbumProfile';
import Searched from './Components/Searched';

function App() {

    return (
        <Router>
            <Routes>
                <Route path="/" element={<Layout />}>
                    <Route index element={<Home />}
                    
                    /*
                    Vista de bienvenida de Chris 
                    
                    ={
                        <div style={{ 
                            textAlign: 'center', 
                            padding: '4rem 2rem',
                            maxWidth: '800px',
                            margin: '0 auto'
                        }}>
                            <h1 style={{ 
                                color: 'var(--color-neon-green)', 
                                fontSize: '3rem',
                                marginBottom: '1rem'
                            }}>
                                Bienvenido a SoundTrack
                            </h1>
                            <p style={{ 
                                color: 'rgba(255, 255, 255, 0.8)', 
                                fontSize: '1.2rem',
                                lineHeight: '1.6'
                            }}>
                                Tu plataforma de música estilo Letterboxd. 
                                Descubre, califica y comparte tu pasión por la música.
                            </p>
                        </div>
                    } */
                    
                    />
                    
                    <Route path="/song/:id" element={<SongProfile />} />
                    {/* en trabajop 🚧🚧 no pasar (aqui van a ir las demas vistas probablemente copy paste) */}
                    <Route path="/artist/:id" element={<ArtistProfile/>}/>
                    <Route path="/album/:id" element={<AlbumProfile/>}/>
                    <Route path="/search/:query" element={<Searched />} />
                </Route>
                
            </Routes>
        </Router>
    );
    
    //async function populateWeatherData() {

    //}
}

export default App;