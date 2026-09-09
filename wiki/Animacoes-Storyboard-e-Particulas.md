# 💨 Animações Declarativas e Efeito de Vapor em XAML

A representação visual de uma locomotiva a vapor clássica requer a reprodução dos fenômenos de exaustão gasosa expelidos periodicamente pela chaminé.

Neste capítulo, detalha-se a implementação do subsistema de partículas e animações declarativas utilizando a classe [`Storyboard`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/) do WPF, garantindo simulação fluida e eficiente em termos de consumo computacional.

---

## 1. Fundamentos da Animação Declarativa via `Storyboard`

No ecossistema WPF, um [`Storyboard`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/) atua como um orquestrador de linhas do tempo para animação de propriedades de dependência (*Dependency Properties*).

Em vez de manipular imperativamente coordenadas quadro a quadro via código procedural, a interface declara regras de interpolação matemática temporal diretamente em XAML:
- Especifica-se o valor inicial (`From`), o valor terminal (`To`) e a duração do ciclo (`Duration`).
- O subsistema de temporização da GPU/WPF calcula as posições intermediárias e aplica a interpolação sem bloquear a thread de UI principal.
- Com a propriedade `RepeatBehavior="Forever"`, a linha do tempo reinicia ciclicamente de forma contínua e autônoma.

---

## 2. Modelagem Cinemática e Fenomenologia do Efeito de Vapor

Para reproduzir a dispersão de partículas gasosas de forma verossímil, modelou-se a evolução das coordenadas considerando quatro variáveis físicas fundamentais:

1. **Convecção Térmica Ascendente**: O vapor aquecido possui menor densidade em relação ao ar ambiente, gerando empuxo vertical. No sistema de coordenadas do WPF (onde a ordenada $Y$ cresce para baixo), a ascensão vertical é descrita por variação negativa em $Y$ ($Y = 40 \to -40$).
2. **Arrasto Aerodinâmico Relativo**: Com o deslocamento da composição ferroviária no sentido positivo do eixo horizontal (para a direita), a resistência do ar induz um vetor de arraste no sentido oposto (para a esquerda). Consequentemente, o deslocamento horizontal $X$ decresce ao longo do ciclo ($X = 395 \to 320$).
3. **Expansão Volumétrica e Difusão**: Ao despressurizar na saída do duto da chaminé, o volume gasoso expande-se progressivamente. Essa variação dimensional é implementada via transformações afins de escala ([`ScaleTransform`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform/)), modulando os fatores de escala de $0.5$ até $2.2$.
4. **Dissipação e Atenuação de Opacidade**: O decaimento térmico e a mistura convectiva reduzem progressivamente a concentração visível da nuvem, representada pela atenuação linear da propriedade `Opacity` de $0.8$ a $0.0$.

```
        (Partícula 3: Raio expandido e opacidade atenuada)
             ( )
           (     )
             \
        (Partícula 2: Expansão intermediária em ascensão)
           (   )
             \
        (Partícula 1: Dimensão inicial e opacidade máxima)
          (o)
           ||   <-- Duto da Chaminé
       +--------+
       | CALDEIRA |  ====> [Vetor de Deslocamento para a Direita]
```

---

## 3. Emissão Contínua Escalonada por Defasagem Temporal (*Staggering*)

Para garantir emissão ininterrupta de vapor sem descontinuidades temporais (*gaps* entre ciclos individuais), implementou-se um sistema com **três partículas circulares independentes** (`Fumaca1`, `Fumaca2` e `Fumaca3`) operando com defasagem de fase e períodos assimétricos:
- **Partícula 1**: Ciclo base de $1.5\text{ s}$, iniciado em $t_0 = 0.0\text{ s}$.
- **Partícula 2**: Ciclo de $1.8\text{ s}$, defasado em $0.5\text{ s}$ (`BeginTime="0:0:0.5"`).
- **Partícula 3**: Ciclo de $2.0\text{ s}$, defasado em $1.0\text{ s}$ (`BeginTime="0:0:1.0"`).

A assimetria entre os períodos temporais ($1.5\text{ s}$, $1.8\text{ s}$ e $2.0\text{ s}$) produz um longo mínimo múltiplo comum, impedindo a sincronização em fase e conferindo aspecto orgânico contínuo à pluma de vapor.

---

## 4. 📖 Análise Linha a Linha do Código da Fumaça

Vamos agora abrir o arquivo [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml) e ver como isso foi programado na prática:

### 4.1 A Criação das Partículas Visuais de Fumaça
Antes de animar, precisamos criar os círculos translúcidos no topo da chaminé:

```xml
<Ellipse x:Name="Fumaca1" Width="20" Height="20" Fill="#65FFFFFF">
    <Ellipse.RenderTransform>
        <TransformGroup>
            <!-- Escala: cresce inflando a partir do centro (10, 10) -->
            <ScaleTransform x:Name="EscalaFumaca1" ScaleX="1" ScaleY="1" CenterX="10" CenterY="10"/>
            <!-- Posição inicial: bem na boca da chaminé (X=395, Y=20) -->
            <TranslateTransform x:Name="TranslacaoFumaca1" X="395" Y="20"/>
        </TransformGroup>
    </Ellipse.RenderTransform>
</Ellipse>
```
- `Fill="#65FFFFFF"`: Branco com transparência (o canal alfa `65` equivale a cerca de $40\%$ de opacidade).
- `CenterX="10" CenterY="10"`: Como a elipse tem tamanho $20 \times 20$, o centro de expansão é $(10,10)$. Se não colocássemos isso, a bolha cresceria para o lado em vez de inflar redonda!

