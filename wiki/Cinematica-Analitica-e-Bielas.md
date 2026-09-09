# 📐 Cinemática Analítica do Mecanismo Biela-Manivela

Neste capítulo, aborda-se a modelagem cinemática analítica e a física mecânica do projeto, implementadas no motor desacoplado [`Models/LocomotivaKinematics.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Models/LocomotivaKinematics.cs) e orquestradas em [`MainWindow.xaml.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/MainWindow.xaml.cs), atendendo integralmente às normas do **[Trabalho C1 (Normas)](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L48)** (Etapa 3 - 8,0 pontos e Etapa 4 - 10,0 pontos).

---

## 1. Fundamentos do Mecanismo Biela-Manivela-Pistão (*Slider-Crank*)

O mecanismo biela-manivela (*slider-crank*) é o fundamento mecânico clássico da propulsão ferroviária a vapor:
- O vapor sob alta pressão expande-se dentro do **cilindro**, forçando o pistão a executar um movimento linear alternativo horizontal.
- A **cruzeta** (*crosshead*) atua como guia rígida, ancorando a extremidade da haste e deslizando estritamente entre duas barras paralelas de aço (*slide bars*).
- A **biela motriz** (*connecting rod*) conecta articuladamente a cruzeta móvel ao pino de manivela da roda motriz.
- Esse acoplamento converte a translação retilínea do êmbolo na rotação contínua dos eixos do trem.

```
[Pistão no Cilindro] <======> [Cruzeta] \
 (Movimento Linear Alternativo)          \   [Biela Motriz Inclinada]
                                          \
                                           O [Pino de Manivela: Rotação Circular]
```

---

## 2. Temporização em Tempo Real via `CompositionTarget.Rendering`

Para garantir taxas de atualização estáveis e renderização suave sem *jitter* ou engasgos, a temporização da aplicação é sincronizada diretamente com o pipeline gráfico do monitor:

O projeto utiliza o manipulador nativo [`CompositionTarget.Rendering`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering):
- O evento é disparado pelo WPF a cada intervalo de quadro de tela (60 Hz, 120 Hz ou 144 Hz conforme o hardware do usuário), imediatamente antes da composição na GPU.
- A medição de tempo decorrido é realizada com precisão de nanossegundos através da classe [`System.Diagnostics.Stopwatch`](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch).
- A classe `MainWindow` obtém o estado físico calculado de `LocomotivaKinematics` e delega as atualizações diretamente ao controle visual.

---

## 3. Dinâmica de Travessia em Circuito Contínuo (Túnel Ferroviário)

O motor físico opera exclusivamente no modo de **Loop Contínuo em Circuito Fechado**:
1. **Ponto de Partida**: O trem inicia sua trajetória completamente fora da área de visualização à esquerda ($X_{\text{start}} = -560\text{ px}$, correspondendo à largura total do corpo).
2. **Travessia Completa**: Percorre o cenário em velocidade escalar uniforme sobre os trilhos.
3. **Ponto de Saída e Reentrada**: Assim que a extremidade traseira do chassi cruza a margem direita da janela ($X_{\text{end}} = \text{larguraCenario}$), a locomotiva reaparece na extrema esquerda, simulando uma travessia ininterrupta de túnel.

A posição horizontal instantânea $x_{\text{loco}}(t)$ é regida pela relação linear:
$$s(t) = v \times t$$
$$x_{\text{loco}}(t) = X_{\text{start}} + (s(t) \pmod{\text{distanciaTotal}})$$

As rodas mantêm rotação contínua progressiva sem qualquer descontinuidade de fase ou inversão no instante da reentrada.

---

## 4. Dinâmica de Rolamento Puro das Rodas (*Pure Rolling*)

Para assegurar aderência mecânica realista entre as rodas e os trilhos (eliminando deslizamentos ou patinagem), o deslocamento linear horizontal $\Delta X$ é acoplado rigorosamente à velocidade angular $\Delta\theta$:

```math
\Delta\theta_{\text{graus}} = \left(\frac{\Delta X}{R_{\text{roda}}}\right) \times \left(\frac{180}{\pi}\right)
```

Com o raio primitivo da roda calibrado em $R_{\text{roda}} = 40\text{ px}$:
- Cada incremento infinitesimal de deslocamento linear resulta na rotação horária precisa da roda, estabelecendo tangência contínua com a superfície do trilho em $Y = 405\text{ px}$.

