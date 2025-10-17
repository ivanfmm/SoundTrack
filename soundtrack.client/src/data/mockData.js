// Datos de ejemplo para pruebas (creados por claude)
export const mockSongs = [
    {
        id: "1",
        name: "Bohemian Rhapsody",
        imageUrl: "https://upload.wikimedia.org/wikipedia/en/9/9f/Bohemian_Rhapsody.png",
        score: 5,
        publicationDate: "1975-10-31T00:00:00Z",
        description: "Una obra maestra del rock progresivo que combina ópera, balada y hard rock en una experiencia musical única. Considerada una de las mejores canciones de todos los tiempos.",
        genres: ["Rock", "Progressive Rock", "Opera Rock"],
        tags: ["Clásico", "Icónico", "Experimental"],
        artists: [
            { id: "1", name: "Queen" }
        ],
        album: {
            id: "1",
            name: "A Night at the Opera"
        },
        reviews: [
            {
                id: 1,
                userId: 1,
                author: "RockFan87",
                description: "Una obra maestra absoluta. La complejidad musical y la fusión de estilos hacen de esta canción algo único en la historia de la música. Freddie Mercury en su máximo esplendor.",
                score: 5,
                publicationDate: "2024-01-15T10:30:00Z",
                likes: 142,
                dislikes: 3
            },
            {
                id: 2,
                userId: 2,
                author: "MusicLover2000",
                description: "Increíble cómo esta canción sigue siendo relevante después de tantos años. La transición entre las diferentes secciones es perfecta.",
                score: 5,
                publicationDate: "2024-02-20T14:15:00Z",
                likes: 89,
                dislikes: 1
            },
            {
                id: 3,
                userId: 3,
                author: "ClassicRockEnthusiast",
                description: "Buena canción, pero a veces siento que está sobrevalorada. Prefiero otras canciones de Queen como 'Somebody to Love'.",
                score: 4,
                publicationDate: "2024-03-10T09:45:00Z",
                likes: 34,
                dislikes: 12
            }
        ]
    },
    {
        id: "2",
        name: "Stairway to Heaven",
        imageUrl: "https://upload.wikimedia.org/wikipedia/en/2/26/Led_Zeppelin_-_Led_Zeppelin_IV.jpg",
        score: 5,
        publicationDate: "1971-11-08T00:00:00Z",
        description: "Un épico viaje musical que comienza con una delicada guitarra acústica y culmina en uno de los solos de guitarra más legendarios de la historia del rock.",
        genres: ["Rock", "Hard Rock", "Progressive Rock"],
        tags: ["Épico", "Legendario", "Guitar Solo"],
        artists: [
            { id: "2", name: "Led Zeppelin" }
        ],
        album: {
            id: "2",
            name: "Led Zeppelin IV"
        },
        reviews: [
            {
                id: 4,
                userId: 4,
                author: "GuitarHero",
                description: "El solo de guitarra de Jimmy Page es simplemente legendario. Esta canción define lo que es el rock clásico.",
                score: 5,
                publicationDate: "2024-01-05T16:20:00Z",
                likes: 201,
                dislikes: 5
            },
            {
                id: 5,
                userId: 5,
                author: "VinylCollector",
                description: "Una experiencia auditiva perfecta. La forma en que construyen la tensión durante 8 minutos es magistral.",
                score: 5,
                publicationDate: "2024-02-14T11:30:00Z",
                likes: 156,
                dislikes: 2
            }
        ]
    },
    {
        id: "3",
        name: "Billie Jean",
        imageUrl: "https://upload.wikimedia.org/wikipedia/en/5/55/Michael_Jackson_-_Thriller.png",
        score: 5,
        publicationDate: "1983-01-02T00:00:00Z",
        description: "El icónico hit de Michael Jackson con su inconfundible línea de bajo. Una de las canciones más influyentes del pop de todos los tiempos.",
        genres: ["Pop", "R&B", "Funk"],
        tags: ["Bailable", "80s", "Moonwalk"],
        artists: [
            { id: "3", name: "Michael Jackson" }
        ],
        album: {
            id: "3",
            name: "Thriller"
        },
        reviews: [
            {
                id: 6,
                userId: 6,
                author: "PopKing",
                description: "El rey del pop en su mejor momento. Esa línea de bajo es adictiva y el moonwalk... ¡histórico!",
                score: 5,
                publicationDate: "2024-01-20T13:10:00Z",
                likes: 178,
                dislikes: 4
            },
            {
                id: 7,
                userId: 7,
                author: "DanceFloorQueen",
                description: "Imposible no bailar cuando suena esta canción. Un clásico atemporal.",
                score: 5,
                publicationDate: "2024-03-05T19:25:00Z",
                likes: 92,
                dislikes: 1
            }
        ]
    },
    {
        id: "4",
        name: "Smells Like Teen Spirit",
        imageUrl: "https://upload.wikimedia.org/wikipedia/en/b/b7/NirvanaNevermindalbumcover.jpg",
        score: 5,
        publicationDate: "1991-09-10T00:00:00Z",
        description: "El himno del grunge que definió una generación. El icónico riff de guitarra y la voz cruda de Kurt Cobain capturan la angustia juvenil de los 90.",
        genres: ["Grunge", "Alternative Rock", "Hard Rock"],
        tags: ["90s", "Generacional", "Rebelde"],
        artists: [
            { id: "4", name: "Nirvana" }
        ],
        album: {
            id: "4",
            name: "Nevermind"
        },
        reviews: [
            {
                id: 8,
                userId: 8,
                author: "GrungeKid",
                description: "Esta canción cambió el panorama musical de los 90. Un himno generacional que sigue resonando hoy en día.",
                score: 5,
                publicationDate: "2024-02-28T10:00:00Z",
                likes: 167,
                dislikes: 8
            },
            {
                id: 9,
                userId: 9,
                author: "AlternativeRocker",
                description: "El riff es icónico, pero siento que se convirtió más en un fenómeno cultural que en una gran canción musical.",
                score: 4,
                publicationDate: "2024-03-15T15:40:00Z",
                likes: 45,
                dislikes: 23
            }
        ]
    },
    {
        id: "5",
        name: "Imagine",
        imageUrl: "https://upload.wikimedia.org/wikipedia/en/1/1d/John_Lennon_-_Imagine_John_Lennon.jpg",
        score: 5,
        publicationDate: "1971-10-11T00:00:00Z",
        description: "Un himno pacifista y utópico que invita a imaginar un mundo sin divisiones. La melodía simple pero poderosa acompaña un mensaje universal de paz.",
        genres: ["Pop", "Soft Rock", "Piano Rock"],
        tags: ["Paz", "Utopía", "Mensaje Social"],
        artists: [
            { id: "5", name: "John Lennon" }
        ],
        album: {
            id: "5",
            name: "Imagine"
        },
        reviews: [
            {
                id: 10,
                userId: 10,
                author: "PeaceLover",
                description: "Una canción que trasciende la música. El mensaje de paz y unidad es tan relevante hoy como cuando fue escrita.",
                score: 5,
                publicationDate: "2024-01-10T08:15:00Z",
                likes: 234,
                dislikes: 7
            },
            {
                id: 11,
                userId: 11,
                author: "LennonFan",
                description: "Hermosa melodía con un mensaje profundo. John Lennon en su forma más inspiradora.",
                score: 5,
                publicationDate: "2024-02-05T12:30:00Z",
                likes: 145,
                dislikes: 3
            }
        ]
    }
];

