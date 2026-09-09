# 🚂 Wiki: Locomotiva a Vapor 2D em WPF (.NET 10)

Bem-vindo à documentação técnica oficial da **Locomotiva a Vapor 2D**, projeto desenvolvido para a disciplina de **Processamento de Imagens (Trabalho 1)** utilizando **C# / .NET 10** e o subsistema de renderização vetorial do **Windows Presentation Foundation (WPF)**.

Esta documentação foi elaborada no padrão **GitHub Wiki**, apresentando a engenharia do projeto de forma estruturada, didática e aprofundada: desde os conceitos fundamentais de computação gráfica 2D até a análise minuciosa de cada trecho de código XAML e C#, tornando a arquitetura perfeitamente compreensível tanto para iniciantes quanto para revisores técnicos.

---

## 📸 Demonstração do Projeto em Tempo Real

Abaixo está a demonstração visual do trem em movimento com as bielas articuladas com precisão analítica, oclusão dinâmica no bloco do cilindro de vapor e emissão contínua de partículas de fumaça:

![Locomotiva a Vapor 2D em Execução](locomotiva.gif)

---

## 📑 Sumário da Documentação

A documentação está dividida em 7 capítulos técnicos especializados:

1. **[Arquitetura e Princípios Gráficos](Arquitetura-e-Visao-Geral)**  
   *O sistema de coordenadas do WPF, o invariante de projeto $(0,0)$, aceleração gráfica por hardware via `RenderTransform` e a separação de responsabilidades em camadas (SRP).*
2. **[Modelagem Visual e Geometria XAML](Modelagem-Visual-MainWindow-XAML)**  
   *A decomposição estrutural da locomotiva 0-4-0T (orientada para a direita), análise funcional dos componentes (caldeira, cabine, cilindro, cowcatcher) e a pilha de renderização em camadas ($Z\text{-Index}$).*
3. **[ControlTemplate e Rodas Reutilizáveis](ControlTemplate-e-Rodas)**  
   *O padrão de template modular (`ControlTemplate`), o modelo circular inspirado no relógio analógico dos slides e a parametrização do centro pivô de rotação (`CenterX="40"` e `CenterY="40"`).*
4. **[Cinemática Analítica do Mecanismo Biela-Manivela](Cinematica-Analitica-e-Bielas)**  
   *A cinemática física no motor desacoplado `LocomotivaKinematics.cs`: rolamento puro sem deslizamento, órbita circular síncrona, Teorema de Pitágoras e cálculo angular via `Math.Atan2`.*
5. **[Animações Declarativas e Efeito de Vapor](Animacoes-Storyboard-e-Particulas)**  
   *O subsistema de animações declarativas em XAML (`Storyboard`), a modelagem física de arrasto aerodinâmico e expansão gasosa, e o escalonamento temporal contínuo.*
6. **[Padrões de Qualidade, SonarQube e EditorConfig](Qualidade-SonarQube-e-EditorConfig)**  
   *Auditoria de código limpo com SonarQube Community (0 bugs, 0 vulnerabilidades, 0 code smells, nota máxima A) e formatação uniforme com `.editorconfig`.*
7. **[Referências Oficiais do Microsoft Learn](Referencias-Oficiais-Microsoft-Learn)**  
   *Catálogo de links oficiais da Microsoft em português (`pt-br`) documentando as classes, transformações afins e subsistemas utilizados.*

---

## 🎯 Atendimento Integral aos Critérios do Trabalho

O projeto atende integralmente a todos os requisitos normativos do **[Trabalho C1 (Normas)](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md)** e do material teórico **[Slide 2D:182-299](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Slide/2D.md#L182-L299)**:

| Etapa | Pontuação | Requisito Normativo | Implementação Técnica no Projeto |
| :---: | :---: | :--- | :--- |
| **Etapa 1** | **3,0 pts** | Corpo estático (`Rectangle`, `Polygon`, `Ellipse`) desenhado em `(0,0)` com `RenderTransform` + `ControlTemplate` de roda com raios visíveis e $\ge 2$ instâncias sob o chassi. | Chassi, cabine com janelas e portas, caldeira com cintas, tanques e cilindro desenhados em `(0,0)` e posicionados com `TranslateTransform`. `RodaTemplate` parametrizado com aro, 8 raios, contrapeso e manivela instanciado duas vezes. |
| **Etapa 2** | **6,0 pts** | Objeto completo agrupado em `Canvas`, `RotateTransform` com rotação contínua nas rodas e `TranslateTransform` para movimentação horizontal da locomotiva. | Conjunto inteiro encapsulado em `LocomotivaCanvas`. Instâncias de rodas contêm `RotateTransform` acopladas ao eixo. Translação global horizontal suave no cenário ferroviário. |
| **Etapa 3** | **8,0 pts** | Bielas conectando as rodas com movimento mecânico sincronizado simulando o acoplamento real ("combinação de transformações a cada quadro"). | **Biela de Acoplamento** (*Side Rod*) mantida horizontal em órbita síncrona. **Biela Motriz** (*Connecting Rod*) e **Cruzeta** (*Crosshead*) calculadas com cinemática analítica exata via Pitágoras e `Math.Atan2` em `CompositionTarget.Rendering`. |
| **Etapa 4** | **10,0 pts** | Locomotiva completa em movimento contínuo nos limites da janela do aplicativo. | **Loop Contínuo (Túnel Ferroviário)**: A locomotiva emerge completamente da esquerda fora da janela ($X = -560$), atravessa o cenário ferroviário em velocidade constante até sair completamente pela direita ($X = \text{largura}$) e reentra instantaneamente pela esquerda em loop contínuo infinito, com rotação puramente monotônica das rodas. |

---

## 🚀 Como Compilar e Executar

### Pré-requisitos
- Sistema Operacional: Windows 10 / 11 (requerido para WPF nativo).
- SDK: [.NET 10 SDK](https://dotnet.microsoft.com/download) ou superior.

### Comandos no Terminal

```powershell
# 1. Navegar até a pasta raiz do projeto
cd d:\ProcessamentoDeImagens\PI_T1

# 2. Restaurar dependências e compilar a solução
dotnet build

# 3. Executar o aplicativo interativo
dotnet run
```

---

## 🔗 Referências Principais
- [Microsoft Learn — Visão geral de transformações no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/)
- [Microsoft Learn — Como renderizar em um intervalo por quadro usando CompositionTarget](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/how-to-render-on-a-per-frame-interval-using-compositiontarget)
- [Microsoft Learn — Visão geral de estilos e modelos (ControlTemplate)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/styles-templates-overview/)
- [Microsoft Learn — Formas e desenho básico no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/)
