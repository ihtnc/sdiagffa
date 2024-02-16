import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    alias: {
      "@tailwind.config": new URL('./tailwind.config', import.meta.url).pathname,
      "@/": new URL('./src/app/', import.meta.url).pathname
    },
    setupFiles: './setupTest.js'
  }
});
