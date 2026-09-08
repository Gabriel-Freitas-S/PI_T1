# PI_T1 — Processamento de Imagens (WPF locomotiva 2D)

## Skill de docs (obrigatoria para WPF/XML)

- Skill: `msdocs-wpf-xml` em `.agents/skills/msdocs-wpf-xml/SKILL.md`.
- MCP: `fetch-docs-microsoft` (fetch do Learn). Configs locais:
  - opencode: `opencode.json`
  - Gemini CLI: `.gemini/settings.json`
  - Antigravity: `.agents/mcp_config.json`
- URLs catalogadas: `.agents/skills/msdocs-wpf-xml/references/urls.md`
- Rules completas: `.agents/skills/msdocs-wpf-xml/references/rules.md`

## Skill do trabalho + slides (obrigatoria para implementar/revisar)

- Skill: `pi-t1-locomotiva` em `.agents/skills/pi-t1-locomotiva/SKILL.md`.
- Fontes: `Trabalho/Trabalho C1.md` + `Trabalho/PDF/Trabalho C1.pdf` (requisitos, PDF prevalece);
  `Slide/2D.md` + `Slide/PDF/2D.pdf` (padrao do relogio, linhas `182:299`).
- Detalhes: `references/fontes.md`, `references/regras-trabalho.md`, `references/padroes.md`.

## Rules

1. Para qualquer API WPF/XAML/XML, consulte o Learn via `fetch-docs-microsoft` antes de responder; cite a URL (pt-br preferencial, fallback en-us).
2. Allowlist docs: apenas `https://learn.microsoft.com/**`.
3. Elementos graficos em `(0,0)` + `RenderTransform`; rodas via `ControlTemplate` + `RotateTransform` + `Storyboard`; conjunto em `Canvas` + `TranslateTransform`; bielas com `Rectangle`/`Line`.
