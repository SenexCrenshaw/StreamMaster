/** @type {import('next').NextConfig} */
const nextConfig = {
  distDir: 'build',
  output: 'export',
  reactStrictMode: true,
  webpack: (config, { dev }) => {
    // Enable source maps in development
    if (dev) {
      config.devtool = 'eval-source-map'
    }

    return config
  },
  images: { unoptimized: true },
}

export default nextConfig
