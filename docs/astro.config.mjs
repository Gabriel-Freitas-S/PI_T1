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
        {
          label: 'Visão Geral & Arquitetura',
          items: [
            { label: 'Introdução & Especificações', slug: 'visao-geral/introducao' },
            { label: 'Arquitetura MVVM & Desacoplamento', slug: 'visao-geral/arquitetura-mvvm' },
          ],
        },
        {
          label: 'Engenharia Gráfica & XAML',
          items: [
            { label: 'Modelagem Vetorial 2D', slug: 'engenharia-grafica/modelagem-vetorial-xaml' },
            { label: 'ControlTemplate & Rodas Desacopladas', slug: 'engenharia-grafica/control-template-rodas' },
          ],
        },
        {
          label: 'Física & Cinemática Analítica',
          items: [
            { label: 'Equações Analíticas Biela-Manivela', slug: 'fisica-cinematica/equacoes-analiticas' },
            { label: 'Simulador Cinemático Interativo', slug: 'fisica-cinematica/simulador-interativo' },
          ],
        },
        {
          label: 'Animação & Partículas',
          items: [
            { label: 'Storyboards & Rotação Contínua', slug: 'animacao-particulas/storyboards-e-rotacao' },
            { label: 'Emissor de Partículas de Vapor', slug: 'animacao-particulas/emissor-de-particulas' },
          ],
        },
        {
          label: 'Qualidade de Código & SonarQube',
          items: [
            { label: 'Métricas SonarQube & Auditoria', slug: 'qualidade-software/sonarqube-e-regras' },
            { label: 'Clean Code & Padrões C#', slug: 'qualidade-software/padroes-csharp-clean-code' },
          ],
        },
        {
          label: 'Referências Oficiais Microsoft Learn',
          items: [
            { label: 'Catálogo de APIs & Conceitos', slug: 'referencias/catalogo-microsoft-learn' },
            { label: 'Diretrizes & Especificações do Trabalho', slug: 'referencias/especificacoes-trabalho' },
          ],
        },
      ],
    }),
  ],
});
