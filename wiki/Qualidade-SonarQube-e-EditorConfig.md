# 🛡️ Padrões de Qualidade, SonarQube e EditorConfig

Neste capítulo, abordam-se os processos de auditoria de qualidade estática de código, padronização de formatação e convenções de documentação semântica aplicados no projeto.

---

## 1. Auditoria de Qualidade Estática com SonarQube

O projeto foi submetido à análise estática contínua utilizando a versão mais recente do **SonarQube Community Edition** com o **SonarScanner for .NET**, avaliando código C#, XAML e arquivos de configuração.

### 1.1 Configuração (`sonar-project.properties`)
O arquivo de configuração do scanner na raiz do projeto estabelece o escopo de análise:

```properties
# Identificação do Projeto
sonar.projectKey=PI_T1
sonar.projectName=PI_T1 - Locomotiva a Vapor 2D (WPF)
sonar.projectVersion=1.0.0

# Codificação dos Arquivos-Fonte
sonar.sourceEncoding=UTF-8

# Escopo de Análise
sonar.sources=.
sonar.inclusions=**/*.cs,**/*.xaml

# Exclusões de Diretórios de Build e Metadados
sonar.exclusions=bin/**,obj/**,.gemini/**,.agents/**,.vscode/**,Slide/**,Trabalho/**,wiki/**,temp_slide_img/**
```

### 1.2 Resultados da Análise de Qualidade (Quality Gate)
A auditoria executada retornou **apvação total (PASSED)** em todas as métricas:

| Dimensão de Qualidade | Resultado Obtido | Classificação |
| :--- | :---: | :---: |
| **Bugs** | **0** | **Rating A** |
| **Vulnerabilidades** | **0** | **Rating A** |
| **Hotspots de Segurança** | **0** | **Rating A** |
| **Code Smells** | **0** | **Rating A** |
| **Duplicação de Código** | **0.0%** | **Rating A** |
| **Cobertura de Débito Técnico** | **0 min** | **Rating A** |

---

## 2. Padronização com `.editorconfig`

Para assegurar consistência tipográfica em diferentes ambientes de desenvolvimento (Visual Studio, VS Code, Rider, CLI), foi estabelecido o arquivo [`.editorconfig`](../.editorconfig) na raiz do projeto:

```ini
root = true

[*]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
trim_trailing_whitespace = true

[*.{cs,xaml}]
indent_style = space
indent_size = 4

[*.{json,md,yml,yaml}]
indent_style = space
indent_size = 2
```

### Regras Adotadas:
- **C# e XAML**: 4 espaços por nível de indentação, garantindo alinhamento claro das tags aninhadas e blocos condicionais.
- **Markdown e JSON**: 2 espaços de recuo para manter os documentos concisos e estruturados.
- **Quebras de Linha**: Padronizadas em `CRLF` (Windows padrão) com terminação obrigatória de linha única no final do arquivo.

---

## 3. Padrão Semântico: Better Comments Next

Todos os comentários nos arquivos C# ([`MainWindow.xaml.cs`](../MainWindow.xaml.cs)) e XAML ([`MainWindow.xaml`](../MainWindow.xaml)) foram padronizados de acordo com a extensão **Better Comments Next**, configurada no arquivo [`.vscode/settings.json`](../.vscode/settings.json).

### Paleta Semântica e Convenção de Prefíxos:

| Prefixo | Significado Semântico | Cor Aplicada | Exemplo de Aplicação no Código |
| :---: | :--- | :---: | :--- |
| `#` | **Divisória / Seção Estrutural** | Azul / Cinza | `// # ETAPA 5: BIELA DE ACOPLAMENTO HORIZONTAL` |
| `*` | **Destaque / Informação Técnica** | Verde Claro | `// * Conforme Trabalho C1.md:48 — Distância 140px` |
| `!` | **Alerta Crítico / Invariante** | Laranja / Vermelho | `<!-- ! INVARIANTE: Todos os elementos partem de (0,0) -->` |
| `?` | **Fórmula Matemática / Cinemática** | Roxo / Ciano | `// ? xCruzeta = pino2X + sqrt(L^2 - deltaY^2)` |
| `todo` | **Ação Futura / Pendência** | Amarelo | `// todo: Adicionar som de apito de vapor opcional` |

Essa convenção transforma o código-fonte em um material didático autoexplicativo, permitindo que qualquer leitor localize rapidamente onde a matemática teórica é transposta para linhas de código executável.

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Padrões e Convenções de Codificação C#](https://learn.microsoft.com/pt-br/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Microsoft Learn — EditorConfig no .NET](https://learn.microsoft.com/pt-br/visualstudio/ide/create-portable-custom-editor-options)
- [Microsoft Learn — Gerenciamento do Ciclo de Vida da Aplicação WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/app-development/application-management-overview)
