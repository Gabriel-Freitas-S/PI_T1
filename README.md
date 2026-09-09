# Processamento de Imagens — Trabalho 1: Locomotiva a Vapor 2D (WPF)

![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-Windows%20Desktop-0078D7?style=flat-square)
[![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=Gabriel-Freitas-S_PI_T1&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Gabriel-Freitas-S_PI_T1)
[![SonarQube Cloud](https://sonarcloud.io/images/project_badges/sonarcloud-light.svg)](https://sonarcloud.io/summary/new_code?id=Gabriel-Freitas-S_PI_T1)
![Arquitetura](https://img.shields.io/badge/Arquitetura-MVVM%20Zero--Alloc-brightgreen?style=flat-square)
![Trajetória](https://img.shields.io/badge/Movimento-Ping--Pong%20Bidirecional-blue?style=flat-square)
![Status](https://img.shields.io/badge/Nota%20Alvo-10.0%20(Etapas%201--4)-success?style=flat-square)

Aplicação gráfica interativa desenvolvida em **C# / .NET 10** utilizando o subsistema nativo do **WPF (Windows Presentation Foundation)**. O projeto modela uma locomotiva-tanque a vapor 2D em vista lateral, com cinemática analítica por quadro para o mecanismo biela-manivela, rodas com raios cruzados parametrizadas por `ControlTemplate`, e translação horizontal contínua de vai-e-volta nos limites da janela.

---

## 1. Demonstração Visual

Abaixo está o registro animado em tempo de execução com o mecanismo biela-manivela, cilindro com oclusão mecânica da haste do pistão e travessia contínua de vai-e-volta (Ping-Pong) nos limites da janela operando com sincronismo analítico exato:

![Locomotiva a Vapor 2D em WPF (Vai-e-Volta Ping-Pong)](locomotiva.gif)

---

## 2. Requisitos Normativos Atendidos

O projeto atende a 100% dos critérios avaliativos estipulados no enunciado acadêmico **[Trabalho C1.md](Trabalho/Trabalho%20C1.md)** (tendo como base normativa definitiva o PDF oficial) e nas aulas de transformações 2D **[Slide 2D.md:182-299](Slide/2D.md#L182-L299)**:

| Etapa | Pontuação | Requisito Normativo | Implementação no Projeto |
| :---: | :---: | :--- | :--- |
| **1** | **3,0 pts** | Corpo estático (`Rectangle`, `Polygon`, `Ellipse`) desenhado em `(0,0)` com `RenderTransform` + `ControlTemplate` de roda com raios visíveis e $\ge 2$ instâncias sob o chassi. | Chassi, cabine com porta e estribos, caldeira, domo, chaminé e cilindro desenhados em `(0,0)` com `TranslateTransform`. `RodaTemplate` com aro, 8 raios, contrapeso e manivela excêntrica instanciado nas Rodas 1 e 2. |
| **2** | **6,0 pts** | Objeto completo agrupado em `Canvas`, `RotateTransform` com rotação contínua nas rodas e `TranslateTransform` para movimentação horizontal da locomotiva. | Todo o trem agrupado em `LocomotivaCanvas`. `RotateTransform` acoplado nas instâncias das rodas. Movimentação horizontal contínua através do cenário com trilhos e brita. |
| **3** | **8,0 pts** | Bielas conectando as rodas com movimento mecânico sincronizado simulando o acoplamento real ("combinação de transformações a cada quadro"). | **Biela de Acoplamento** (*Side Rod*) mantida horizontal em órbita circular síncrona. **Biela Motriz** (*Connecting Rod*) e **Cruzeta** (*Crosshead*) calculadas com cinemática analítica exata via Teorema de Pitágoras e `Math.Atan2` em `CompositionTarget.Rendering`. |
| **4** | **10,0 pts** | Locomotiva completa em movimento contínuo nos limites da janela do aplicativo. | **Movimento Vai-e-Volta (Ping-Pong)**: A locomotiva percorre o cenário em perfil ferroviário trapezoidal com suavização (aceleração gradual, cruzeiro constante, frenagem realista até repouso e pausa de 1s para manobra), virando 180° com `ScaleTransform` (`ScaleX = -1 / 1`) e retornando infinitamente, com a composição 100% visível na tela e margem segura de 20px dos limites da janela. |

---

## 3. Arquitetura e Engenharia de Software

1. **Origem Analítica (0,0)**: Todas as primitivas geométricas (`Rectangle`, `Ellipse`, `Polygon`, `Line`) foram desenhadas com vértices ou posições relativas à origem `(0,0)`, sendo transladadas, rotacionadas e escaladas exclusivamente por `RenderTransform`.
2. **Controle Parametrizado de Rodas (`ControlTemplate`)**: Inspirado no exemplo do relógio dos slides (`Slide 2D.md:258`), o `RodaTemplate` centraliza aro externo, pneu de aço, contrapeso de meia-lua, 8 raios ortogonais/diagonais convergentes.
3. **Padrão Arquitetural MVVM de Alta Performance (Zero-Alloc)**:
   - **Camada Model (`Models/`)**: `LocomotivaKinematics.cs` (motor analítico puro sem UI) e `LocomotivaFrameState.cs` (DTO imutável alocado na Stack).
   - **Camada ViewModel (`ViewModels/`)**: `ViewModelBase.cs` (infraestrutura com cache estático), `LocomotivaViewModel.cs` (Data Binding das 9 transformações afins) e `MainViewModel.cs` (orquestrador de estado e status descritivo).
   - **Camada View (`MainWindow` e `Controls/`)**: `MainWindow.xaml` (View acoplada a `CompositionTarget.Rendering`) e `LocomotivaControl.xaml` (View vetorial desacoplada com Data Binding).
   - **Camada de Recursos (`Resources/`)**: `LocomotivaResources.xaml` (`ResourceDictionary` contendo `RodaTemplate`, `MancalBielaTemplate` e paleta metálica).
4. **Mecanismo Biela-Manivela-Pistão (Cinemática Analítica)**:
   - **Biela de Acoplamento**: Transladada circularmente para $(pino1X, pino1Y)$, conectando os eixos das duas rodas com $140\text{ px}$ de distância.
   - **Cruzeta do Pistão**: Desliza no eixo horizontal $Y = 210\text{ px}$ entre as guias de aço, com coordenada analítica $x_{\text{cruzeta}} = pino2X + \sqrt{L^2 - (210 - pino2Y)^2}$ ($L = 82\text{ px}$).
   - **Biela Motriz**: Transladada para o pino da Roda 2 e rotacionada no sentido horário por $\alpha = \text{atan2}(210 - pino2Y, x_{\text{cruzeta}} - pino2X) \times \frac{180}{\pi}$.
5. **Acabamento Estético Realista**: Modelo autônomo de **locomotiva-tanque (Tank Engine 0-4-0T)** com caixas de água laterais, bunker de carvão traseiro, limpa-trilhos em cunha dianteiro, para-choques de absorção e engate ferroviário.
6. **Análise de Qualidade (SonarQube Community)**: **0 bugs, 0 vulnerabilidades, 0 code smells, 0.0% duplicação** e **Quality Gate PASSED (A)**.

---

## 4. Estrutura de Arquivos

```text
PI_T1/
├── .editorconfig              # Padrões tipográficos e convenções de formatação C#/XAML
├── .vscode/                   # Configurações do VS Code
│   └── settings.json          # Configuração do Better Comments Next e SonarLint
├── App.xaml                   # Definição do aplicativo WPF e MergedDictionaries
├── App.xaml.cs                # Code-behind do ciclo de vida e comandos CLI (--record-frames, --screenshot)
├── Controls/                  # Controles Visuais Autônomos (Views)
│   ├── LocomotivaControl.xaml # UserControl vetorial com Data Binding e fumaça da locomotiva
│   └── LocomotivaControl.xaml.cs # Code-behind da View com suporte a LocomotivaViewModel
├── MainWindow.xaml            # Janela principal: cabeçalho, cenário e DataContext="{Binding Locomotiva}"
├── MainWindow.xaml.cs         # View principal orquestradora acoplada ao V-Sync (CompositionTarget.Rendering)
├── Models/                    # Camada Model (M): Cinemática Analítica Pura (sem dependências de UI)
│   ├── LocomotivaFrameState.cs # DTO/Record imutável com as coordenadas e rotações do quadro
│   └── LocomotivaKinematics.cs # Motor analítico de física, rolamento monotônico e circuito contínuo
├── ViewModels/                # Camada ViewModel (VM): Padrão MVVM com INotifyPropertyChanged
│   ├── ViewModelBase.cs       # Classe base com implementação de INotifyPropertyChanged e SetProperty
│   ├── LocomotivaViewModel.cs # Propriedades observáveis das 9 transformações afins da locomotiva
│   └── MainViewModel.cs       # ViewModel raiz orquestrador de estado global, títulos e simulação
├── Resources/                 # Dicionários de Recursos e Templates XAML
│   └── LocomotivaResources.xaml # ResourceDictionary com RodaTemplate, MancalBielaTemplate e Brushes
├── PI_T1.csproj               # Arquivo de projeto SDK .NET 10 (net10.0-windows, UseWPF=true)
├── sonar-project.properties   # Configuração de análise estática SonarQube
├── generate_gif.ps1           # Script PowerShell para geração automática do GIF animado com FFmpeg
├── locomotiva.gif             # Demonstração animada da locomotiva em circuito contínuo
├── frame_check.png            # Captura estática de validação mecânica
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
    ├── _Footer.md             # Rodapé da Wiki
    └── locomotiva.gif         # GIF animado para exibição na Wiki
```

---

## 5. Como Executar o Projeto

### Pré-requisitos

- **[.NET 10 SDK](https://dotnet.microsoft.com/download)** (ou superior) instalado no Windows.
- **[FFmpeg](https://ffmpeg.org/)** (opcional, para geração automatizada do GIF animado).

### Compilação e Execução

Clone ou acesse a pasta raiz do projeto no terminal e execute:

```powershell
# 1. Restaurar dependências e compilar a aplicação
dotnet build

# 2. Executar a aplicação interativa
dotnet run

# 3. Gerar automaticamente o GIF animado da locomotiva (requer FFmpeg)
.\generate_gif.ps1
```

---

## 6. Documentação Completa (GitHub Wiki)

Para uma explicação exaustiva e detalhada de cada linha de código, fórmulas matemáticas, transformações afins e links para as documentações oficiais da Microsoft, acesse a **[Wiki Oficial no GitHub](https://github.com/Gabriel-Freitas-S/PI_T1/wiki)**:

- **[Home / Índice Geral](https://github.com/Gabriel-Freitas-S/PI_T1/wiki)**
- **[1. Arquitetura e Princípios de Transformação Afim](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Arquitetura-e-Visao-Geral)**
- **[2. Modelagem Visual e Geometria XAML](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Modelagem-Visual-MainWindow-XAML)**
- **[3. Template da Roda e Parametrização Gráfica](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/ControlTemplate-e-Rodas)**
- **[4. Cinemática Analítica do Mecanismo Biela-Manivela](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Cinematica-Analitica-e-Bielas)**
- **[5. Animações Declarativas e Efeito de Vapor](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Animacoes-Storyboard-e-Particulas)**
- **[6. Padrões de Qualidade, SonarQube e EditorConfig](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Qualidade-SonarQube-e-EditorConfig)**
- **[7. Catálogo de Referências Oficiais do Microsoft Learn](https://github.com/Gabriel-Freitas-S/PI_T1/wiki/Referencias-Oficiais-Microsoft-Learn)**

---

## 7. Licença e Créditos

Trabalho prático desenvolvido para a disciplina de **Processamento de Imagens**. Todos os direitos reservados aos autores acadêmicos.
