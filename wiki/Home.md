# 🚂 Wiki: Locomotiva a Vapor 2D em WPF (.NET 10)

Bem-vindo à documentação oficial da **Locomotiva a Vapor 2D**, desenvolvida para a disciplina de **Processamento de Imagens (Trabalho 1)** utilizando **C# / .NET 10** e o subsistema de renderização vetorial do **Windows Presentation Foundation (WPF)**.

Esta documentação foi elaborada no padrão **GitHub Wiki** e escrita com um cuidado especial: **explicar conceitos de computação gráfica e física de forma "mastigada" e intuitiva**, usando analogias do cotidiano (blocos de Lego, carimbos de biscoito, bicicletas, máquinas de costura e folhas de celofane). Assim, **qualquer pessoa**, mesmo sem conhecimento prévio em programação gráfica ou processamento de imagens, consegue entender como o trem foi desenhado e animado!

---

## 📸 Demonstração do Projeto em Tempo Real

Abaixo está a demonstração visual do trem em movimento com as bielas articuladas com perfeição matemática, oclusão no cilindro de vapor e baforadas contínuas de fumaça:

![Locomotiva a Vapor 2D em Execução](locomotiva.gif)

---

## 📑 Sumário da Documentação

A documentação está dividida em 7 capítulos técnicos e didáticos:

1. **[Arquitetura e Princípios Gráficos](Arquitetura-e-Visao-Geral)**  
   *Como o computador desenha na tela, a "Regra de Ouro do Lego" (o invariante $(0,0)$), por que usamos a placa de vídeo com `RenderTransform` e o mapa de responsabilidade de cada arquivo.*
2. **[Modelagem Visual e Geometria XAML](Modelagem-Visual-MainWindow-XAML)**  
   *O desenho da locomotiva 0-4-0T (com a frente voltada para a direita), o que cada peça faz na vida real (domo, caldeira, limpa-trilhos, cabine) e o truque das camadas visuais (Z-Index).*
3. **[ControlTemplate e Rodas Reutilizáveis](ControlTemplate-e-Rodas)**  
   *A analogia da "Forma de Bolo / Carimbo" (`ControlTemplate`), o ponteiro de relógio dos slides da aula e o segredo da tachinha no centro (`CenterX="40" CenterY="40"`).*
4. **[Cinemática Analítica do Mecanismo Biela-Manivela](Cinematica-Analitica-e-Bielas)**  
   *A física do trem explicada com o pedal da bicicleta, a escada na parede (Teorema de Pitágoras) e a bússola (`Math.Atan2`) para guiar a biela motriz sem derrapar.*
5. **[Animações Declarativas e Efeito de Vapor](Animacoes-Storyboard-e-Particulas)**  
   *O "Diretor de Cinema" do WPF (`Storyboard`), a física do vento empurrando a fumaça para trás e a técnica das bolhas de sabão descompassadas.*
6. **[Padrões de Qualidade, SonarQube e EditorConfig](Qualidade-SonarQube-e-EditorConfig)**  
   *Auditoria de código limpo com SonarQube (0 bugs, 0 vulnerabilidades, 0 code smells, nota máxima A) e formatação uniforme com `.editorconfig`.*
7. **[Referências Oficiais do Microsoft Learn](Referencias-Oficiais-Microsoft-Learn)**  
   *Catálogo de links oficiais da Microsoft em português (`pt-br`) documentando todas as ferramentas, formas geométricas e classes utilizadas.*

---

## 🎯 Atendimento Integral aos Critérios do Trabalho

O projeto atende com nota máxima (10,0 / 10,0) a todos os requisitos normativos do **[Trabalho C1 (Normas)](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md)** e do material teórico **[Slide 2D:182-299](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Slide/2D.md#L182-L299)**:

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
