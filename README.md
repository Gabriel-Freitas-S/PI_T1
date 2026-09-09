# Processamento de Imagens — Trabalho 1: Locomotiva a Vapor 2D (WPF)

[![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/)
[![WPF](https://img.shields.io/badge/WPF-Windows%20Desktop-0078D7?style=flat)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/)
[![SonarQube Quality Gate](https://img.shields.io/badge/SonarQube-PASSED-4CAF50?style=flat&logo=sonarqube)](http://localhost:9000/dashboard?id=PI_T1)
[![License](https://img.shields.io/badge/Status-Concluído%20(10%2C0)-brightgreen)](#)

Aplicação gráfica interativa desenvolvida em **C# / .NET 10** utilizando o subsistema nativo do **WPF (Windows Presentation Foundation)**. O projeto modela uma autêntica locomotiva-tanque a vapor 2D em vista lateral, com cinemática analítica por quadro para o mecanismo biela-manivela, rodas com raios cruzados parametrizadas por `ControlTemplate`, e translação horizontal contínua de vai-e-volta nos limites da janela.

---

## 🚂 Demonstração Visual

Abaixo está o registro animado em tempo de execução com o mecanismo biela-manivela, cilindro com oclusão mecânica da haste do pistão e travessia contínua em loop operando com sincronismo analítico exato:

![Locomotiva a Vapor 2D em WPF (Loop Contínuo)](locomotiva.gif)

---

## 🎯 Requisitos Normativos Atendidos

O projeto atende a 100% dos critérios avaliativos estipulados no enunciado acadêmico **[Trabalho C1.md](Trabalho/Trabalho%20C1.md)** (tendo como base normativa definitiva o PDF oficial) e nas aulas de transformações 2D **[Slide 2D.md:182-299](Slide/2D.md#L182-L299)**:

| Etapa | Pontuação | Requisito Normativo | Implementação no Projeto |
| :---: | :---: | :--- | :--- |
| **1** | **3,0 pts** | Corpo estático (`Rectangle`, `Polygon`, `Ellipse`) desenhado em `(0,0)` com `RenderTransform` + `ControlTemplate` de roda com raios visíveis e $\ge 2$ instâncias sob o chassi. | Chassi, cabine com porta e estribos, caldeira, domo, chaminé e cilindro desenhados em `(0,0)` com `TranslateTransform`. `RodaTemplate` com aro, 8 raios, contrapeso e manivela excêntrica instanciado nas Rodas 1 e 2. |
| **2** | **6,0 pts** | Objeto completo agrupado em `Canvas`, `RotateTransform` com rotação contínua nas rodas e `TranslateTransform` para movimentação horizontal da locomotiva. | Todo o trem agrupado em `LocomotivaCanvas`. `RotateTransform` acoplado nas instâncias das rodas. Movimentação horizontal contínua através do cenário com trilhos e brita. |
| **3** | **8,0 pts** | Bielas conectando as rodas com movimento mecânico sincronizado simulando o acoplamento real ("combinação de transformações a cada quadro"). | **Biela de Acoplamento** (*Side Rod*) mantida horizontal em órbita circular síncrona. **Biela Motriz** (*Connecting Rod*) e **Cruzeta** (*Crosshead*) calculadas com cinemática analítica exata via Teorema de Pitágoras e `Math.Atan2` em `CompositionTarget.Rendering`. |
| **4** | **10,0 pts** | Locomotiva completa em movimento contínuo nos limites da janela do aplicativo. | **Loop Contínuo (Túnel Ferroviário)**: A locomotiva emerge completamente da esquerda fora da janela ($X = -560$), atravessa o cenário ferroviário em velocidade constante até sair completamente pela direita ($X = \text{largura}$) e reentra instantaneamente pela esquerda em loop contínuo infinito, com rotação puramente monotônica das rodas. |

---

## ⚙️ Arquitetura e Engenharia de Software

1. **Origem Analítica (0,0)**: Todas as primitivas geométricas (`Rectangle`, `Ellipse`, `Polygon`, `Line`) foram desenhadas com vértices ou posições relativas à origem `(0,0)`, sendo transladadas, rotacionadas e escaladas exclusivamente por `RenderTransform`.
2. **Controle Parametrizado de Rodas (`ControlTemplate`)**: Inspirado no exemplo do relógio dos slides (`Slide 2D.md:258`), o `RodaTemplate` centraliza aro externo, pneu de aço, contrapeso de meia-lua, 8 raios ortogonais/diagonais convergentes.
3. **Padrão Arquitetural MVVM (Model-View-ViewModel)**:
   - **Camada Model (`Models/`)**:
     - `LocomotivaKinematics.cs`: Motor analítico em C# puro, desacoplado da UI do WPF, responsável pelo cálculo cinemático do loop contínuo, rolamento monotônico sem deslizamento das rodas e equações analíticas da cruzeta/bielas.
     - `LocomotivaFrameState.cs`: DTO imutável (`record struct`) contendo as coordenadas e rotações calculadas para cada quadro.
   - **Camada ViewModel (`ViewModels/`)**:
     - `ViewModelBase.cs`: Classe base com implementação de `INotifyPropertyChanged` e método `SetProperty`.
     - `LocomotivaViewModel.cs`: ViewModel observável com as propriedades das 9 transformações afins da locomotiva vinculadas via Data Binding declarativo.
     - `MainViewModel.cs`: ViewModel raiz que gerencia o estado da simulação, os textos da interface e orquestra a comunicação com o motor cinemático.
   - **Camada View (`MainWindow` e `Controls/`)**:
     - `MainWindow.xaml` / `MainWindow.xaml.cs`: View principal desacoplada que associa seu DataContext ao `MainViewModel` e acopla o ciclo de simulação ao V-Sync da GPU (`CompositionTarget.Rendering`).
     - `LocomotivaControl.xaml` / `LocomotivaControl.xaml.cs`: Controle de apresentação vetorial conectado ao `LocomotivaViewModel` via Data Binding, com método de compatibilidade direta para renderização offline.
   - **Camada de Recursos (`Resources/`)**:
     - `LocomotivaResources.xaml`: `ResourceDictionary` que isola o `ControlTemplate` da roda (`RodaTemplate`), o template dos mancais (`MancalBielaTemplate`) e a paleta de materiais metálicos (`SolidColorBrush`).
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

## 🚀 Como Executar o Projeto

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
