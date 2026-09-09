# Padrões de Qualidade, SonarQube e EditorConfig

[![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=Gabriel-Freitas-S_PI_T1&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Gabriel-Freitas-S_PI_T1)
[![SonarQube Cloud](https://sonarcloud.io/images/project_badges/sonarcloud-light.svg)](https://sonarcloud.io/summary/new_code?id=Gabriel-Freitas-S_PI_T1)
![Bugs](https://img.shields.io/badge/Bugs-0%20(Rating%20A)-success?style=flat-square)
![Vulnerabilidades](https://img.shields.io/badge/Vulnerabilidades-0%20(Rating%20A)-success?style=flat-square)
![Code Smells](https://img.shields.io/badge/Code%20Smells-0%20(Rating%20A)-success?style=flat-square)
![Padronização](https://img.shields.io/badge/Formata%C3%A7%C3%A3o-.editorconfig-blue?style=flat-square)

Neste capítulo, abordam-se os processos de governança de código, auditoria estática de qualidade, padronização tipográfica e convenções de documentação semântica aplicados no projeto.

---

## 1. Auditoria de Qualidade Estática com SonarQube

O projeto foi submetido à análise estática contínua utilizando a distribuição **SonarQube Community Edition** com o **SonarScanner for .NET**, inspecionando código C#, marcação XAML e arquivos de configuração estrutural.

### 1.1 Configuração do Scanner (`sonar-project.properties`)

O arquivo de configuração do scanner na raiz do repositório delimita o escopo da inspeção:

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

# Exclusões de Diretórios Temporários, Build e Metadados
sonar.exclusions=bin/**,obj/**,.gemini/**,.agents/**,.vscode/**,Slide/**,Trabalho/**,wiki/**,temp_slide_img/**
```

---

### 1.2 Resultados da Auditoria de Qualidade (Quality Gate)

A auditoria executada retornou **aprovação total (PASSED)** em todas as dimensões de confiabilidade, segurança e manutenibilidade:

| Dimensão de Qualidade | Métrica Obtida | Meta do Quality Gate | Classificação |
| :--- | :---: | :---: | :---: |
| **Bugs Concorrentes ou Lógicos** | **0** | **0** | **Rating A** |
| **Vulnerabilidades de Segurança** | **0** | **0** | **Rating A** |
| **Hotspots de Segurança Revisados** | **0** | **0** | **Rating A** |
| **Code Smells (Dívida Técnica)** | **0** | **0** | **Rating A** |
| **Duplicação de Código (DRY)** | **0.0%** | **< 3.0%** | **Rating A** |
| **Tempo de Remediação Estimado** | **0 min** | **0 min** | **Rating A** |

### 1.3 Fatores Determinantes para o Rating A

1. **Padrão MVVM Puro e Desacoplado**: A total separação entre lógica física (`LocomotivaKinematics.cs`), modelo de estado (`LocomotivaFrameState.cs`), camada intermediária reativa (`LocomotivaViewModel.cs`) e controles de visão (`LocomotivaControl.xaml`) eliminou complexidade ciclomática e acoplamento desnecessário.
2. **Otimização Zero-Allocation**: O uso de `readonly record struct` na pilha e de instâncias estáticas cacheadas de `PropertyChangedEventArgs` impede vazamentos de memória e pressão sobre o coletor de lixo (*Garbage Collector*).
3. **Templates Reutilizáveis (`ControlTemplate`)**: O isolamento da geometria das rodas e mancais em `LocomotivaResources.xaml` garantiu 0.0% de duplicação em trechos vetoriais repetitivos.

---

## 2. Padronização Tipográfica com `.editorconfig`

Para assegurar conformidade tipográfica e formatação uniforme entre múltiplos ambientes integrados de desenvolvimento (Visual Studio, VS Code, Rider e compiladores em linha de comando), foi fixado o arquivo [`.editorconfig`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/.editorconfig) na raiz do projeto:

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

### Regras Estabelecidas

- **C# e XAML**: Indentação estrita com 4 espaços por nível hierárquico, promovendo alinhamento legível em nós aninhados de marcação vetorial e blocos estruturados de código.
- **Markdown e JSON**: Recuo de 2 espaços para garantir concisão em esquemas e documentações técnicas.
- **Quebras de Linha e Codificação**: Padronização em `CRLF` (padrão Windows) e codificação `UTF-8` com terminação obrigatória de linha única no encerramento de arquivos.

---

## 3. Padrão Semântico: Better Comments Next

Todos os comentários nos arquivos de código C# e marcação XAML seguem as diretrizes da convenção semântica **Better Comments Next**, registrada no arquivo de configuração do espaço de trabalho [`.vscode/settings.json`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/.vscode/settings.json).

### Taxonomia Semântica de Comentários

| Marcador | Função Semântica | Cor de Destaque | Aplicação no Projeto |
| :---: | :--- | :---: | :--- |
| `#` | **Divisória / Seção Estrutural** | Azul / Cinza | `// # ETAPA 5: CRUZETA E MECANISMO DO PISTÃO` |
| `*` | **Destaque / Informação Técnica** | Verde Claro | `// * Conforme Trabalho C1.md:48 — Distância 140px` |
| `!` | **Invariante Crítico / Restrição** | Laranja / Vermelho | `<!-- ! INVARIANTE: Todos os elementos partem de (0,0) -->` |
| `?` | **Equacionamento / Cinemática** | Roxo / Ciano | `// ? xCruzeta = pino2X + sqrt(L^2 - deltaY^2)` |
| `todo` | **Ação Futura / Pendência** | Amarelo | `// todo: Integração com áudio sintético opcional` |

Essa taxonomia transforma a base de código em uma documentação viva e didática, permitindo que pesquisadores, alunos e revisores identifiquem instantaneamente onde a teoria matemática e as restrições normativas foram convertidas em código executável.

---

## Referências Oficiais da Microsoft

- [Microsoft Learn — Padrões e Convenções de Codificação em C#](https://learn.microsoft.com/pt-br/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Microsoft Learn — EditorConfig no .NET e Visual Studio](https://learn.microsoft.com/pt-br/visualstudio/ide/create-portable-custom-editor-options)
- [Microsoft Learn — Gerenciamento do Ciclo de Vida da Aplicação WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/app-development/application-management-overview)
