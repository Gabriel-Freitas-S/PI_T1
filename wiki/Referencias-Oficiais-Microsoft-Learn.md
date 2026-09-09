# Catálogo de Referências Oficiais do Microsoft Learn

![Documentação](https://img.shields.io/badge/Cat%C3%A1logo-Microsoft%20Learn-0078D4?style=flat-square)
![Idioma](https://img.shields.io/badge/Idioma-Portugu%C3%AAs%20(pt--br)-2ecc71?style=flat-square)
![Escopo](https://img.shields.io/badge/Tecnologias-WPF%20%7C%20XAML%20%7C%20C%23-blue?style=flat-square)
![Padrão](https://img.shields.io/badge/Arquitetura-MVVM%20Zero--Alloc-brightgreen?style=flat-square)

Todas as tecnologias de renderização vetorial, transformações lineares homogêneas, temporização por intervalo de quadro, arquitetura MVVM e estilização modular empregadas neste projeto foram concebidas com base na documentação técnica oficial da **Microsoft (Microsoft Learn)**.

Abaixo apresenta-se o catálogo estruturado de referências oficiais em língua portuguesa (`pt-br`):

---

## 1. Núcleo do WPF e Ciclo de Vida da Aplicação

| Módulo / API | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral do WPF** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/) | Fundamentos do subsistema gráfico e segregação de responsabilidades entre XAML e C#. |
| **Introdução ao WPF** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/getting-started/) | Estrutura de compilação SDK .NET 10 e inicialização da aplicação. |
| **Visão Geral do XAML** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/xaml/) | Sintaxe declarativa de marcação vetorial e resolução de nós da interface gráfica. |
| **Sintaxe XAML em Detalhes** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/advanced/xaml-syntax-in-detail/) | Sintaxe de propriedades anexadas, propriedades de dependência e extensões de marcação. |
| **Classe Application** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.application) | Ponto de entrada global do executável configurado em `App.xaml` e `App.xaml.cs`. |
| **Classe Window** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.window) | Janela principal de visualização da locomotiva declarada em `MainWindow`. |
| **Classe UserControl** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.controls.usercontrol) | Encapsulamento modular dos componentes visuais do trem em `LocomotivaControl`. |

---

## 2. Layout, Contêineres e Primitivas Gráficas Vetoriais

| Módulo / API | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral de Layout** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/layout/) | Pipeline de layout de duas etapas (`Measure` e `Arrange`) do motor de renderização. |
| **Elemento Canvas** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/canvas/) | Contêiner de posicionamento cartesiano explícito e ancoragem das peças na origem $(0,0)$. |
| **Formas e Desenho Básico** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/) | Primitivas `Rectangle`, `Ellipse`, `Polygon` e `Line` ancoradas em coordenadas canônicas. |
| **Classe Rectangle** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle/) | Vigas estruturais do chassi, corpo do cilindro, caldeira e hastes retangulares. |
| **Classe Ellipse** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse/) | Aros das rodas, mancais/olhais das bielas, domo de vapor e partículas de condensação. |
| **Classe Polygon** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon/) | Limpa-trilhos triangular (*cowcatcher*), teto da cabine e projeção de luz do farol. |
| **Classe Line** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line/) | Raios convergentes das rodas motrizes, guias da cruzeta (*slide bars*) e corrimãos. |
| **Pincéis e Gradientes** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/wpf-brushes-overview/) | Pincéis `SolidColorBrush` e gradientes lineares `LinearGradientBrush` do céu noturno. |

---

## 3. Álgebra Linear e Transformações Afins 2D

| Módulo / API | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral de Transformações** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/) | Teoria das transformações afins, matrizes homogêneas $3 \times 3$ e espaços de coordenadas. |
| **RenderTransform vs LayoutTransform** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/#differences-between-the-rendertransform-and-layouttransform-properties) | Justificativa técnica para aplicação exclusiva de `RenderTransform` a 60/144 FPS na GPU. |
| **Classe TranslateTransform** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.translatetransform/) | Translação linear da locomotiva, das bielas, da cruzeta e peças canônicas. |
| **Classe RotateTransform** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/) | Rotação síncrona das rodas motrizes em torno do centro $(40,40)$ e inclinação da biela. |
| **Classe ScaleTransform** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform/) | Expansão volumétrica de vapor e inversão simétrica de orientação de sentido (`ScaleX = -1.0`). |
| **Classe TransformGroup** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.transformgroup/) | Composição hierárquica simultânea de escala, rotação e translação no mesmo elemento. |

