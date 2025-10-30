#Dependencias
Para poder correr el código, de preferencia correrlo en Visual Studio, para poder usar:
- ASP.NET: para poder instalar todo se ocupa .NET SDK (la version 9.0.306 funciona)
-> Swashbuckle.AspNetCore
-> Microsoft.EntityFrameworkCore
-> Microsoft.EntityFrameworkCore.Design
-> Microsoft.EntityFrameworkCore.Relational
-> Npgsql.EntityFrameworkCore.PostgreSQL
-> Microsoft.AspNetCore.SpaProxy


- React: para poder instalar todo se ocupa Node.js (la version v23.5.0 funciona)
-> react
-> react-dom
-> react-router-dom
-> vite (y rolldown-vite)
-> @vitejs/plugin-react
-> eslint
-> @types/react y @types/react-dom

Para poder conectarse a la base de datos, es necesario en la carpeta SoundTrack.Server correr el comando: dotnet user-secrets set "ConnectionStrings:DefaultConnection" "SupabaseConnection = Host=db.ywzspxwxcqzyhttjrbrh.supabase.co;Database=postgres;Username=postgres;Password=12345SoundTrack12345!SSL Mode=Require;Trust Server Certificate=true"

NOTA IMPORTANTE: al correr el código, si no carga nada, sera necesario copiar el URL y abrirlo en otro navegador (de preferencia Chrome).
