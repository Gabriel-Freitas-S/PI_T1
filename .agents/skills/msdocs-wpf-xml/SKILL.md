---
name: msdocs-wpf-xml
description: Consulta documentacao oficial Microsoft Learn de WPF/XAML e XML no .NET via MCP fetch-docs-microsoft. Use quando precisar de RotateTransform, TranslateTransform, Storyboard, ControlTemplate, Canvas, data binding, estilos, ou System.Xml, XmlReader, XmlWriter, LINQ to XML.
---

# MSDocs WPF/XML

Skill de projeto para buscar conteudo oficial da Microsoft via MCP `fetch-docs-microsoft`
(`uvx mcp-server-fetch`). Vale para **opencode**, **Gemini CLI** e **Antigravity**,
pois fica em `.agents/skills/` — caminho lido pelos tres.

## Quando usar

- Duvida de API WPF: `Rectangle`, `Ellipse`, `Polygon`, `Canvas`, `ControlTemplate`,
  `RenderTransform`, `RotateTransform`, `TranslateTransform`, `Storyboard`, `DoubleAnimation`.
- Duvida de XAML: sintaxe, namespaces, `StaticResource`/`DynamicResource`, templates, triggers.
- Duvida de XML no .NET: `System.Xml`, `XmlReader`/`XmlWriter`, `XmlDocument`, `XPath`,
  `XDocument` / LINQ to XML, `XmlSerializer`.
- Sempre que for responder sobre WPF/XML neste projeto, prefira o Learn ao conhecimento do modelo.

## Como usar (obrigatorio)

1. Use a tool do MCP `fetch-docs-microsoft` (tool `fetch`), nunca `curl`/`Invoke-WebRequest` para docs.
2. Comece pelas URLs catalogadas em `references/urls.md`.
3. Prefira `pt-br`; se 404 ou conteudo raso, repita com `en-us` (troque `/pt-br/` por `/en-us/`).
4. O fetch retorna Markdown limpo. Cite a URL usada na resposta.
5. Se der 403, o User-Agent de navegador ja esta configurado nos MCPs do projeto — nao remova
   `--user-agent` nem `--ignore-robots-txt`. Apenas tente de novo ou use o espelho `en-us`.

## Exemplos de prompt

- `Use o fetch-docs-microsoft para ler https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/xaml/ e me mostrar como declarar uma RotateTransform animada`
- `Busque no Learn como animar TranslateTransform de um Canvas com Storyboard para a locomotiva`
- `Leia a doc de XDocument (LINQ to XML) e mostre como ler/escrever XML no .NET`

## Rules resumidas

Ver `references/rules.md` para as rules completas (allowlist de dominios, fallback pt-br/en-us,
proibicao de adivinhar API, padrao de citacao).