---

### 4.2 Definição Declarativa do `Storyboard` em XAML

A orquestração temporal das partículas é configurada declarativamente no bloco de gatilhos do contêiner:

```xml
<Canvas.Triggers>
    <!-- Quando a locomotiva nasce na tela, dispara a animação imediatamente -->
    <EventTrigger RoutedEvent="Canvas.Loaded">
        <BeginStoryboard>
            <Storyboard RepeatBehavior="Forever">

                <!-- ===== BAFORADA 1 (Período: 1.5 segundos) ===== -->
                <!-- 1. Subir para o céu: vai de Y=40 até Y=-40 -->
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca1" Storyboard.TargetProperty="Y"
                                 From="40" To="-40" Duration="0:0:1.5" RepeatBehavior="Forever"/>

                <!-- 2. Vento soprando para trás: vai de X=395 para trás até X=320 -->
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca1" Storyboard.TargetProperty="X"
                                 From="395" To="320" Duration="0:0:1.5" RepeatBehavior="Forever"/>

                <!-- 3. Inflar de tamanho: de metade (0.5) até mais que o dobro (2.2) -->
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca1" Storyboard.TargetProperty="ScaleX"
                                 From="0.5" To="2.2" Duration="0:0:1.5" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca1" Storyboard.TargetProperty="ScaleY"
                                 From="0.5" To="2.2" Duration="0:0:1.5" RepeatBehavior="Forever"/>

                <!-- 4. Dissipar: de visível (0.8) até sumir no ar (0.0) -->
                <DoubleAnimation Storyboard.TargetName="Fumaca1" Storyboard.TargetProperty="Opacity"
                                 From="0.8" To="0.0" Duration="0:0:1.5" RepeatBehavior="Forever"/>

                <!-- ===== BAFORADA 2 (Começa 0.5s depois e dura 1.8s) ===== -->
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

                <!-- ===== BAFORADA 3 (Começa 1.0s depois e dura 2.0s) ===== -->
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
- O escalonamento temporal via `BeginTime` (`0:0:0.0` $\to$ `0:0:0.5` $\to$ `0:0:1.0`) desfaz o alinhamento de fase entre as três instâncias, mantendo a densidade volumétrica contínua ao longo do tempo.

---

### 4.3 Renderização Procedural em C# (`AtualizarFumaca`)

Para possibilitar a renderização determinística de quadros estáticos na exportação do GIF animado (`dotnet run -- --record-frames`), a evolução temporal da pluma de vapor também foi formulada analiticamente em C# no arquivo [`Controls/LocomotivaControl.xaml.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml.cs):

```csharp
public void AtualizarFumaca(double tempo)
{
    // Partícula 1 (Período: 1.5s): progresso normalizado p1 in [0.0, 1.0)
    double p1 = (tempo % 1.5) / 1.5;
    TranslacaoFumaca1.Y = 40.0 - 80.0 * p1;           // Ascensão de 40 a -40 px
    TranslacaoFumaca1.X = 395.0 - 75.0 * p1;          // Arrasto relativo de 395 a 320 px
    EscalaFumaca1.ScaleX = 0.5 + 1.7 * p1;            // Fator de escala de 0.5 a 2.2
    EscalaFumaca1.ScaleY = 0.5 + 1.7 * p1;
    Fumaca1.Opacity = 0.8 * (1.0 - p1);               // Atenuação de opacidade de 0.8 a 0.0

    // Partícula 2 (Período: 1.8s com defasagem de fase)
    double p2 = ((tempo + 1.0) % 1.8) / 1.8;
    TranslacaoFumaca2.Y = 40.0 - 90.0 * p2;
    TranslacaoFumaca2.X = 395.0 - 95.0 * p2;
    EscalaFumaca2.ScaleX = 0.4 + 2.1 * p2;
    EscalaFumaca2.ScaleY = 0.4 + 2.1 * p2;
    Fumaca2.Opacity = 0.7 * (1.0 - p2);

    // Partícula 3 (Período: 2.0s com defasagem de fase)
    double p3 = ((tempo + 0.5) % 2.0) / 2.0;
    TranslacaoFumaca3.Y = 40.0 - 100.0 * p3;
    TranslacaoFumaca3.X = 395.0 - 120.0 * p3;
    EscalaFumaca3.ScaleX = 0.3 + 2.5 * p3;
    EscalaFumaca3.ScaleY = 0.3 + 2.5 * p3;
    Fumaca3.Opacity = 0.6 * (1.0 - p3);
}
```

#### Justificativa da Parametrização Procedural
Durante a geração automatizada de fotogramas em modo *headless* ou *offscreen* (`dotnet run -- --record-frames`), o sistema captura 275 passos discretizados ao longo de um ciclo espacial fechado ($550\text{ px}$).  
Como as linhas do tempo do `Storyboard` em XAML operam atreladas ao relógio de tempo real do sistema operacional (*wall-clock time*), o método procedural `AtualizarFumaca(tempo)` garante determinismo estrito e independência de eventuais flutuações na taxa de escrita em disco.

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Visão Geral de Animação no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/animation-overview/)
- [Microsoft Learn — Visão Geral de Storyboards](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/)
- [Microsoft Learn — Classe DoubleAnimation (Animação de Números)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.animation.doubleanimation/)
- [Microsoft Learn — Classe ScaleTransform (Aumento e Diminuição de Escala)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform/)
- [Microsoft Learn — Classe EventTrigger (Disparadores de Eventos)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.eventtrigger/)
