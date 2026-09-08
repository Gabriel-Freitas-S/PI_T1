# PI_T1 — GEMINI.md (Gemini CLI / Antigravity)

## Skill de docs

- Ative a skill `msdocs-wpf-xml` (`.agents/skills/msdocs-wpf-xml/SKILL.md`) para WPF/XAML/XML.
- MCP local: `fetch-docs-microsoft` (ver `.gemini/settings.json` e `.agents/mcp_config.json`).
- URLs: `.agents/skills/msdocs-wpf-xml/references/urls.md`
- Rules: `.agents/skills/msdocs-wpf-xml/references/rules.md`

## Skill do trabalho + slides

- Ative a skill `pi-t1-locomotiva` (`.agents/skills/pi-t1-locomotiva/SKILL.md`) para implementar/revisar.
- Requisitos: `Trabalho/Trabalho C1.md` + PDF; padrao relogio: `Slide/2D.md` (`182:299`) + PDF.

## Rules

1. Consulte o Learn via MCP antes de afirmar sintaxe WPF/XAML; cite a URL (pt-br, fallback en-us).
2. Allowlist: apenas `https://learn.microsoft.com/**`.
3. Padrao do trabalho: `(0,0)` + `RenderTransform`; rodas `ControlTemplate` + `RotateTransform` + `Storyboard`; `Canvas` + `TranslateTransform`; bielas `Rectangle`/`Line`.
