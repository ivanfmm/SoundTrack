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

# React + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) (or [oxc](https://oxc.rs) when used in [rolldown-vite](https://vite.dev/guide/rolldown)) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the ESLint configuration

If you are developing a production application, we recommend using TypeScript with type-aware lint rules enabled. Check out the [TS template](https://github.com/vitejs/vite/tree/main/packages/create-vite/template-react-ts) for information on how to integrate TypeScript and [`typescript-eslint`](https://typescript-eslint.io) in your project.
