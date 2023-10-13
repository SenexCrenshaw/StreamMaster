/** @type {import('next').NextConfig} */
import { createRequire } from 'module';
import { dirname } from 'path';
import { fileURLToPath } from 'url';

const require = createRequire(import.meta.url);
const path = require('path');
const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const nextConfig = {
  distDir: 'build',
  output: 'export',
  reactStrictMode: true,
  webpack: (config, { dev }) => {
    // Enable source maps in development
    config.resolve.alias['@'] = path.resolve(__dirname, './');
    if (dev) {
      config.devtool = 'eval-source-map';
    }

    return config;
  },
  images: { unoptimized: true },
};

export default nextConfig;
