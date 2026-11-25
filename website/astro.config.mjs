// @ts-check
import { defineConfig } from 'astro/config';

// https://astro.build/config
export default defineConfig({
  site: 'https://zerodayz7.github.io',
  base: '/CyberCalc.NET9/',
  trailingSlash: 'always',
  build: {
    assets: '_astro'
  }
});