---

## 5. Biela de Acoplamento Horizontal (*Side Rod*): Translação Circular Pura

A locomotiva possui dois conjuntos de eixos motrizes: a **Roda 1 (Traseira)** e a **Roda 2 (Dianteira)**.  
A sincronização do torque entre ambos é realizada pela **Biela de Acoplamento (*Side Rod*)**:

```
        (Pino 1)                      (Pino 2)
           O=============================O
         /                                 \
       /                                     \
    [RODA 1]                              [RODA 2]
```

### Cinemática do Paralelogramo Articulado:
1. Ambas as rodas possuem raio primitivo idêntico ($R = 40\text{ px}$).
2. Ambas as manivelas possuem excentricidade idêntica ($r = 22\text{ px}$) e giram na mesma velocidade angular $\omega(t)$.
3. A distância entre os eixos das rodas é fixa e invariante: $D = 270 - 130 = \mathbf{140\text{ pixels}}$.

Como os vetores posição de ambos os pinos diferem unicamente por uma constante horizontal fixa $\vec{D} = (140, 0)$:
- O segmento que une os dois pinos mantém módulo constante ($140\text{ px}$) e inclinação estritamente horizontal em todos os pontos da órbita de $360^\circ$.
- A biela de acoplamento não necessita de rotação sobre seu próprio eixo local ($\omega_{\text{biela}} = 0$); ela executa **translação circular pura**, transladando com precisão de sub-pixel para a coordenada instantânea do Pino 1.

---

## 6. Cinemática da Biela Motriz e Teorema de Pitágoras

A biela motriz conecta o pino da Roda 2 à cruzeta deslizante do pistão:

```
       (pino2X, pino2Y)
              O============================O (xCruzeta, Y=210)
            /        BIELA MOTRIZ (L=82)   |
          / r=22                           | Altura (ΔY)
        /                                  |
      O------------------------------------+
   (270, 210)       Distância Horizontal (ΔX)
```

### Formulação Analítica da Posição da Cruzeta:
A cruzeta está mecanicamente restrita a deslizar sobre as guias lineares horizontais, fixando sua coordenada vertical estritamente em:
$$Y_{\text{cruzeta}} = 210\text{ px}$$

A biela motriz possui comprimento indeformável entre centros de olhais $L = 82\text{ px}$. Aplicando o Teorema de Pitágoras no triângulo retângulo formado pelo pino excêntrico e a cruzeta:
$$(\Delta X)^2 + (\Delta Y)^2 = L^2$$

Substituindo a diferença vertical $\Delta Y = 210 - pino2Y$:
$$(x_{\text{cruzeta}} - pino2X)^2 + (\Delta Y)^2 = L^2$$

Isolando a coordenada horizontal $x_{\text{cruzeta}}$:
```math
x_{\text{cruzeta}} = pino2X + \sqrt{L^2 - (\Delta Y)^2}
```

No código C#, essa dedução analítica é executada com três operações fundamentais:
```csharp
double catetoVertical = CentroRodasY - pino2Y;
double catetoHorizontal = Math.Sqrt(Math.Max(0.0, (ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical)));
double xCruzeta = pino2X + catetoHorizontal;
```
Essa formulação garante precisão matemática absoluta na posição da cruzeta em 100% dos quadros.

---

## 7. Orientação Angular da Biela Motriz (`Math.Atan2`)

Para que o olhal dianteiro da biela motriz coincida rigorosamente com o pino da cruzeta a cada instante, o braço rígido deve ser rotacionado sobre seu olhal traseiro no ângulo analítico $\alpha$:

