# 💨 Animações Declarativas e Efeito de Vapor: O Diretor de Cinema do WPF

Quando vemos uma locomotiva antiga em movimento, a primeira coisa que chama a atenção é aquela fumaça branca e volumosa saindo da chaminé em baforadas ritmadas.

Neste capítulo, explicamos de forma **mastigada e acessível** como usamos a ferramenta [`Storyboard`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/) do WPF para criar esse efeito visual orgânico e realista sem sobrecarregar a memória do seu computador.

---

## 1. O que é um `Storyboard`? (A Analogia do Diretor de Cinema)

Nos estúdios de animação e cinema, um *Storyboard* é o roteiro desenhado quadro a quadro que o diretor usa para dar ordens aos atores e desenhistas.

No WPF, o [`Storyboard`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/) funciona exatamente como esse **diretor de animação automático**:
- Em vez de você ter que programar na mão cada milímetro que a fumaça sobe a cada fração de segundo, você só dá a ordem inicial em XAML:
  > *"Atenção bolha de fumaça: comece bem pequenininha e opaca em cima da chaminé, suba 80 pixels para o céu, aumente de tamanho em 2 vezes e vá ficando transparente até sumir. E faça isso durar 1 segundo e meio!"*
- E o melhor: com o comando `RepeatBehavior="Forever"`, o diretor repete essa cena sem parar enquanto o programa estiver aberto!

---

## 2. A Física da Fumaça Explicada com o Vento

Para que a fumaça pareça de verdade (e não um círculo estático colado no trem), reproduzimos os 4 comportamentos físicos que acontecem na natureza:

1. **O Vapor é Quente e Sobe**: O ar quente é mais leve que o ar frio, então a fumaça sobe em direção ao céu. No sistema de coordenadas do computador, subir significa que o valor de $Y$ diminui ($Y = 40 \to -40$).
2. **O Trem Anda para a Frente, o Vento Empurra para Trás**: Como a locomotiva está correndo para a **direita**, o vento relativo empurra a fumaça para a **esquerda** (para trás do trem). Por isso, o valor de $X$ diminui ($X = 395 \to 320$).
3. **A Fumaça se Espalha (Expansão)**: Assim que o vapor sai do cano apertado da chaminé, ele se expande no ar livre. Usamos [`ScaleTransform`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform/) para fazê-la crescer de metade do tamanho ($0.5$) até mais do que o dobro ($2.2$).
4. **Ela se Mistura com o Ar (Dissipação)**: Conforme o vapor esfria e se dispersa na noite, ele vai ficando transparente até sumir completamente (a propriedade `Opacity` vai de $0.8$ até $0.0$).

```
        (Fumaça 3: Enorme e quase invisível)
             ( )
           (     )
             \
        (Fumaça 2: Média e subindo)
           (   )
             \
        (Fumaça 1: Pequena e branca saindo agora)
          (o)
           ||   <-- Chaminé
       +--------+
       | CALDEIRA |  ====> [Trem Correndo para a Direita]
```

---

## 3. A Analogia das Bolhas de Sabão: Por Que Criamos 3 Baforadas?

Se você assoprar uma única bolha de sabão e esperar ela estourar para assoprar outra, o ar vai ficar vazio a maior parte do tempo.  
Para criar um rastro contínuo e volumoso, você assopra várias bolhas uma atrás da outra!

No nosso código, criamos **três elipses translúcidas** (`Fumaca1`, `Fumaca2` e `Fumaca3`) e usamos uma técnica chamada **escalonamento temporal (*staggering*)**:
- **Baforada 1**: Começa imediatamente no segundo `0.0s`.
- **Baforada 2**: Começa com um atraso de meio segundo (`BeginTime="0:0:0.5"`).
- **Baforada 3**: Começa com um atraso de um segundo (`BeginTime="0:0:1.0"`).

Como cada uma delas tem durações ligeiramente diferentes ($1.5\text{ s}$, $1.8\text{ s}$ e $2.0\text{ s}$), elas nunca sobem exatamente juntas. Isso cria uma ilusão visual maravilhosa de vapor constante, vivo e fofinho saindo da chaminé!

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

### 4.2 O Bloco do `Storyboard` em XAML

Veja como o diretor de animação comanda a cena:

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
- Repare no escalonamento de `BeginTime`: `0:0:0.0` $\to$ `0:0:0.5` $\to$ `0:0:1.0`. Cada baforada surge em um momento diferente e dura um pouco mais, enchendo o céu de vapor natural!

---

### 4.3 O Método C# `AtualizarFumaca`: Para Gerar o GIF e Tirar Fotos

Você sabia que o projeto também tem um método em C# para controlar a fumaça?  
No arquivo [`Controls/LocomotivaControl.xaml.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml.cs):

```csharp
public void AtualizarFumaca(double tempo)
{
    // Baforada 1 (Ciclo de 1.5s): p1 vai suavemente de 0.0 até 1.0
    double p1 = (tempo % 1.5) / 1.5;
    TranslacaoFumaca1.Y = 40.0 - 80.0 * p1;           // Sobe de 40 até -40
    TranslacaoFumaca1.X = 395.0 - 75.0 * p1;          // Vai de 395 para trás até 320
    EscalaFumaca1.ScaleX = 0.5 + 1.7 * p1;            // Infla de 0.5 até 2.2
    EscalaFumaca1.ScaleY = 0.5 + 1.7 * p1;
    Fumaca1.Opacity = 0.8 * (1.0 - p1);               // Dissipa até 0.0

    // Baforada 2 (Ciclo de 1.8s com defasagem de 0.5s)
    double p2 = ((tempo + 1.0) % 1.8) / 1.8;
    TranslacaoFumaca2.Y = 40.0 - 90.0 * p2;
    TranslacaoFumaca2.X = 395.0 - 95.0 * p2;
    EscalaFumaca2.ScaleX = 0.4 + 2.1 * p2;
    EscalaFumaca2.ScaleY = 0.4 + 2.1 * p2;
    Fumaca2.Opacity = 0.7 * (1.0 - p2);

    // Baforada 3 (Ciclo de 2.0s com defasagem de 1.0s)
    double p3 = ((tempo + 0.5) % 2.0) / 2.0;
    TranslacaoFumaca3.Y = 40.0 - 100.0 * p3;
    TranslacaoFumaca3.X = 395.0 - 120.0 * p3;
    EscalaFumaca3.ScaleX = 0.3 + 2.5 * p3;
    EscalaFumaca3.ScaleY = 0.3 + 2.5 * p3;
    Fumaca3.Opacity = 0.6 * (1.0 - p3);
}
```

#### Por que esse método C# foi criado?
Quando geramos o GIF animado em linha de comando (`dotnet run -- --record-frames`), o programa roda em modo automático e precisa capturar 275 fotos estáticas da tela uma por uma.  
Como o `Storyboard` do XAML depende do relógio de tempo real do Windows, esse método matemático garante que cada fotograma gravado no disco tenha as bolhas de fumaça exatamente no milímetro certo para o GIF ficar perfeito!

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Visão Geral de Animação no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/animation-overview/)
- [Microsoft Learn — Visão Geral de Storyboards](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/)
- [Microsoft Learn — Classe DoubleAnimation (Animação de Números)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.animation.doubleanimation/)
- [Microsoft Learn — Classe ScaleTransform (Aumento e Diminuição de Escala)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform/)
- [Microsoft Learn — Classe EventTrigger (Disparadores de Eventos)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.eventtrigger/)