export const mockArtists = [
    {
        id: "1",
        name: "Queen",
        imageUrl: "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Queen_-_Montreal_1981.jpg/1200px-Queen_-_Montreal_1981.jpg",
        score: 5,
        description: "Banda de rock británica formada en 1970, conocida por su diversidad musical y las extraordinarias actuaciones de Freddie Mercury.",
        genres: ["Rock", "Progressive Rock", "Arena Rock"],
        tags: ["Legendario", "Icónico"]
    },
    {
        id: "2",
        name: "Led Zeppelin",
        imageUrl: "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fd/Led_Zeppelin_1977.jpg/1200px-Led_Zeppelin_1977.jpg",
        score: 5,
        description: "Pioneros del hard rock y heavy metal, reconocidos como una de las bandas más influyentes de la historia del rock.",
        genres: ["Rock", "Hard Rock", "Blues Rock"],
        tags: ["Pioneros", "Influyente"]
    }
];

export const mockAlbums = [
    {
        id: "1",
        name: "A Night at the Opera",
        imageUrl: "https://upload.wikimedia.org/wikipedia/en/4/4d/Queen_A_Night_At_The_Opera.png",
        score: 5,
        publicationDate: "1975-11-21T00:00:00Z",
        description: "El cuarto álbum de estudio de Queen, que incluye su canción más famosa 'Bohemian Rhapsody'.",
        genres: ["Rock", "Progressive Rock"],
        artists: [{ id: "1", name: "Queen" }]
    }
];