import { defineConfig, presetUno, presetIcons } from 'unocss';

export default defineConfig({
  content: {
    filesystem: ['src/**/*.{astro,html,js,jsx,md,mdx,ts,tsx}'],
  },
  presets: [
    presetUno(),
    presetIcons({
      scale: 1.2,
      extraProperties: {
        'display': 'inline-block',
        'vertical-align': 'middle',
      },
    }),
  ],
  theme: {
    colors: {
      brand: {
        dark: '#090d16',
        card: '#0f172a',
        border: '#1e293b',
        borderLight: '#334155',
        cyan: '#00d2ff',
        cyanHover: '#38bdf8',
        amber: '#f59e0b',
        steel: '#94a3b8',
      },
    },
  },
  shortcuts: {
    'btn-action': 'px-4 py-2 rounded font-medium transition duration-200 cursor-pointer flex items-center justify-center gap-2',
    'card-tech': 'p-4 rounded-lg bg-[#0f172a] border border-[#1e293b] shadow-lg shadow-black/40',
  },
});
