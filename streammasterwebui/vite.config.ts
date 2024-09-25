import react from '@vitejs/plugin-react';
import { visualizer } from 'rollup-plugin-visualizer';
import path from 'node:path';
import viteCompression from 'vite-plugin-compression';

import { builtinModules } from 'module';
import { defineConfig } from 'vite';

export default defineConfig({
  appType: 'spa',

  build: {
    emptyOutDir: true,
    rollupOptions: {
      external: builtinModules,
      output: {
        manualChunks: (id): any => {
          console.log(id);
          if (id.includes('node_modules')) {
            return id.toString().split('node_modules/')[1].split('/')[0].toString();
          }

          if (id.includes('/streammasterwebui/lib/smAPI/')) {
            return 'smAPI';
          }
          if (id.includes('/streammasterwebui/lib/')) {
            return 'smLib';
          }
          if (id.includes('/streammasterwebui/components//')) {
            return 'smComponents';
          }
          if (id.includes('/streammasterwebui/features//')) {
            return 'smFeatures';
          }
          return undefined;
        }
      }
    }
  },
  clearScreen: true,
  plugins: [react(), visualizer(), viteCompression()],
  resolve: {
    alias: {
      '@': path.resolve('./'),
      '@components': path.resolve('./components/'),
      '@features': path.resolve('./features/'),
      '@lib': path.resolve('./lib/')
    }
  }
});
