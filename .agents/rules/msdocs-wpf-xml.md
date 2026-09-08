---
description: Rules da skill msdocs-wpf-xml — consultar Learn via fetch-docs-microsoft (allowlist learn.microsoft.com, pt-br com fallback en-us). Aplica-se a WPF/XAML/XML no PI_T1.
---

# Rules — msdocs-wpf-xml

Ver skill: `../skills/msdocs-wpf-xml/SKILL.md`.
Ver rules completas: `../skills/msdocs-wpf-xml/references/rules.md`.
Ver URLs: `../skills/msdocs-wpf-xml/references/urls.md`.

1. Para qualquer API WPF/XAML/XML, consulte o Learn via `fetch-docs-microsoft` antes de responder; cite a URL.
2. Allowlist: apenas `https://learn.microsoft.com/**`. Locale: `pt-br` preferencial, fallback `en-us`.
3. Nao remover `--user-agent` / `--ignore-robots-txt` das configs MCP do projeto.
4. Padrao PI_T1: `(0,0)` + `RenderTransform`; rodas `ControlTemplate` + `RotateTransform` + `Storyboard`; `Canvas` + `TranslateTransform`; bielas `Rectangle`/`Line`.
