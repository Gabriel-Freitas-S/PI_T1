# PI_T1 — GEMINI.md (Gemini CLI / Antigravity)

## Skill de docs

- Ative a skill `msdocs-wpf-xml` (`.agents/skills/msdocs-wpf-xml/SKILL.md`) para WPF/XAML/XML e C#/.NET.
- MCP local: `fetch-docs-microsoft` (ver `.gemini/settings.json` e `.agents/mcp_config.json`).
- URLs: `.agents/skills/msdocs-wpf-xml/references/urls.md`
- Rules: `.agents/skills/msdocs-wpf-xml/references/rules.md`

## Skill do trabalho + slides

- Ative a skill `pi-t1-locomotiva` (`.agents/skills/pi-t1-locomotiva/SKILL.md`) para implementar/revisar.
- Requisitos: `Trabalho/Trabalho C1.md` + PDF; padrao relogio: `Slide/2D.md` (`182:299`) + PDF.

## Rules

1. Consulte o Learn via MCP antes de afirmar sintaxe WPF/XAML ou C#/.NET; cite a URL (pt-br, fallback en-us).
2. Allowlist: apenas `https://learn.microsoft.com/**`.
3. Padrao do trabalho: `(0,0)` + `RenderTransform`; rodas `ControlTemplate` + `RotateTransform` + `Storyboard`; `Canvas` + `TranslateTransform`; bielas `Rectangle`/`Line`.

<!-- CODEGRAPH_START -->
## CodeGraph

In repositories indexed by CodeGraph (a `.codegraph/` directory exists at the repo root), reach for it BEFORE grep/find or reading files when you need to understand or locate code:

- **MCP tool** (when available): `codegraph_explore` answers most code questions in one call — the relevant symbols' verbatim source plus the call paths between them, including dynamic-dispatch hops grep can't follow. Name a file or symbol in the query to read its current line-numbered source. If it's listed but deferred, load it by name via tool search.
- **Shell** (always works): `codegraph explore "<symbol names or question>"` prints the same output.

If there is no `.codegraph/` directory, skip CodeGraph entirely — indexing is the user's decision.
<!-- CODEGRAPH_END -->
