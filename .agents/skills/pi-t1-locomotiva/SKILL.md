---
name: pi-t1-locomotiva
description: Implementa a locomotiva 2D em WPF seguindo o enunciado do trabalho e o exemplo do relogio dos slides. Use quando for criar, revisar ou animar corpo, rodas, bielas, Canvas, ControlTemplate, RotateTransform, TranslateTransform e Storyboard no PI_T1.
---

# PI_T1 Locomotiva 2D

Skill de projeto (opencode + Gemini CLI + Antigravity via `.agents/skills/`).

## Fontes (obrigatorias, nesta ordem)

1. **Trabalho (o que deve ser feito)** — normativo, nunca contradizer:
   - `Trabalho/Trabalho C1.md` (leitura rapida, 56 linhas)
   - `Trabalho/PDF/Trabalho C1.pdf` (original autoritativo; usar quando o MD estiver truncado ou com encoding duvidoso)
2. **Slides (conteudo da aula, padrao a seguir)** — como fazer:
   - `Slide/2D.md` (transcricao, 299 linhas: pipeline, transformacoes, exemplo do relogio)
   - `Slide/PDF/2D.pdf` (original autoritativo para diagramas/codigo com formatacao)
3. **Docs oficiais** — detalhe de API via skill `msdocs-wpf-xml` + MCP `fetch-docs-microsoft`.

Detalhes em `references/fontes.md`. Checklist do trabalho em `references/regras-trabalho.md`.
Mapeamento relogio -> locomotiva em `references/padroes.md`.

## Como usar (obrigatorio)

1. Leia `Trabalho/Trabalho C1.md` inteiro antes de codar ou revisar.
2. Leia as secoes indicadas de `Slide/2D.md` (linhas `182:299` = Intro WPF + relogio).
3. So consulte o PDF original se: MD com `?`/mojibake, figura/codigo ilegivel, ou duvida de requisito.
4. Para duvida de API (`Storyboard.TargetProperty`, `RotateTransform.CenterX` etc.), ative a skill
   `msdocs-wpf-xml` e busque no Learn; cite a URL.
5. Ao responder, cite `arquivo:linha` das fontes locais, ex.: `Trabalho C1.md:32`, `2D.md:235`.

## Mapeamento rapido relogio -> locomotiva

- `Canvas` como area grafica + coordenadas relativas: `2D.md:230`.
- Tudo desenhado em `(0,0)` + `RenderTransform` (escala/translacao): `2D.md:237`, `Trabalho C1.md:26`.
- Ponteiros via `Polygon` + template em `resources`: `2D.md:255` -> rodas via `ControlTemplate` com raios.
- Animacao `DoubleAnimation` do angulo dentro de `Storyboard` + `triggers`: `2D.md:288` -> `RotateTransform` das rodas + `TranslateTransform` do `Canvas`.
