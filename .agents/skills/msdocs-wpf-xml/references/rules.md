# Rules — msdocs-wpf-xml (projeto PI_T1)

Estas rules valem para **opencode**, **Gemini CLI** e **Antigravity** neste projeto.

## 1. Fonte de verdade

- Para WPF/XAML/XML, a fonte oficial e o Microsoft Learn via MCP `fetch-docs-microsoft`.
- Nao adivinhe assinatura de API (`RotateTransform.Angle`, `Storyboard.TargetProperty`, etc.).
  Se nao lembra, busque na URL catalogada em `references/urls.md` e cite.

## 2. Allowlist de dominios (fetch MCP)

- Permitido: `https://learn.microsoft.com/**`
- Negado: qualquer outro dominio para documentacao (blogs, StackOverflow) salvo pedido explicito do usuario.
- Nunca exponha conteudo fora do Markdown retornado pelo fetch como se fosse oficial.

## 3. Locale e fallback

- Ordem: `pt-br` -> `en-us`.
- Ao citar, informe qual locale foi lido, ex.: `[Learn pt-br] <url>` ou `[Learn en-us] <url>`.

## 4. Uso do MCP fetch

- Sempre via tool `fetch` do servidor `fetch-docs-microsoft`.
- Preservar no projeto os args `--user-agent <Chrome/Windows>` e `--ignore-robots-txt`
  (Learn bloqueia bots sem User-Agent).
- Nao usar `fetch` para sites com auth, nem para `localhost`/intranet.

## 5. Padrao de resposta com docs

1. O que a doc diz (1-3 linhas) + link.
2. Trecho XAML/C# minimo aplicavel ao trabalho (locomotiva: corpo, rodas, bielas).
3. Onde colocar no projeto (arquivo/controle sugerido).

## 6. Escopo do projeto (WPF locomotiva 2D)

- Elementos em `(0,0)` + `RenderTransform` para posicionar/animar (regra do enunciado).
- Rodas: `ControlTemplate` com raios visiveis + `RotateTransform` + `Storyboard` continuo.
- Conjunto: `Canvas` + `TranslateTransform` animada para deslocamento horizontal.
- Bielas: `Rectangle`/`Line` ligando rodas, movimento sincronizado.