Utiliza-se a função trigonométrica [`Math.Atan2`](https://learn.microsoft.com/pt-br/dotnet/api/system.math.atan2):
$$\alpha = \text{atan2}(210 - pino2Y, x_{\text{cruzeta}} - pino2X) \times \left(\frac{180}{\pi}\right)$$

```csharp
double anguloBielaMotriz = Math.Atan2(CentroRodasY - pino2Y, xCruzeta - pino2X) * (180.0 / Math.PI);
RotacaoBielaMotriz.Angle = anguloBielaMotriz;
```

Essa operação assegura o fechamento geométrico perfeito da cadeia cinemática sem folgas ou desvios perceptíveis.

---

## 8. 📖 Análise Linha a Linha do Código C#

Abaixo é detalhada a implementação dos quatro componentes de software que estruturam esse sistema:

### 8.1 `Models/LocomotivaFrameState.cs` (DTO de Transporte Cinemático)

Estrutura imutável de dados responsável por transportar as coordenadas calculadas pelo motor físico para a camada de apresentação:

```csharp
namespace PI_T1.Models;

public readonly record struct LocomotivaFrameState(
    double LocomotivaX,          //* Deslocamento horizontal global da locomotiva no cenário
    double AnguloRodas,          //* Ângulo acumulado de rotação das rodas motrizes (graus)
    double BielaAcoplamentoX,    //* Coordenada X da biela de acoplamento (side rod)
    double BielaAcoplamentoY,    //* Coordenada Y da biela de acoplamento
    double CruzetaX,             //* Coordenada X do bloco da cruzeta deslizante
    double CruzetaY,             //* Coordenada Y da cruzeta (fixo em Y=201)
    double PinoCruzetaX,         //* Coordenada X do pino de articulação da cruzeta
    double PinoCruzetaY,         //* Coordenada Y do pino de articulação
    double HastePistaoX,         //* Coordenada X da haste cromada do pistão
    double HastePistaoY,         //* Coordenada Y da haste do pistão
    double BielaMotrizX,         //* Coordenada X de ancoragem da biela motriz na Roda 2
    double BielaMotrizY,         //* Coordenada Y de ancoragem da biela motriz
    double BielaMotrizAngulo     //* Ângulo horário de inclinação da biela motriz (graus)
);
```
- **Alocação Eficiente em Memória**: Por ser definida como `readonly record struct`, a instância é alocada diretamente na pilha (*Stack*) de execução do .NET. Isso elimina completamente a pressão sobre o coletor de lixo (*Garbage Collector*), garantindo taxa constante de 60 quadros por segundo sem pausas por desalocação.

---

### 8.2 `Models/LocomotivaKinematics.cs` (Motor Físico e Cinemático)

Módulo desacoplado de qualquer dependência visual ou controle de interface do WPF:

#### 1. Constantes Físicas e Geométricas:
```csharp
public const double RaioRoda = 40.0;               //* Raio primitivo das rodas motrizes
public const double RaioManivela = 22.0;           //* Excentricidade radial dos pinos de tração
public const double ComprimentoBielaMotriz = 82.0; //* Distância fixa entre eixos dos olhais (L)
public const double LarguraLocomotiva = 560.0;     //* Extensão total da composição com buffers
public const double CentroRoda1X = 130.0;          //* Posição do eixo da Roda Traseira no Canvas
public const double CentroRoda2X = 270.0;          //* Posição do eixo da Roda Dianteira no Canvas
public const double CentroRodasY = 210.0;          //* Ordenada comum dos eixos e da guia da cruzeta
public const double DuracaoLoopContinuo = 11.0;     //* Tempo de travessia em segundos (~150 px/s)
```

#### 2. Implementação de `CalcularQuadro`:
```csharp
public LocomotivaFrameState CalcularQuadro(double segundos, double larguraCenario = 1100.0)
{
    // 1. Definição dos limites de entrada e saída
    double xEntrada = -LarguraLocomotiva;
    double xSaida = larguraCenario > 0 ? larguraCenario : 1100.0;
    double distanciaTotal = xSaida - xEntrada;
    double velocidade = distanciaTotal / DuracaoLoopContinuo;

    // 2. Cálculo do progresso no ciclo contínuo
    double distanciaPercorrida = velocidade * segundos;
    double progressoNoCiclo = distanciaPercorrida % distanciaTotal;
    double xLocoAtual = xEntrada + progressoNoCiclo;

    // 3. Rotação puramente monotônica sem escorregamento
    double theta = (distanciaPercorrida / RaioRoda) * (180.0 / Math.PI);
    double rad = theta * (Math.PI / 180.0);

    // 4. Posições instantâneas dos pinos de manivela
    double dxManivela = RaioManivela * Math.Cos(rad);
    double dyManivela = RaioManivela * Math.Sin(rad);

    double pino1X = CentroRoda1X + dxManivela;
    double pino1Y = CentroRodasY + dyManivela;
    double pino2X = CentroRoda2X + dxManivela;
    double pino2Y = CentroRodasY + dyManivela;

    // 5. Equacionamento da cruzeta via Pitágoras
    double catetoVertical = CentroRodasY - pino2Y;
    double termoRadical = Math.Max(0.0, (ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical));
    double catetoHorizontal = Math.Sqrt(termoRadical);
    double xCruzeta = pino2X + catetoHorizontal;

    // 6. Determinação angular da biela motriz
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
- **Salvaguarda Numérica**: A expressão `Math.Max(0.0, ...)` atua como salvaguarda matemática contra eventuais imprecisões infinitesimais de ponto flutuante, prevenindo a ocorrência de indeterminações numéricas (`NaN`) na radiciação.

---

### 8.3 `MainWindow.xaml.cs` (Orquestração do Pipeline Gráfico)

A janela principal gerencia o ciclo de vida da execução e a ponte entre o temporizador e a visão:

```csharp
public MainWindow()
{
    InitializeComponent();

    // Associação ao manipulador de renderização no carregamento da janela
    Loaded += (_, _) =>
    {
        _cronometro.Restart();
        CompositionTarget.Rendering += AtualizarQuadroMecanico;
    };

    // Desvinculação no fechamento para evitar vazamentos de memória
    Unloaded += (_, _) =>
    {
        CompositionTarget.Rendering -= AtualizarQuadroMecanico;
        _cronometro.Stop();
    };
}

private void AtualizarQuadroMecanico(object? sender, EventArgs e)
{
    double largura = CenarioCanvas.ActualWidth > 0 ? CenarioCanvas.ActualWidth : 1100.0;
    
    // Consulta o motor físico e delega o estado calculado ao componente visual
    LocomotivaFrameState estado = _kinematics.CalcularQuadro(_cronometro.Elapsed.TotalSeconds, largura);
    Locomotiva.AtualizarEstado(estado);
}
```

---

### 8.4 `Controls/LocomotivaControl.xaml.cs` (Aplicação das Transformações Afins)

Encapsula os elementos visuais internos da composição, atualizando suas matrizes de transformação afim:

```csharp
public void AtualizarEstado(in LocomotivaFrameState estado)
{
    // 1. Rotação das rodas sob o chassi
    RotacaoRoda1.Angle = estado.AnguloRodas;
    RotacaoRoda2.Angle = estado.AnguloRodas;

    // 2. Translação da biela de acoplamento horizontal
    TranslacaoBielaAcoplamento.X = estado.BielaAcoplamentoX;
    TranslacaoBielaAcoplamento.Y = estado.BielaAcoplamentoY;

    // 3. Posicionamento da cruzeta e haste do pistão
    TranslacaoCruzeta.X = estado.CruzetaX;
    TranslacaoCruzeta.Y = estado.CruzetaY;
    TranslacaoPinoCruzeta.X = estado.PinoCruzetaX;
    TranslacaoPinoCruzeta.Y = estado.PinoCruzetaY;
    TranslacaoHastePistao.X = estado.HastePistaoX;
    TranslacaoHastePistao.Y = estado.HastePistaoY;

    // 4. Ancoragem e rotação da biela motriz
    TranslacaoBielaMotriz.X = estado.BielaMotrizX;
    TranslacaoBielaMotriz.Y = estado.BielaMotrizY;
    RotacaoBielaMotriz.Angle = estado.BielaMotrizAngulo;

    // 5. Translação global do Canvas da locomotiva pelo cenário
    TranslacaoLocomotiva.X = estado.LocomotivaX;
}
```

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Como renderizar em um intervalo por quadro usando CompositionTarget](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/how-to-render-on-a-per-frame-interval-using-compositiontarget)
- [Microsoft Learn — Evento CompositionTarget.Rendering](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering)
- [Microsoft Learn — Classe Stopwatch (Cronômetro de Alta Resolução)](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch)
- [Microsoft Learn — Método Math.Atan2 (Cálculo de Ângulos)](https://learn.microsoft.com/pt-br/dotnet/api/system.math.atan2)
- [Microsoft Learn — Visão Geral de Transformações Afins no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/)
- [Microsoft Learn — Estruturas de Registro (Record Structs em C#)](https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/builtin-types/record#structs)
