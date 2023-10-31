import react from '@vitejs/plugin-react';
import path from 'node:path';

import { defineConfig } from 'vite';

export default defineConfig({
  clearScreen: true,
  appType: 'spa',
  build: {
    emptyOutDir: true,
    outDir: '../StreamMasterAPI/bin/Debug/net7.0/wwwroot/'
    //   rollupOptions: {
    //     output: {
    //       manualChunks: (id): 'vendor_aws' | 'vendor_mui' | 'vendor' | 'smAPI' | 'smLib' | 'smComponents/' | 'smFeatures/' | undefined => {
    //         console.log(id);
    //         if (id.includes('node_modules')) {
    //           if (id.includes('@aws-amplify')) {
    //             return 'vendor_aws';
    //           }
    //           if (id.includes('@mui')) {
    //             return 'vendor_mui';
    //           }

    //           return 'vendor'; // all other package goes here
    //         }
    //         if (id.includes('/streammasterwebui2/lib/smAPI/')) {
    //           return 'smAPI';
    //         }
    //         if (id.includes('/streammasterwebui/lib/')) {
    //           return 'smLib';
    //         }
    //         if (id.includes('/streammasterwebui/components//')) {
    //           return 'smComponents/';
    //         }
    //         if (id.includes('/streammasterwebui/features//')) {
    //           return 'smFeatures/';
    //         }
    //         return undefined;
    //       }
    //     }
    //   }
  },
  plugins: [react()],

  resolve: {
    alias: {
      '@': path.resolve('./'),
      '@components': path.resolve('./components/'),
      '@features': path.resolve('./features/'),
      '@lib': path.resolve('./lib/')
    }
  }
});
