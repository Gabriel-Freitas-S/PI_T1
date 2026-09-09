# Animações Declarativas via Storyboard e Subsistema de Partículas de Vapor

![Módulo](https://img.shields.io/badge/M%C3%B3dulo-Anima%C3%A7%C3%B5es%20Declarativas-007ACC?style=flat-square)
![Subsistema](https://img.shields.io/badge/Subsistema-Storyboard%20XAML-512BD4?style=flat-square)
![Física](https://img.shields.io/badge/F%C3%ADsica-Pluma%20de%20Vapor%20e%20Arrasto-2ecc71?style=flat-square)
![Renderização](https://img.shields.io/badge/Renderiza%C3%A7%C3%A3o-Offline%20GIF%20(Determin%C3%ADstica)-blue?style=flat-square)

A representação visual verossímil de uma locomotiva a vapor clássica requer a reprodução contínua dos fenômenos de exaustão gasosa expelidos ciclicamente pela chaminé.

Neste capítulo, detalha-se a implementação do subsistema de partículas e animações declarativas utilizando a classe [`Storyboard`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/) do WPF, bem como a rotina determinística de renderização procedural para captura *offscreen* de quadros.

---

## 1. Fundamentos da Animação Declarativa via `Storyboard`

No subsistema gráfico do WPF, um [`Storyboard`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/) atua como um gerenciador e orquestrador de linhas do tempo para animação de propriedades de dependência (*Dependency Properties*).

Em vez de manipular imperativamente coordenadas quadro a quadro via laços manuais na thread de interface, a marcação XAML declara regras explícitas de interpolação temporal:

- **Origem e Destino**: Especifica-se o valor inicial (`From`) e o valor terminal (`To`).
- **Duração e Repetição**: Define-se o intervalo de tempo (`Duration`) e o comportamento cíclico perpétuo (`RepeatBehavior="Forever"`).
- **Processamento Assíncrono**: O subsistema de temporização da composição gráfica calcula os valores intermediários diretamente no pipeline de renderização, garantindo alta fidelidade visual sem onerar a lógica principal da aplicação.

---

## 2. Modelagem Cinemática e Fenomenologia do Efeito de Vapor

Para simular a dispersão das partículas gasosas em conformidade com as leis da dinâmica dos fluidos, a trajetória espacial é decomposta em quatro variáveis físicas:

1. **Convecção Térmica Ascendente (Empuxo)**: O vapor superaquecido possui densidade significativamente menor que o ar atmosférico circundante, gerando empuxo vertical positivo. No sistema cartesiano do WPF (onde a ordenada $Y$ cresce no sentido descendente), a ascensão é expressa por uma variação negativa: $Y = 40.0 \to -40.0\text{ px}$.
2. **Arrasto Aerodinâmico Relativo (Resistência do Ar)**: Durante o avanço da composição ferroviária para a direita, a massa de ar estacionária impõe uma força de resistência no sentido oposto (para trás). Consequentemente, a coordenada horizontal $X$ decresce progressivamente em relação à chaminé: $X = 395.0 \to 320.0\text{ px}$.
3. **Expansão Volumétrica por Descompressão**: Ao escapar da pressão interna da chaminé para o ambiente atmosférico livre, a nuvem sofre expansão volumétrica contínua. Essa deformação é computada via transformações afins de escala ([`ScaleTransform`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform/)), variando os fatores de escala linearmente de $0.5$ até $2.2$.
4. **Dissipação e Atenuação de Concentração**: O resfriamento térmico e a mistura turbulenta reduzem progressivamente a opacidade visível da pluma, modelada pela atenuação monotônica da propriedade `Opacity` de $0.8$ (alta densidade na boca da chaminé) até $0.0$ (dissipação completa no ar).

```text
        (Partícula 3: Raio expandido e opacidade atenuada)
             ( )
           (     )
             \
        (Partícula 2: Expansão intermediária em ascensão)
           (   )
             \
        (Partícula 1: Dimensão inicial e densidade máxima)
          (o)
           ||   <-- Duto da Chaminé
       +--------+
       | CALDEIRA |  ====> [Vetor de Deslocamento para a Direita]
```

---

## 3. Emissão Contínua Escalonada por Defasagem Temporal (*Staggering*)

Para assegurar uma pluma contínua e sem hiatos visíveis entre os ciclos individuais de exaustão, o sistema instancia **três partículas circulares independentes** (`Fumaca1`, `Fumaca2` e `Fumaca3`) operando com defasagem de fase e períodos temporais assimétricos:

- **Partícula 1**: Ciclo base com período $T_1 = 1.5\text{ s}$, iniciado em $t_0 = 0.0\text{ s}$.
- **Partícula 2**: Ciclo com período $T_2 = 1.8\text{ s}$, defasado em $0.5\text{ s}$ (`BeginTime="0:0:0.5"`).
- **Partícula 3**: Ciclo com período $T_3 = 2.0\text{ s}$, defasado em $1.0\text{ s}$ (`BeginTime="0:0:1.0"`).

A assimetria entre os períodos temporais ($1.5\text{ s}$, $1.8\text{ s}$ e $2.0\text{ s}$) gera um mínimo múltiplo comum longo, impedindo que as três partículas atinjam seus pontos de expansão simultaneamente e garantindo aspecto dinâmico e contínuo à exaustão.

---

## 4. Análise Detalhada da Implementação

### 4.1 Primitivas Visuais das Partículas em `Controls/LocomotivaControl.xaml`

As partículas são modeladas como elipses translúcidas encapsuladas em contêineres com matrizes de escala e translação afim:

```xml
<Ellipse x:Name="Fumaca1" Width="20" Height="20" Fill="#65FFFFFF">
    <Ellipse.RenderTransform>
        <TransformGroup>
            <!-- Centro de escala fixado no meio da partícula (10, 10) -->
            <ScaleTransform x:Name="EscalaFumaca1" ScaleX="1" ScaleY="1" CenterX="10" CenterY="10"/>
            <!-- Posição inicial na boca da chaminé em (395, 20) -->
            <TranslateTransform x:Name="TranslacaoFumaca1" X="395" Y="20"/>
        </TransformGroup>
    </Ellipse.RenderTransform>
</Ellipse>
```

- `Fill="#65FFFFFF"`: Branco translúcido com canal alfa em `0x65` (aproximadamente $40\%$ de opacidade).
- `CenterX="10" CenterY="10"`: Como a elipse possui dimensões $20 \times 20\text{ px}$, fixar o pivô de escala em $(10,10)$ assegura que a partícula se expanda radialmente a partir de seu centro, sem deslocamentos laterais indesejados.

---

### 4.2 Orquestração Declarativa via `Storyboard` em XAML

A linha do tempo completa é disparada automaticamente no carregamento do contêiner através de um [`EventTrigger`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.eventtrigger/):

```xml
<Canvas.Triggers>
    <EventTrigger RoutedEvent="Canvas.Loaded">
        <BeginStoryboard>
            <Storyboard RepeatBehavior="Forever">

                <!-- ===== PARTÍCULA 1 (Período: 1.5s) ===== -->
                <!-- Ascensão vertical de Y=40 até Y=-40 -->
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca1" Storyboard.TargetProperty="Y"
                                 From="40" To="-40" Duration="0:0:1.5" RepeatBehavior="Forever"/>

                <!-- Arrasto horizontal para trás de X=395 até X=320 -->
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca1" Storyboard.TargetProperty="X"
                                 From="395" To="320" Duration="0:0:1.5" RepeatBehavior="Forever"/>

                <!-- Expansão de escala de 0.5 a 2.2 -->
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca1" Storyboard.TargetProperty="ScaleX"
                                 From="0.5" To="2.2" Duration="0:0:1.5" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca1" Storyboard.TargetProperty="ScaleY"
                                 From="0.5" To="2.2" Duration="0:0:1.5" RepeatBehavior="Forever"/>

                <!-- Atenuação de opacidade de 0.8 a 0.0 -->
                <DoubleAnimation Storyboard.TargetName="Fumaca1" Storyboard.TargetProperty="Opacity"
                                 From="0.8" To="0.0" Duration="0:0:1.5" RepeatBehavior="Forever"/>

                <!-- ===== PARTÍCULA 2 (Início: 0.5s | Período: 1.8s) ===== -->
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca2" Storyboard.TargetProperty="Y"
                                 BeginTime="0:0:0.5" From="40" To="-50" Duration="0:0:1.8" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca2" Storyboard.TargetProperty="X"
                                 BeginTime="0:0:0.5" From="395" To="300" Duration="0:0:1.8" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca2" Storyboard.TargetProperty="ScaleX"
                                 BeginTime="0:0:0.5" From="0.4" To="2.5" Duration="0:0:1.8" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca2" Storyboard.TargetProperty="ScaleY"
                                 BeginTime="0:0:0.5" From="0.4" To="2.5" Duration="0:0:1.8" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="Fumaca2" Storyboard.TargetProperty="Opacity"
                                 BeginTime="0:0:0.5" From="0.7" To="0.0" Duration="0:0:1.8" RepeatBehavior="Forever"/>

                <!-- ===== PARTÍCULA 3 (Início: 1.0s | Período: 2.0s) ===== -->
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca3" Storyboard.TargetProperty="Y"
                                 BeginTime="0:0:1.0" From="40" To="-60" Duration="0:0:2.0" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca3" Storyboard.TargetProperty="X"
                                 BeginTime="0:0:1.0" From="395" To="275" Duration="0:0:2.0" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca3" Storyboard.TargetProperty="ScaleX"
                                 BeginTime="0:0:1.0" From="0.3" To="2.8" Duration="0:0:2.0" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca3" Storyboard.TargetProperty="ScaleY"
                                 BeginTime="0:0:1.0" From="0.3" To="2.8" Duration="0:0:2.0" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="Fumaca3" Storyboard.TargetProperty="Opacity"
                                 BeginTime="0:0:1.0" From="0.6" To="0.0" Duration="0:0:2.0" RepeatBehavior="Forever"/>
            </Storyboard>
        </BeginStoryboard>
    </EventTrigger>
</Canvas.Triggers>
```

---

### 4.3 Renderização Determinística em C# (`AtualizarFumaca`)

Para possibilitar a captura automatizada e determinística de quadros para o arquivo GIF (`dotnet run -- --record-frames`), a evolução temporal da pluma também foi transcrita analiticamente no método procedural `AtualizarFumaca` em [`Controls/LocomotivaControl.xaml.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml.cs):

```csharp
public void AtualizarFumaca(double tempo)
{
    // Partícula 1 (Período: 1.5s): progresso normalizado p1 in [0.0, 1.0)
    double p1 = (tempo % 1.5) / 1.5;
    TranslacaoFumaca1.Y = 40.0 - (80.0 * p1);          // Ascensão de 40 a -40 px
    TranslacaoFumaca1.X = 395.0 - (75.0 * p1);         // Arrasto relativo de 395 a 320 px
    EscalaFumaca1.ScaleX = 0.5 + (1.7 * p1);           // Fator de escala de 0.5 a 2.2
    EscalaFumaca1.ScaleY = 0.5 + (1.7 * p1);
    Fumaca1.Opacity = 0.8 * (1.0 - p1);                // Atenuação de opacidade de 0.8 a 0.0

    // Partícula 2 (Período: 1.8s com defasagem de fase)
    double p2 = ((tempo + 1.0) % 1.8) / 1.8;
    TranslacaoFumaca2.Y = 40.0 - (90.0 * p2);
    TranslacaoFumaca2.X = 395.0 - (95.0 * p2);
    EscalaFumaca2.ScaleX = 0.4 + (2.1 * p2);
    EscalaFumaca2.ScaleY = 0.4 + (2.1 * p2);
    Fumaca2.Opacity = 0.7 * (1.0 - p2);

    // Partícula 3 (Período: 2.0s com defasagem de fase)
    double p3 = ((tempo + 0.5) % 2.0) / 2.0;
    TranslacaoFumaca3.Y = 40.0 - (100.0 * p3);
    TranslacaoFumaca3.X = 395.0 - (120.0 * p3);
    EscalaFumaca3.ScaleX = 0.3 + (2.5 * p3);
    EscalaFumaca3.ScaleY = 0.3 + (2.5 * p3);
    Fumaca3.Opacity = 0.6 * (1.0 - p3);
}
```

#### Determinismo na Exportação de Mídia

Enquanto a execução interativa em tela depende do relógio do sistema operacional (*wall-clock time*), o modo de gravação estática discretiza 300 quadros sobre um ciclo temporal fechado de $15.0\text{ s}$ ($20.0\text{ FPS}$). O método procedural garante que cada imagem renderizada corresponda ao instante temporal exato prescrito pela simulação física.

---

## Referências Oficiais da Microsoft

- [Microsoft Learn — Visão Geral de Animação no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/animation-overview/)
- [Microsoft Learn — Visão Geral de Storyboards](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/)
- [Microsoft Learn — Classe DoubleAnimation](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.animation.doubleanimation/)
- [Microsoft Learn — Classe ScaleTransform](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform/)
- [Microsoft Learn — Classe EventTrigger](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.eventtrigger/)
