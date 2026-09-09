# 📐 Cinemática Analítica do Mecanismo Biela-Manivela: A Física do Trem Explicada sem Complicação

Você já olhou para uma Maria-Fumaça antiga e se perguntou: *como é que um vapor saindo de uma chaleira gigante consegue fazer aquelas barras de ferro pesadas girarem rodas gigantescas sem travar nem entortar?*

Neste capítulo, explicamos de forma **mastigada e intuitiva** (com analogias do dia a dia e análise detalhada de código) como programamos o movimento mecânico perfeito da nossa locomotiva no arquivo [`Models/LocomotivaKinematics.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Models/LocomotivaKinematics.cs), atendendo a todos os requisitos de acoplamento do **[Trabalho C1 (Normas)](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L48)**.

---

## 1. O Que É o Mecanismo Biela-Manivela? (A Analogia da Bicicleta e da Máquina de Costura)

Pense em duas máquinas muito comuns:
1. **A Bicicleta**: As suas pernas sobem e descem (movimento em linha reta) nos pedais, e as correntes transformam isso no giro circular da roda.
2. **A Máquina de Costura Antiga**: O pé da costureira fica balançando uma prancha para frente e para trás, e uma haste de ferro faz a roda pesada girar.

Numa **locomotiva a vapor**, o processo é idêntico:
- O vapor entra com muita força no **cilindro** e empurra um pistão reto para frente e para trás.
- A **cruzeta** segura a haste do pistão e desliza em linha reta sobre dois trilhos de aço.
- A **biela motriz** é uma barra de metal articulada que liga a cruzeta ao pino da roda.
- Conforme a cruzeta vai e volta em linha reta, a biela empurra e puxa a manivela, fazendo a roda girar em círculos!

```
[Pistão no Cilindro] <======> [Cruzeta] \
(Vai e vem em linha reta)               \   [Biela Motriz Inclinada]
                                         \
                                          O [Pino na Roda Gira em Círculo!]
```

---

## 2. O Maestro da Animação: `CompositionTarget.Rendering` (60 Vezes por Segundo)

Para que a animação fique lisa como um filme de cinema, não podemos usar relógios comuns de computador (que engasgam quando o processador fica ocupado).

Usamos o evento especial do WPF chamado [`CompositionTarget.Rendering`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering):
- Ele funciona como o **projetor de uma sala de cinema**.
- Toda vez que a sua tela (monitor) está prestes a exibir uma nova imagem (normalmente 60 vezes por segundo), o WPF chama nosso método `AtualizarQuadroMecanico`.
- Nós medimos o tempo exato com um cronômetro de precisão atômica ([`System.Diagnostics.Stopwatch`](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch)), calculamos a nova posição de todas as peças e atualizamos a tela em milissegundos.

---

## 3. O Trem Andando sem Fim: O Circuito do "Túnel Infinito"

Para o trem se movimentar continuamente sem parar nunca:
1. **Largada**: Ele começa completamente escondido fora da tela, à esquerda ($X = -560\text{ px}$).
2. **Travessia**: Ele cruza a janela inteira da esquerda para a direita, soltando fumaça e girando as rodas.
3. **Saída e Reentrada**: Assim que o último milímetro da traseira do trem cruza a borda direita da janela, ele reaparece instantaneamente na esquerda, como se estivesse saindo de um túnel ferroviário que dá a volta no mundo!

Como calculamos isso a cada fração de segundo?
- Sabendo o tempo $t$ e a velocidade $v$, sabemos a distância percorrida:
  $$s(t) = v \times t$$
- As rodas giram sempre continuamente para a frente, sem nenhum tranco ou salto quando o trem reentra na tela.

---

## 4. Rolamento Puro: Como Fazer a Roda Girar sem Derrapar?

Imagine que você coloca uma moeda de R$ 1 em pé sobre a mesa e faz ela dar uma volta completa:
- A distância que a moeda percorreu na mesa é rigorosamente igual ao comprimento da borda redonda dela ($2 \times \pi \times \text{Raio}$).
- Se o trem andar para a frente mas a roda girar devagar demais, parece que o trem está patinando no sabão. Se girar rápido demais, parece que está cantando pneu no asfalto!

Para que o contato com o trilho seja **100% realista (rolamento puro)**, conectamos o ângulo de rotação da roda $\theta$ ao deslocamento horizontal $\Delta X$:

```math
\Delta\theta_{\text{graus}} = \left(\frac{\Delta X}{R_{\text{roda}}}\right) \times \left(\frac{180}{\pi}\right)
```

Como o raio da nossa roda é $R = 40\text{ px}$:
- Cada pixel que a locomotiva avança para a direita faz as duas rodas girarem no sentido horário no ângulo exato.

---

## 5. A Biela de Acoplamento: Por Que a Barra entre as Rodas Não Entorta?

A locomotiva possui duas rodas de tração: a **Roda 1 (Traseira)** e a **Roda 2 (Dianteira)**.  
Entre elas, existe uma barra horizontal de aço chamada **Biela de Acoplamento (*Side Rod*)**:

```
        (Pino 1)                      (Pino 2)
           O=============================O
         /                                 \
       /                                     \
    [RODA 1]                              [RODA 2]
```

### A Analogia de Dois Amigos Carregando um Sofá
Pense em duas pessoas com a mesma altura, caminhando lado a lado com os mesmos passos:
- A distância entre as mãos dos dois nunca muda.
- O sofá que eles carregam permanece sempre nivelado na horizontal!

Com as nossas rodas acontece a mesma mágica da geometria:
1. As duas rodas têm o mesmo tamanho ($80\text{ px}$).
2. Os dois pinos ficam na mesma distância de manivela ($r = 22\text{ px}$).
3. A distância entre os centros das rodas é fixa: $270 - 130 = \mathbf{140\text{ pixels}}$.

Portanto, **a distância entre o Pino 1 e o Pino 2 é SEMPRE 140 pixels**, e a barra está **SEMPRE perfeitamente deitada (horizontal)**!  
Ela não precisa girar sobre si mesma: ela apenas passeia em círculos junto com as manivelas, mantendo as duas rodas conectadas com força total.

---

## 6. A Biela Motriz e o Teorema de Pitágoras (A Analogia da Escada na Parede)

Aqui entra a parte mais engenhosa da matemática do projeto: como calcular onde a **cruzeta** e o **pistão** estão a cada milissegundo?

```
       (pino2X, pino2Y)
              O============================O (xCruzeta, Y=210)
            /        BIELA MOTRIZ (L=82)   |
          / r=22                           | Altura (ΔY)
        /                                  |
      O------------------------------------+
   (270, 210)       Distância Horizontal (ΔX)
```

### A Analogia da Escada Encostada na Parede
Imagine que você tem uma escada de ferro de comprimento fixo de $82\text{ cm}$ ($L = 82$):
- A ponta de trás da escada está apoiada no pino da Roda 2, que fica subindo e descendo conforme a roda gira.
- A ponta da frente da escada está presa dentro de uma canaleta horizontal no chão (a altura $Y$ nunca muda, é sempre $210\text{ px}$).
- Conforme o pino da roda sobe, a escada fica inclinada e puxa a ponta da frente para trás.
- Quando o pino da roda fica reto no meio, a escada deita e empurra a ponta da frente para o mais longe possível!

### Usando o Teorema de Pitágoras da Escola ($a^2 + b^2 = c^2$):
A biela forma a hipotenusa de um triângulo retângulo com a altura e a distância horizontal:
$$(\Delta X)^2 + (\Delta Y)^2 = L^2$$

Como sabemos o comprimento da biela ($L = 82$) e sabemos a altura do pino ($\Delta Y = 210 - pino2Y$), basta isolar a distância horizontal:

```math
x_{\text{cruzeta}} = pino2X + \sqrt{L^2 - (\Delta Y)^2}
```

No código C#, calculamos isso em apenas 3 linhas:
```csharp
double catetoVertical = CentroRodasY - pino2Y;
double catetoHorizontal = Math.Sqrt((ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical));
double xCruzeta = pino2X + catetoHorizontal;
```
Pronto! Sem adivinhações nem números inventados: a cruzeta desliza no trilho com **precisão matemática absoluta**!

---

## 7. A Bússola da Biela: Calculando a Inclinação com `Math.Atan2`

Agora só falta um detalhe: a barra da biela motriz precisa se inclinar para que sua ponta da frente encaixe perfeitamente no pino da cruzeta.

Para descobrir quantos graus inclinar a barra a cada momento, usamos a função matemática [`Math.Atan2`](https://learn.microsoft.com/pt-br/dotnet/api/system.math.atan2):
- Ela funciona como uma bússola mágica: você informa a diferença de altura ($\Delta Y$) e a distância ($\Delta X$), e ela devolve o ângulo exato em graus!

```csharp
double anguloBielaMotriz = Math.Atan2(CentroRodasY - pino2Y, xCruzeta - pino2X) * (180.0 / Math.PI);
RotacaoBielaMotriz.Angle = anguloBielaMotriz;
```

Com isso, a biela motriz fica perfeitamente esticada entre a roda e a cruzeta em todos os 60 quadros por segundo, sem nunca escapar nem um décimo de milímetro!

---

## 8. 📖 Análise Linha a Linha do Código C#

Vamos agora inspecionar cada um dos 4 arquivos de código C# que formam esse mecanismo:

### 8.1 `Models/LocomotivaFrameState.cs` (O Envelope com os Dados do Quadro)

Este arquivo é um DTO (*Data Transfer Object*). Ele funciona como uma carta que o motor de física preenche e envia para a tela:

```csharp
namespace PI_T1.Models;

public readonly record struct LocomotivaFrameState(
    double LocomotivaX,          //* Onde o trem inteiro está na tela (horizontal)
    double AnguloRodas,          //* Quantos graus as rodas giraram (ex: 720° = 2 voltas)
    double BielaAcoplamentoX,    //* Posição X da barra que une as duas rodas
    double BielaAcoplamentoY,    //* Posição Y da barra que une as duas rodas
    double CruzetaX,             //* Posição X do bloco de ferro que guia o pistão
    double CruzetaY,             //* Posição Y da cruzeta (fixo em 201px)
    double PinoCruzetaX,         //* Posição X do pino de articulação da cruzeta
    double PinoCruzetaY,         //* Posição Y do pino de articulação
    double HastePistaoX,         //* Posição X da barra prateada que entra no cilindro
    double HastePistaoY,         //* Posição Y da haste prateada
    double BielaMotrizX,         //* Onde a biela inclinada se apoia na roda dianteira
    double BielaMotrizY,         //* Posição Y do apoio na roda dianteira
    double BielaMotrizAngulo     //* Inclinação da biela motriz em graus
);
```
- **Por que `readonly record struct`?**  
  Em C#, classes normais criam lixo na memória (*Garbage Collector*) quando são criadas 60 vezes por segundo, o que causaria pequenos engasgos na tela. Uma `readonly record struct` é alocada diretamente na pilha rápida de memória (*Stack*), com **custo de memória ZERO**!

---

### 8.2 `Models/LocomotivaKinematics.cs` (O Motor de Física Pura)

Este é o cérebro matemático. Ele não sabe o que é cor, botão ou desenho: ele só calcula números puros!

#### 1. Constantes Mecânicas:
```csharp
public const double RaioRoda = 40.0;               //* Raio primitivo da roda (diâmetro 80px)
public const double RaioManivela = 22.0;           //* Distância do centro ao pino excêntrico
public const double ComprimentoBielaMotriz = 82.0; //* Distância fixa entre olhais da biela (L=82px)
public const double LarguraLocomotiva = 560.0;     //* Tamanho do trem para saber quando sumiu da tela
public const double CentroRoda1X = 130.0;          //* Posição do eixo da Roda Traseira
public const double CentroRoda2X = 270.0;          //* Posição do eixo da Roda Dianteira
public const double CentroRodasY = 210.0;          //* Altura do eixo e da canaleta do pistão
public const double DuracaoLoopContinuo = 11.0;     //* Duração da travessia completa (em segundos)
```

#### 2. O Método `CalcularQuadro`:
```csharp
public LocomotivaFrameState CalcularQuadro(double segundos, double larguraCenario = 1100.0)
{
    // 1. Largada na extrema esquerda oculta (X = -560)
    double xEntrada = -LarguraLocomotiva;
    double xSaida = larguraCenario > 0 ? larguraCenario : 1100.0;
    double distanciaTotal = xSaida - xEntrada;
    double velocidade = distanciaTotal / DuracaoLoopContinuo;

    // 2. Progresso do trem no ciclo contínuo
    double distanciaPercorrida = velocidade * segundos;
    double progressoNoCiclo = distanciaPercorrida % distanciaTotal;
    double xLocoAtual = xEntrada + progressoNoCiclo;

    // 3. Giro contínuo da roda sem derrapar (s = R * theta)
    double theta = (distanciaPercorrida / RaioRoda) * (180.0 / Math.PI);
    double rad = theta * (Math.PI / 180.0);

    // 4. Posição circular dos pinos das manivelas
    double dxManivela = RaioManivela * Math.Cos(rad);
    double dyManivela = RaioManivela * Math.Sin(rad);

    double pino1X = CentroRoda1X + dxManivela;
    double pino1Y = CentroRodasY + dyManivela;
    double pino2X = CentroRoda2X + dxManivela;
    double pino2Y = CentroRodasY + dyManivela;

    // 5. Teorema de Pitágoras para achar a cruzeta
    double catetoVertical = CentroRodasY - pino2Y;
    double termoRadical = Math.Max(0.0, (ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical));
    double catetoHorizontal = Math.Sqrt(termoRadical);
    double xCruzeta = pino2X + catetoHorizontal;

    // 6. Inclinação da biela motriz em graus
    double anguloBielaMotriz = Math.Atan2(CentroRodasY - pino2Y, xCruzeta - pino2X) * (180.0 / Math.PI);

    return new LocomotivaFrameState(
        LocomotivaX: xLocoAtual,
        AnguloRodas: theta,
        BielaAcoplamentoX: pino1X,
        BielaAcoplamentoY: pino1Y,
        CruzetaX: xCruzeta - 9.0,
        CruzetaY: CentroRodasY - 9.0,
        PinoCruzetaX: xCruzeta - 4.0,
        PinoCruzetaY: CentroRodasY - 4.0,
        HastePistaoX: xCruzeta,
        HastePistaoY: CentroRodasY - 3.0,
        BielaMotrizX: pino2X,
        BielaMotrizY: pino2Y,
        BielaMotrizAngulo: anguloBielaMotriz
    );
}
```
- Repare no `Math.Max(0.0, ...)` na linha 27: se por um erro minúsculo de arredondamento o número ficasse negativo (tipo `-0.0000001`), a raiz quadrada daria erro (`NaN`). O `Math.Max` é nosso cinto de segurança numérico!

---

### 8.3 `MainWindow.xaml.cs` (O Maestro)

Aqui conectamos o cronômetro com a taxa de atualização do monitor:

```csharp
public MainWindow()
{
    InitializeComponent();

    // Quando a janela aparece na tela:
    Loaded += (_, _) =>
    {
        _cronometro.Restart();
        CompositionTarget.Rendering += AtualizarQuadroMecanico; // Começa a tocar a orquestra!
    };

    // Quando o usuário fecha a janela:
    Unloaded += (_, _) =>
    {
        CompositionTarget.Rendering -= AtualizarQuadroMecanico; // Desliga tudo para não gastar bateria
        _cronometro.Stop();
    };
}

private void AtualizarQuadroMecanico(object? sender, EventArgs e)
{
    double largura = CenarioCanvas.ActualWidth > 0 ? CenarioCanvas.ActualWidth : 1100.0;
    
    // 1. Pede ao motor de física para calcular o instante atual
    LocomotivaFrameState estado = _kinematics.CalcularQuadro(_cronometro.Elapsed.TotalSeconds, largura);
    
    // 2. Entrega o resultado para a locomotiva se mover
    Locomotiva.AtualizarEstado(estado);
}
```

---

### 8.4 `Controls/LocomotivaControl.xaml.cs` (O Coreógrafo da Locomotiva)

Por fim, este arquivo pega os números que o maestro mandou e aplica nas 9 peças correspondentes:

```csharp
public void AtualizarEstado(in LocomotivaFrameState estado)
{
    // 1. Gira as duas rodas
    RotacaoRoda1.Angle = estado.AnguloRodas;
    RotacaoRoda2.Angle = estado.AnguloRodas;

    // 2. Translada a barra que une as rodas
    TranslacaoBielaAcoplamento.X = estado.BielaAcoplamentoX;
    TranslacaoBielaAcoplamento.Y = estado.BielaAcoplamentoY;

    // 3. Move a cruzeta e a haste do pistão
    TranslacaoCruzeta.X = estado.CruzetaX;
    TranslacaoCruzeta.Y = estado.CruzetaY;
    TranslacaoPinoCruzeta.X = estado.PinoCruzetaX;
    TranslacaoPinoCruzeta.Y = estado.PinoCruzetaY;
    TranslacaoHastePistao.X = estado.HastePistaoX;
    TranslacaoHastePistao.Y = estado.HastePistaoY;

    // 4. Move e inclina a biela motriz
    TranslacaoBielaMotriz.X = estado.BielaMotrizX;
    TranslacaoBielaMotriz.Y = estado.BielaMotrizY;
    RotacaoBielaMotriz.Angle = estado.BielaMotrizAngulo;

    // 5. Move a locomotiva inteira pelo cenário
    TranslacaoLocomotiva.X = estado.LocomotivaX;
}
```
- Veja como o código é limpo e direto: cada propriedade do estado vai direto para o seu respectivo `TranslateTransform` ou `RotateTransform`. Simples, rápido e 100% livre de bugs!

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Como renderizar em um intervalo por quadro usando CompositionTarget](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/how-to-render-on-a-per-frame-interval-using-compositiontarget)
- [Microsoft Learn — Evento CompositionTarget.Rendering](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering)
- [Microsoft Learn — Classe Stopwatch (Cronômetro de Alta Resolução)](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch)
- [Microsoft Learn — Método Math.Atan2 (Cálculo de Ângulos)](https://learn.microsoft.com/pt-br/dotnet/api/system.math.atan2)
- [Microsoft Learn — Visão Geral de Transformações Afins no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/)
- [Microsoft Learn — Estruturas de Registro (Record Structs em C#)](https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/builtin-types/record#structs)
