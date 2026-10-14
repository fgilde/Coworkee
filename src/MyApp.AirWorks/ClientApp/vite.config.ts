import { cpSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import { defineConfig } from 'vite';

const path = (relative: string) => fileURLToPath(new URL(relative, import.meta.url));

export default defineConfig({
  // the AirWorks icons and wallpapers, served under /icons/
  publicDir: path('node_modules/@fgilde/airworks/package'),
  build: { outDir: path('../wwwroot'), emptyOutDir: true, target: 'es2022' },
  // the MudEx web components for file previews and the frame page that isolates them, served under /mudex/ (public CDNs fail on parts of the .NET runtime files)
  plugins: [{
    name: 'mudex',
    closeBundle: () => {
      cpSync(path('node_modules/mudex-webcomponents'), path('../wwwroot/mudex'), { recursive: true });
      cpSync(path('node_modules/@coworkee/airworks/dist/mudex-frame.html'), path('../wwwroot/mudex/coworkee-frame.html'));
    },
  }],
});
