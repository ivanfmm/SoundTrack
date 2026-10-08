import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

// Backend local para desarrollo. En Vercel no se usa: ahi /api lo enruta vercel.json
const target = 'https://127.0.0.1:7232';

// Crea (si no existe) el certificado HTTPS de desarrollo con dotnet dev-certs.
// Solo se llama con "npm run dev": en Vercel no hay dotnet y el build tronaria.
function getDevCertificate() {
    const baseFolder =
        env.APPDATA !== undefined && env.APPDATA !== ''
            ? `${env.APPDATA}/ASP.NET/https`
            : `${env.HOME}/.aspnet/https`;

    const certificateName = "soundtrack.client";
    const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
    const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

    if (!fs.existsSync(baseFolder)) {
        fs.mkdirSync(baseFolder, { recursive: true });
    }

    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        if (0 !== child_process.spawnSync('dotnet', [
            'dev-certs',
            'https',
            '--export-path',
            certFilePath,
            '--format',
            'Pem',
            '--no-password',
        ], { stdio: 'inherit', }).status) {
            throw new Error("Could not create certificate.");
        }
    }

    return {
        key: fs.readFileSync(keyFilePath),
        cert: fs.readFileSync(certFilePath),
    };
}

// https://vitejs.dev/config/
export default defineConfig(({ command }) => ({
    plugins: [plugin()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    // command es 'serve' en npm run dev y 'build' en npm run build (Vercel)
    server: command === 'serve' ? {
        proxy: {
            '^/weatherforecast': {
                target,
                secure: false,
                changeOrigin: true
            },
            '/api': {
                target,
                secure: false,
                changeOrigin: true
            }
        },
        host: '127.0.0.1',
        port: 49825,
        https: getDevCertificate()
    } : undefined
}));
