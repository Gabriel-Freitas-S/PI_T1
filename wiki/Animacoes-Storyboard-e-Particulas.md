# 💨 Animações Declarativas e Efeito de Vapor

Neste capítulo, aborda-se a implementação das animações declarativas em XAML utilizadas para gerar o efeito visual de baforadas de fumaça e vapor saindo da chaminé da locomotiva.

---

## 1. O Subsistema de Animação Declarativa do WPF

Enquanto a cinemática das rodas e das bielas foi programada imperativamente em C# via `CompositionTarget.Rendering` devido à necessidade de acoplamento físico analítico rígido, o efeito de fumaça foi implementado através do subsistema declarativo de animação do WPF baseado em [`Storyboard`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/) e [`DoubleAnimation`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.animation.doubleanimation).

### Vantagens da Animação Declarativa em XAML:
1. **Separação Limpa de Camadas**: O cálculo das partículas de vapor não consome ciclos de CPU na thread física de simulação mecânica do código C#.
2. **Execução Otimizada**: O mecanismo de animação do WPF gerencia interpolações temporais automaticamente, ajustando a taxa de quadros e aplicando interpolação cúbica/linear na thread de composição.
3. **Looping Infinito Desacoplado**: O atributo `RepeatBehavior="Forever"` garante que a fumaça continue sendo expelida ininterruptamente durante todo o tempo em que a janela estiver aberta.

---

## 2. Física Visual das Baforadas de Vapor

Uma pluma de fumaça saindo da chaminé de uma locomotiva a vapor em movimento exibe quatro comportamentos físicos simultâneos:
1. **Empuxo Térmico (Elevação Vertical)**: O vapor superaquecido sobe na atmosfera ($Y$ diminui no sistema de coordenadas do WPF).
2. **Arrasto Aerodinâmico e Inércia (Recuo Horizontal)**: À medida que a locomotiva avança, o ar empurra a pluma para trás ($X$ diminui em relação à chaminé).
3. **Expansão Gasosa (Aumento de Escala)**: A pressão diminui e o gás expande-se volumetricamente (`ScaleX` e `ScaleY` aumentam).
4. **Dissipação e Condensação (Decaimento de Opacidade)**: A mistura se dissipa no ar ambiente (`Opacity` vai de um valor visível para zero).

---

## 3. Estrutura XAML das Partículas de Vapor

Três elipses translúcidas (`Fumaca1`, `Fumaca2`, `Fumaca3`) foram posicionadas na saída superior da chaminé da locomotiva ($X \approx 395, Y \approx 20$) dentro do componente autônomo [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml). Cada elipse contém um [`TransformGroup`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.transformgroup) composto por um [`ScaleTransform`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform) e um [`TranslateTransform`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.translatetransform):

```xml
<Ellipse x:Name="Fumaca1" Width="20" Height="20" Fill="#65FFFFFF">
    <Ellipse.RenderTransform>
        <TransformGroup>
            <ScaleTransform x:Name="EscalaFumaca1" ScaleX="1" ScaleY="1" CenterX="10" CenterY="10"/>
            <TranslateTransform x:Name="TranslacaoFumaca1" X="395" Y="20"/>
        </TransformGroup>
    </Ellipse.RenderTransform>
</Ellipse>
```

---

## 4. O `Storyboard` e o Escalonamento Temporal

O disparador de eventos [`EventTrigger`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.eventtrigger) escuta o evento `Canvas.Loaded` para inicializar a linha do tempo automaticamente:

```xml
<Canvas.Triggers>
    <EventTrigger RoutedEvent="Canvas.Loaded">
        <BeginStoryboard>
            <Storyboard RepeatBehavior="Forever">
                <!-- Baforada 1: Duração 1.5s, início imediato -->
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca1" Storyboard.TargetProperty="Y"
                                 From="40" To="-40" Duration="0:0:1.5" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca1" Storyboard.TargetProperty="X"
                                 From="395" To="320" Duration="0:0:1.5" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca1" Storyboard.TargetProperty="ScaleX"
                                 From="0.5" To="2.2" Duration="0:0:1.5" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="EscalaFumaca1" Storyboard.TargetProperty="ScaleY"
                                 From="0.5" To="2.2" Duration="0:0:1.5" RepeatBehavior="Forever"/>
                <DoubleAnimation Storyboard.TargetName="Fumaca1" Storyboard.TargetProperty="Opacity"
                                 From="0.8" To="0.0" Duration="0:0:1.5" RepeatBehavior="Forever"/>

                <!-- Baforada 2: Duração 1.8s, defasagem BeginTime=0.5s -->
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca2" Storyboard.TargetProperty="Y"
                                 BeginTime="0:0:0.5" From="40" To="-50" Duration="0:0:1.8" RepeatBehavior="Forever"/>
                <!-- ... -->

                <!-- Baforada 3: Duração 2.0s, defasagem BeginTime=1.0s -->
                <DoubleAnimation Storyboard.TargetName="TranslacaoFumaca3" Storyboard.TargetProperty="Y"
                                 BeginTime="0:0:1.0" From="40" To="-60" Duration="0:0:2.0" RepeatBehavior="Forever"/>
                <!-- ... -->
            </Storyboard>
        </BeginStoryboard>
    </EventTrigger>
</Canvas.Triggers>
```

### O Efeito de Pulsação Escalonada (*Staggering*)
Ao configurar durações e tempos de início progressivamente defasados (`0.0s`, `0.5s`, `1.0s`), as baforadas nunca se sobrepõem perfeitamente. O resultado visual é uma emissão contínua e orgânica de vapor que acompanha a locomotiva onde quer que ela se desloque na tela.

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Visão Geral de Animação no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/animation-overview/)
- [Microsoft Learn — Visão Geral de Storyboards](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/)
- [Microsoft Learn — Classe DoubleAnimation](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.animation.doubleanimation/)
- [Microsoft Learn — Classe ScaleTransform](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.scaletransform/)
- [Microsoft Learn — Classe EventTrigger](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.eventtrigger/)