---

## 4. Templates, Recursos e Estilos XAML

| Módulo / API | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral de Estilos e Modelos** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/styles-templates-overview/) | Segregação estrita entre visualização gráfica e controle de comportamento. |
| **Modelos de Controle (ControlTemplate)** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/) | Implementação do `RodaTemplate` e `MancalBielaTemplate` conforme o padrão do relógio. |
| **Classe ResourceDictionary** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.resourcedictionary) | Dicionário de recursos `LocomotivaResources.xaml` para isolamento de templates. |
| **Propriedade MergedDictionaries** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.resourcedictionary.mergeddictionaries) | Fusão modular de dicionários de recursos estáticos no escopo de `App.xaml`. |
| **Recursos XAML** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/systems/xaml-resources/) | Definição e consumo de recursos estáticos e compartilhados via `StaticResource`. |

---

## 5. Arquitetura MVVM e Vinculação de Dados (*Data Binding*)

| Módulo / API | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Padrão MVVM no WPF** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/architecture/maui/mvvm) | Padrão arquitetural Model-View-ViewModel aplicado para segregação de lógica e visão. |
| **Visão Geral de Vinculação de Dados** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/data/) | Ligação declarativa de propriedades entre ViewModel e controles XAML via `{Binding}`. |
| **Interface INotifyPropertyChanged** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.componentmodel.inotifypropertychanged) | Contrato de notificação de alteração de propriedades implementado em `ViewModelBase`. |
| **Classe PropertyChangedEventArgs** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.componentmodel.propertychangedeventargs) | Otimização com instâncias estáticas cacheadas para atingir Zero-Allocation na heap. |
| **Estruturas de Registro (Record Structs)** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/builtin-types/record#structs) | `LocomotivaFrameState` imutável e alocada na pilha (*Stack*) do runtime .NET. |

---

## 6. Renderização em Tempo Real e Cinemática Analítica

| Módulo / API | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Renderização por Intervalo de Quadro** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/how-to-render-on-a-per-frame-interval-using-compositiontarget) | Padrão arquitetural de alta performance para simulação física em tempo real no WPF. |
| **Evento CompositionTarget.Rendering** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering) | Gancho síncrono disparado imediatamente antes de cada etapa de composição na GPU. |
| **Classe Stopwatch** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch) | Medição contínua de tempo com resolução de alta precisão (`QueryPerformanceCounter`). |
| **Classe Math (Atan2, Cos, Sin, Sqrt)** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.math) | Resolução analítica do mecanismo biela-manivela e perfil de aceleração cosseno. |

---

## 7. Animações Declarativas e Storyboards

| Módulo / API | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Visão Geral de Animação** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/animation-overview/) | Linhas do tempo, interpolação matemática de dependência e taxas de atualização. |
| **Visão Geral de Storyboards** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/) | Agrupamento de linhas do tempo vetoriais em `Canvas.Triggers`. |
| **Classe DoubleAnimation** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.animation.doubleanimation/) | Interpolação suave de coordenadas, escala e atenuação de opacidade da fumaça. |
| **Classe EventTrigger** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.eventtrigger/) | Disparo automático das animações associado ao evento de carregamento `Loaded`. |

---

## 8. Qualidade de Código, Convenções C# e EditorConfig

| Módulo / API | URL Oficial do Microsoft Learn | Aplicação no Projeto |
| :--- | :--- | :--- |
| **Convenções de Código C#** | [Acessar Documentação](https://learn.microsoft.com/pt-br/dotnet/csharp/fundamentals/coding-style/coding-conventions) | Padrões de nomenclatura pascalina, métodos puros e Clean Code. |
| **EditorConfig no Visual Studio / .NET** | [Acessar Documentação](https://learn.microsoft.com/pt-br/visualstudio/ide/create-portable-custom-editor-options) | Configuração portátil de tabulação, terminação de linha `CRLF` e conjunto de caracteres UTF-8. |
