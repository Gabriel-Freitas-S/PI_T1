# 📚 Referências Oficiais do Microsoft Learn

Todas as tecnologias de renderização, vetores, transformações matriciais e temporização por quadro utilizadas neste projeto foram implementadas com base direta na documentação técnica oficial da **Microsoft (Microsoft Learn)**.

Abaixo está o catálogo estruturado de referências oficiais em português (`pt-br`):

---

## 1. Núcleo do WPF e Ciclo de Vida da Aplicação

| Tópico / Classe | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral do WPF** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/) | Fundamentos do subsistema gráfico e separação XAML / C#. |
| **Introdução ao WPF** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/getting-started/) | Estrutura de janelas, compilação de projeto e ciclo de inicialização. |
| **Visão Geral do XAML** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/xaml/) | Sintaxe declarativa de marcação vetorial da interface. |
| **Sintaxe XAML em Detalhes** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/advanced/xaml-syntax-in-detail/) | Sintaxe de propriedades anexadas, sintaxe de ponto e coleções. |
| **Classe Application** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.application) | Ponto de entrada global da aplicação em `App.xaml` e `App.xaml.cs`. |
| **Classe Window** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.window) | Janela principal de visualização da locomotiva `MainWindow`. |
| **Classe UserControl** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.controls.usercontrol) | Encapsulamento modular da locomotiva 2D em `LocomotivaControl`. |

---

## 2. Layout, Contêineres e Primitivas Gráficas Vetoriais

| Tópico / Classe | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral de Layout** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/layout/) | Pipeline de layout de duas etapas (`Measure` e `Arrange`). |
| **Elemento Canvas** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/canvas/) | Contêiner de posicionamento absoluto e hierarquia de peças móveis. |
| **Formas e Desenho Básico** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/) | Primitivas `Rectangle`, `Ellipse`, `Polygon` e `Line` na origem $(0,0)$. |
| **Classe Rectangle** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle/) | Vigas do chassi, corpo do cilindro, caldeira e hastes retangulares. |
| **Classe Ellipse** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse/) | Aros das rodas, mancais/olhais das bielas e partículas de fumaça. |
| **Classe Polygon** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon/) | Limpa-trilhos (*cowcatcher*), teto da cabine e feixe do farol. |
| **Classe Line** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line/) | Raios convergentes das rodas, guias da cruzeta e corrimãos. |
| **Pincéis e Gradientes** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/wpf-brushes-overview/) | Gradientes `LinearGradientBrush` do céu e cores das ligas metálicas. |

---

## 3. Álgebra Linear e Transformações Afins 2D

| Tópico / Classe | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral de Transformações** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/) | Teoria das transformações afins, matrizes homogêneas e eixos locais. |
| **RenderTransform vs LayoutTransform** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/#differences-between-the-rendertransform-and-layouttransform-properties) | Justificativa técnica para uso de `RenderTransform` a 60 FPS. |
| **Classe TranslateTransform** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.translatetransform/) | Translação da locomotiva, das bielas, da cruzeta e peças em $(0,0)$. |
| **Classe RotateTransform** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/) | Rotação síncrona das rodas em $(40,40)$ e rotação da biela motriz. |
| **Classe ScaleTransform** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform/) | Expansão volumétrica das baforadas de vapor. |
| **Classe TransformGroup** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.transformgroup/) | Composição combinada de rotação e translação no mesmo elemento. |

---

## 4. Templates, Recursos e Estilos XAML

| Tópico / Classe | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral de Estilos e Modelos** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/styles-templates-overview/) | Separação entre estrutura visual e controle. |
| **Modelos de Controle (ControlTemplate)** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/) | Implementação do `RodaTemplate` baseado no relógio dos slides. |
| **Classe ResourceDictionary** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.resourcedictionary) | Dicionário de recursos `LocomotivaResources.xaml` para isolamento de templates. |
| **Propriedade MergedDictionaries** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.resourcedictionary.mergeddictionaries) | Composição modular de dicionários de recursos em `App.xaml`. |
| **Recursos XAML** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/systems/xaml-resources/) | Definição e consumo de recursos estáticos via `StaticResource`. |

---

## 5. Renderização em Tempo Real e Cinemática

| Tópico / Classe | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Renderização por Intervalo de Quadro** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/how-to-render-on-a-per-frame-interval-using-compositiontarget) | Padrão arquitetural para simulação física em tempo real no WPF. |
| **Evento CompositionTarget.Rendering** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering) | Gancho de renderização antes da composição final da GPU. |
| **Classe Stopwatch** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch) | Medição de tempo contínuo de alta precisão sem acúmulo de erro. |
| **Classe Math (Atan2, Cos, Sin, Sqrt)** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.math) | Resolução trigonométrica e analítica do mecanismo biela-manivela. |

---

## 6. Animação Declarativa e Storyboards

| Tópico / Classe | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral de Animação** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/animation-overview/) | Linha do tempo, interpolação temporal e taxas de atualização. |
| **Visão Geral de Storyboards** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/) | Agrupamento de animações em `Canvas.Triggers`. |
| **Classe DoubleAnimation** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.animation.doubleanimation/) | Animação suave de coordenadas, escala e opacidade da fumaça. |
| **Classe EventTrigger** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.eventtrigger/) | Disparo da animação no evento de carregamento da interface. |

---

## 7. Qualidade, Convenções de Código e EditorConfig

| Tópico / Ferramenta | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Convenções de Código C#** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/csharp/fundamentals/coding-style/coding-conventions) | Padronização de nomenclatura, métodos e estruturação de classes. |
| **EditorConfig no Visual Studio / .NET** | [Acessar Documentação](https://learn.microsoft.com/pt-br/visualstudio/ide/create-portable-custom-editor-options) | Configuração portátil de tabulações, quebras de linha e charset. |
