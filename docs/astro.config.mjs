// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import { unified } from '@astrojs/markdown-remark';
import remarkMath from 'remark-math';
import rehypeKatex from 'rehype-katex';

export default defineConfig({
  site: 'https://gabriel-freitas-s.github.io',
  base: '/PI_T1',
  markdown: {
    processor: unified({
      remarkPlugins: [remarkMath],
      rehypePlugins: [rehypeKatex],
    }),
  },
  integrations: [
    starlight({
      title: 'PI_T1 — Locomotiva 2D WPF',
      description: 'Documentação técnica de engenharia gráfica, cinemática analítica e WPF/XAML da Locomotiva 2D.',
      favicon: '/favicon.svg',
      customCss: [
        'katex/dist/katex.min.css',
        '/src/styles/custom.css',
      ],
      social: [
        {
          icon: 'github',
          label: 'GitHub',
          href: 'https://github.com/Gabriel-Freitas-S/PI_T1',
        },
      ],
      sidebar: [
        ['Visão Geral & Arquitetura', 'visao-geral'],
        ['Engenharia Gráfica & XAML', 'engenharia-grafica'],
        ['Física & Cinemática Analítica', 'fisica-cinematica'],
        ['Animação & Partículas', 'animacao-particulas'],
        ['Qualidade de Código & SonarQube', 'qualidade-software'],
        ['Referências Oficiais Microsoft Learn', 'referencias'],
      ].map(([label, directory]) => ({
        label,
        items: [{ autogenerate: { directory } }],
      })),
    }),
  ],
});
