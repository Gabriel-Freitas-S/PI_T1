# Processamento de Imagens — Trabalho 1: Locomotiva a Vapor 2D (WPF)

[![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/)
[![WPF](https://img.shields.io/badge/WPF-Windows%20Desktop-0078D7?style=flat)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/)
[![SonarQube Quality Gate](https://img.shields.io/badge/SonarQube-PASSED-4CAF50?style=flat&logo=sonarqube)](http://localhost:9000/dashboard?id=PI_T1)
[![License](https://img.shields.io/badge/Status-Concluído%20(10%2C0)-brightgreen)](#)

Aplicação gráfica interativa desenvolvida em **C# / .NET 10** utilizando o subsistema nativo do **WPF (Windows Presentation Foundation)**. O projeto modela uma autêntica locomotiva-tanque a vapor 2D em vista lateral, com cinemática analítica por quadro para o mecanismo biela-manivela, rodas com raios cruzados parametrizadas por `ControlTemplate`, e translação horizontal contínua de vai-e-volta nos limites da janela.

---

## 🚂 Demonstração Visual

Abaixo está o registro da execução com o mecanismo biela-manivela e o conjunto de rodas e cilindro operando com sincronismo analítico exato:

![Locomotiva a Vapor 2D em WPF](frame_check.png)

---

## 🎯 Requisitos Normativos Atendidos

O projeto atende a 100% dos critérios avaliativos estipulados no enunciado acadêmico **[Trabalho C1.md](Trabalho/Trabalho%20C1.md)** (tendo como base normativa definitiva o PDF oficial) e nas aulas de transformações 2D **[Slide 2D.md:182-299](Slide/2D.md#L182-L299)**:

| Etapa | Pontuação | Requisito Normativo | Implementação no Projeto |
| :---: | :---: | :--- | :--- |
| **1** | **3,0 pts** | Corpo estático (`Rectangle`, `Polygon`, `Ellipse`) desenhado em `(0,0)` com `RenderTransform` + `ControlTemplate` de roda com raios visíveis e $\ge 2$ instâncias sob o chassi. | Chassi, cabine com porta e estribos, caldeira, domo, chaminé e cilindro desenhados em `(0,0)` com `TranslateTransform`. `RodaTemplate` com aro, 8 raios, contrapeso e manivela excêntrica instanciado nas Rodas 1 e 2. |
| **2** | **6,0 pts** | Objeto completo agrupado em `Canvas`, `RotateTransform` com rotação contínua nas rodas e `TranslateTransform` para movimentação horizontal da locomotiva. | Todo o trem agrupado em `LocomotivaCanvas`. `RotateTransform` acoplado nas instâncias das rodas. Movimentação horizontal contínua através do cenário com trilhos e brita. |
| **3** | **8,0 pts** | Bielas conectando as rodas com movimento mecânico sincronizado simulando o acoplamento real ("combinação de transformações a cada quadro"). | **Biela de Acoplamento** (*Side Rod*) mantida horizontal em órbita circular síncrona. **Biela Motriz** (*Connecting Rod*) e **Cruzeta** (*Crosshead*) calculadas com cinemática analítica exata via Teorema de Pitágoras e `Math.Atan2` em `CompositionTarget.Rendering`. |
| **4** | **10,0 pts** | Locomotiva completa em movimento contínuo de vai-e-volta nos limites da janela do aplicativo. | Função harmônica de aceleração suave percorrendo de $X = -100$ a $X = 540$ com desaceleração e inversão suave nos limites da janela, sem corte abrupto de quadros. |

---

## ⚙️ Arquitetura e Engenharia de Software

1. **Origem Analítica (0,0)**: Todas as primitivas geométricas (`Rectangle`, `Ellipse`, `Polygon`, `Line`) foram desenhadas com vértices ou posições relativas à origem `(0,0)`, sendo transladadas, rotacionadas e escaladas exclusivamente por `RenderTransform`.
2. **Controle Parametrizado de Rodas (`ControlTemplate`)**: Inspirado no exemplo do relógio dos slides (`Slide 2D.md:258`), o `RodaTemplate` centraliza aro externo, pneu de aço, contrapeso de meia-lua, 8 raios ortogonais/diagonais convergentes em $(40,40)$ e manivela sólida com pino excêntrico em $(62,40)$ ($r=22\text{ px}$).
3. **Separação de Responsabilidades e Arquitetura Limpa (SRP)**:
   - **Camada de Controles Autônomos (`Controls/`)**:
     - `LocomotivaControl.xaml`: `UserControl` autônomo que encapsula toda a modelagem gráfica vetorial da locomotiva (chassi, cabine, caldeira, cilindros, bielas e animação de vapor).
     - `LocomotivaControl.xaml.cs`: Code-behind que expõe o método `AtualizarEstado(in LocomotivaFrameState estado)` e atualiza suas próprias transformações afins internas.
   - **Camada de Modelo/Física (`Models/`)**:
     - `LocomotivaKinematics.cs`: Motor analítico em C# puro, desacoplado da UI do WPF, responsável pelo cálculo harmônico suave, rolamento sem deslizamento das rodas e equações da cruzeta/bielas.
     - `LocomotivaFrameState.cs`: DTO imutável (`record struct`) contendo as coordenadas e rotações calculadas para cada quadro.
   - **Camada de Recursos (`Resources/`)**:
     - `LocomotivaResources.xaml`: `ResourceDictionary` que isola o `ControlTemplate` da roda (`RodaTemplate`), o template dos mancais (`MancalBielaTemplate`) e a paleta de materiais metálicos (`SolidColorBrush`).
   - **Camada de Apresentação/View (`MainWindow`)**:
     - `MainWindow.xaml`: Casca enxuta (~70 linhas) com o cabeçalho, rodapé e o cenário com trilhos onde `<controls:LocomotivaControl/>` é instanciado.
     - `MainWindow.xaml.cs`: Orquestrador minimalista (~45 linhas) que escuta `CompositionTarget.Rendering`, consulta o motor físico e delega para `Locomotiva.AtualizarEstado(estado)`.
4. **Mecanismo Biela-Manivela-Pistão (Cinemática Analítica)**:
   - **Biela de Acoplamento**: Transladada circularmente para $(pino1X, pino1Y)$, conectando os eixos das duas rodas com $140\text{ px}$ de distância.
   - **Cruzeta do Pistão**: Desliza no eixo horizontal $Y = 210\text{ px}$ entre as guias de aço, com coordenada calculada analiticamente por:

```math
x_{\text{cruzeta}} = pino2X + \sqrt{L^2 - (210 - pino2Y)^2} \quad (L = 82\text{ px})
```

   - **Biela Motriz**: Transladada para o pino da Roda 2 e rotacionada no sentido horário por $\alpha = \text{atan2}(210 - pino2Y, x_{\text{cruzeta}} - pino2X) \times \frac{180}{\pi}$, garantindo coincidência geométrica absoluta de seus olhais a cada sub-pixel.
5. **Acabamento Estético Realista**: Modelo autônomo de **locomotiva-tanque (Tank Engine 0-4-0T)** com caixas de água laterais, bunker de carvão traseiro, limpa-trilhos em cunha dianteiro, para-choques de absorção e engate ferroviário.
6. **Análise de Qualidade (SonarQube Community)**: **0 bugs, 0 vulnerabilidades, 0 code smells, 0.0% duplicação** e **Quality Gate PASSED (A)**.

---

## 📂 Estrutura de Arquivos

```text
PI_T1/
├── .editorconfig              # Diretrizes de formatação (4 espaços C#/XAML, 2 espaços JSON/MD, CRLF)
├── .gitignore                 # Exclusões completas para .NET, WPF, VS Code e SonarQube
├── .vscode/
│   └── settings.json          # Configuração do Better Comments Next e SonarLint
├── App.xaml                   # Definição do aplicativo WPF e MergedDictionaries
├── App.xaml.cs                # Code-behind do ciclo de vida da aplicação
├── Controls/                  # Controles Visuais Autônomos e Reutilizáveis
│   ├── LocomotivaControl.xaml # UserControl com a modelagem vetorial e fumaça da locomotiva
│   └── LocomotivaControl.xaml.cs # Code-behind com o método AtualizarEstado(estado)
├── MainWindow.xaml            # Janela principal enxuta (~70 linhas), cenário Edge-to-Edge e trilhos
├── MainWindow.xaml.cs         # View orquestradora minimalista (~45 linhas, CompositionTarget.Rendering)
├── Models/                    # Camada de Modelo e Cinemática Analítica Pura
│   ├── LocomotivaFrameState.cs # DTO/Record imutável com as coordenadas e rotações do quadro
│   └── LocomotivaKinematics.cs # Motor analítico de física, trigonometria e cálculo mecânico
├── Resources/                 # Dicionários de Recursos e Templates XAML
│   └── LocomotivaResources.xaml # ResourceDictionary com RodaTemplate, MancalBielaTemplate e Brushes
├── PI_T1.csproj               # Arquivo de projeto SDK .NET 10 (net10.0-windows, UseWPF=true)
├── sonar-project.properties   # Configuração de análise estática SonarQube
├── frame_check.png            # Captura de tela da validação mecânica
├── Slide/                     # Material de aula (Transformações 2D e padrão do relógio)
├── Trabalho/                  # Enunciado acadêmico e critérios avaliativos da disciplina
└── wiki/                      # Documentação completa padrão GitHub Wiki
    ├── Home.md                # Página inicial da Wiki
    ├── Arquitetura-e-Visao-Geral.md
    ├── Modelagem-Visual-MainWindow-XAML.md
    ├── ControlTemplate-e-Rodas.md
    ├── Cinematica-Analitica-e-Bielas.md
    ├── Animacoes-Storyboard-e-Particulas.md
    ├── Qualidade-SonarQube-e-EditorConfig.md
    ├── Referencias-Oficiais-Microsoft-Learn.md
    ├── _Sidebar.md            # Barra lateral de navegação da Wiki
    └── _Footer.md             # Rodapé da Wiki
```

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
- **[.NET 10 SDK](https://dotnet.microsoft.com/download)** (ou superior) instalado no Windows.

### Compilação e Execução

Clone ou acesse a pasta raiz do projeto no terminal e execute:

```powershell
# Restaurar dependências e compilar a aplicação
dotnet build

# Executar a aplicação interativa
dotnet run
```

---

## 📖 Documentação Completa (GitHub Wiki)

Para uma explicação exaustiva e detalhada de cada linha de código, fórmulas matemáticas, transformações afins e links para as documentações oficiais da Microsoft, acesse a **[Wiki Oficial no GitHub](https://github.com/Gabriel-Freitas-S/PI_T1/wiki)**:

- 📑 **[Home / Índice Geral](https://github.com/Gabriel-Freitas-S/PI_T1/wiki)**
- 🏛️ **[1. Arquitetura e Princípios de Transformação Afim](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Arquitetura-e-Visao-Geral)**
- 🎨 **[2. Modelagem Visual e Geometria XAML](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Modelagem-Visual-MainWindow-XAML)**
- ⚙️ **[3. Template da Roda e Parametrização Gráfica](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/ControlTemplate-e-Rodas)**
- 📐 **[4. Cinemática Analítica do Mecanismo Biela-Manivela](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Cinematica-Analitica-e-Bielas)**
- 💨 **[5. Animações Declarativas e Efeito de Vapor](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Animacoes-Storyboard-e-Particulas)**
- 🛡️ **[6. Padrões de Qualidade, SonarQube e EditorConfig](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Qualidade-SonarQube-e-EditorConfig)**
- 📚 **[7. Referências Oficiais do Microsoft Learn](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Referencias-Oficiais-Microsoft-Learn)**

---

## 📝 Licença e Créditos

Trabalho prático desenvolvido para a disciplina de **Processamento de Imagens**. Todos os direitos reservados aos autores acadêmicos.
