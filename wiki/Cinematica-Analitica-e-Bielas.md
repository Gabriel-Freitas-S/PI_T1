# Cinemática Analítica do Mecanismo Biela-Manivela e Trajetória Ping-Pong

![Módulo](https://img.shields.io/badge/M%C3%B3dulo-Cinem%C3%A1tica%20Anal%C3%ADtica-007ACC?style=flat-square)
![Trajetória](https://img.shields.io/badge/Trajet%C3%B3ria-Ping--Pong%20Bidirecional-2ecc71?style=flat-square)
![Alocação](https://img.shields.io/badge/Aloca%C3%A7%C3%A3o-Zero--Alloc%20(Stack)-brightgreen?style=flat-square)
![Norma](https://img.shields.io/badge/Norma-Trabalho%20C1%20(Etapas%203%20%26%204)-blue?style=flat-square)

Neste capítulo, aborda-se a modelagem matemática, cinemática analítica e física mecânica do projeto, implementadas no motor desacoplado [`Models/LocomotivaKinematics.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Models/LocomotivaKinematics.cs) e orquestradas pelo pipeline de apresentação em [`MainWindow.xaml.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/MainWindow.xaml.cs), atendendo integralmente às normas do [Trabalho C1](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L48) (Etapa 3 - 8,0 pontos e Etapa 4 - 10,0 pontos).

---

## 1. Fundamentos do Mecanismo Biela-Manivela-Pistão (*Slider-Crank*)

O mecanismo biela-manivela (*slider-crank*) é o fundamento clássico da propulsão mecânica em locomotivas a vapor:

1. **Cilindro de Pressão**: O vapor expande-se contra as faces do pistão interno, gerando força de empuxo linear alternativo.
2. **Cruzeta Deslizante (*Crosshead*)**: Bloco maciço de ancoragem que desliza estritamente confinado entre duas barras de guia horizontais (*slide bars*). Sua função é absorver forças verticais transmitidas pela inclinação da biela, mantendo a haste do pistão imune a esforços de flexão.
3. **Biela Motriz (*Connecting Rod*)**: Barra rígida articulada entre o pino da cruzeta (que translada horizontalmente) e o pino excêntrico da manivela da roda motriz (que descreve trajetória circular).
4. **Conversão de Movimento**: Essa cadeia cinemática converte o movimento retilíneo alternativo do êmbolo na rotação contínua dos rodeiros ferroviários.

```text
[Pistão no Cilindro] <======> [Cruzeta] \
 (Translação Horizontal Linear)          \   [Biela Motriz Inclinada L=82px]
                                          \
                                           O [Pino de Manivela r=22px: Órbita Circular]
```

---

## 2. Temporização em Tempo Real via `CompositionTarget.Rendering`

Para assegurar taxa de atualização contínua e eliminar trepidações (*jitter*), o motor de simulação sincroniza-se diretamente ao ciclo de varredura vertical do monitor através da API [`CompositionTarget.Rendering`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering):

- **Frequência Dinâmica Nativa**: O evento é despachado pelo subsistema gráfico do WPF a cada quadro composto (60 Hz, 120 Hz ou 144 Hz, conforme a taxa de atualização da tela).
- **Tempo Contínuo Real**: O tempo decorrido não é incrementado por somatório de $\Delta t$ discretos (o que acumularia erros de truncamento em ponto flutuante), mas amostrado diretamente através de [`System.Diagnostics.Stopwatch`](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch), cuja resolução baseada no contador `QueryPerformanceCounter` do Windows opera na escala de nanossegundos.
- **Determinismo Estrito**: Dada qualquer estampa de tempo $t \in \mathbb{R}^+$, o método `CalcularQuadro(t, largura)` produz um estado vetorial e angular determinístico e reprodutível.

---

## 3. Dinâmica de Movimento Vai-e-Volta (*Ping-Pong* Bidirecional)

Em estrita conformidade com as diretrizes do [Trabalho C1](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L54-L56) e [Trabalho C1](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L23) (*"a locomotiva completa deve se deslocar para a direita e para a esquerda na tela, indo e voltando até alcançar os limites da janela"*), a trajetória foi estruturada com perfil contínuo bidirecional.

### 3.1 Limites de Visibilidade no Cenário

Para garantir que a composição permaneça **100% visível na tela** em todo o percurso:

- **Limite Esquerdo**: $X_{\min} = \text{MargemSegurancaLateral} = 20.0\text{ px}$.
- **Limite Direito**: $X_{\max} = \text{larguraCenario} - \text{LarguraLocomotiva} - \text{MargemSegurancaLateral}$. Com largura de referência de $1100\text{ px}$ e corpo da locomotiva de $560\text{ px}$, obtém-se $X_{\max} = 1100 - 560 - 20 = 520.0\text{ px}$.
- **Curso Total Útil**: $\Delta X_{\text{curso}} = X_{\max} - X_{\min} = 500.0\text{ px}$.

### 3.2 Perfil Ferroviário Trapezoidal com Suavização Cosseno (*Easing $C^1$*)

O movimento físico de uma composição ferroviária real possui inércia mecânica. Movimentos com aceleração instantânea infinita geram saltos de velocidade visíveis (*jerk*). Para sanar essa descontinuidade, as transições de velocidade utilizam modulação por cosseno suavizado ($C^1$ contínuo):

```text
       Velocidade v(t)
            ^
v_cruzeiro -|         /-------------------\
            |        /                     \
            |       /                       \
          0 +------/-------------------------\-------> Tempo (s)
            | Aceleração |    Cruzeiro    | Frenagem | Pausa |
            |<-- 1.5s -->|<---- 3.5s ---->|<- 1.5s ->|<-1.0s>|
            |<------------------ 7.5s ---------------------->|
```

Cada meio-ciclo ($T_{\text{half}} = 7.5\text{ s}$) divide-se em quatro regimes:

1. **Fase 1: Partida e Aceleração Suave ($t \in [0, 1.5]\text{ s}$)**  
   A velocidade escalar cresce suavemente de $0$ a $v_{\text{cruzeiro}}$ seguindo o perfil:
   $$v(t) = v_{\text{cruzeiro}} \cdot \frac{1 - \cos\left(\pi \frac{t}{T_{\text{acc}}}\right)}{2}$$
   A distância percorrida resulta da integração analítica da velocidade:
   $$s(t) = \int_0^t v(\tau) d\tau = d_{\text{acc}} \cdot \left[ u - \frac{\sin(\pi u)}{\pi} \right], \quad u = \frac{t}{T_{\text{acc}}}$$

2. **Fase 2: Deslocamento em Velocidade de Cruzeiro ($t \in [1.5, 5.0]\text{ s}$)**  
   A composição viaja com velocidade constante $v_{\text{cruzeiro}} \approx 100\text{ px/s}$:
   $$s(t) = d_{\text{acc}} + v_{\text{cruzeiro}} \cdot (t - T_{\text{acc}})$$

3. **Fase 3: Aproximação e Frenagem Progressiva ($t \in [5.0, 6.5]\text{ s}$)**  
   A velocidade decai suavemente até o repouso absoluto no limite da janela:
   $$s(t) = d_{\text{acc}} + d_{\text{cruzeiro}} + d_{\text{frenagem}} \cdot \left[ u + \frac{\sin(\pi u)}{\pi} \right], \quad u = \frac{t - (T_{\text{acc}} + T_{\text{cruzeiro}})}{T_{\text{frenagem}}}$$

4. **Fase 4: Parada de Manobra e Inversão ($t \in [6.5, 7.5]\text{ s}$)**  
   A locomotiva permanece estacionada no limite enquanto a fumaça continua sua exaustão. A transformação de escala inverte o sentido de orientação para a próxima pernada.

O ciclo completo de ida e volta totaliza $T_{\text{ciclo}} = 15.0\text{ s}$.

### 3.3 Inversão Geométrica de Sentido via `ScaleTransform`

Ao atingir a estação e inverter o sentido de marcha, a orientação visual de todos os subsistemas da locomotiva (farol, chaminé, bielas, cabine e cilindro) é espelhada horizontalmente pela matriz afim de escala:

- **Marcha para a Direita (Ida)**: `ScaleX = 1.0`
- **Marcha para a Esquerda (Volta)**: `ScaleX = -1.0`
- **Ponto Pivô de Simetria**: `CenterX = 280.0` (centro geométrico do contêiner da locomotiva com $560\text{ px}$ de largura).

Dessa forma, a locomotiva sempre trafega com o farol e o limpa-trilhos voltados para a frente do sentido de avanço, reproduzindo uma manobra ferroviária real.

---

## 4. Dinâmica de Rolamento Puro das Rodas (*Pure Rolling*)

Para que o contato entre as rodas e os trilhos obedeça às leis da mecânica de rolamento puro (sem patinagem ou arrasto por escorregamento), o ângulo acumulado de rotação $\theta$ é acoplado diretamente à distância total percorrida pela composição sobre os trilhos:

```math
\theta_{\text{graus}} = \left( \frac{s_{\text{total}}}{R_{\text{roda}}} \right) \times \left( \frac{180}{\pi} \right)
```

- **Raio Primitivo da Roda**: $R_{\text{roda}} = 40.0\text{ px}$ (diâmetro de $80\text{ px}$).
- **Monotonicidade Angular**: O ângulo acumulado $\theta$ é estritamente não-decrescente em relação à distância percorrida acumulada, assegurando rotação coerente e suave tanto na ida quanto na volta.

---

## 5. Biela de Acoplamento Horizontal (*Side Rod*): Translação Circular Pura

A locomotiva possui dois rodeiros motrizes: a **Roda 1 (Traseira)** em $X_1 = 130\text{ px}$ e a **Roda 2 (Dianteira)** em $X_2 = 270\text{ px}$, ambos com eixos em $Y = 210\text{ px}$.  
O acoplamento entre ambos é realizado pela barra de conexão horizontal (*side rod*):

```text
       (Pino 1: Roda Traseira)            (Pino 2: Roda Dianteira)
               O=================================O
              /                                   \
             /                                     \
         [RODA 1]                               [RODA 2]
```

### Cinemática do Paralelogramo Rígido Articulado

1. Ambas as rodas possuem raio de rotação idêntico ($R = 40\text{ px}$).
2. Ambas as manivelas possuem raio excêntrico idêntico ($r = 22\text{ px}$) e giram com a mesma frequência angular $\omega(t)$.
3. A distância entre os eixos das rodas é fixa e indeformável:
   $$D = 270 - 130 = \mathbf{140.0\text{ pixels}}$$

Como os vetores instantâneos de posição de ambos os pinos de manivela diferem estritamente pelo vetor constante horizontal $\vec{D} = (140, 0)$:

- O segmento que une os dois olhais permanece com módulo invariante ($140\text{ px}$) e inclinação rigorosamente nula ($\alpha = 0^\circ$) em todos os pontos da órbita de $360^\circ$.
- A biela de acoplamento não executa rotação angular sobre seu próprio centro ($\omega_{\text{biela}} = 0$); ela realiza **translação circular pura**, transladando com precisão de ponto flutuante diretamente para a coordenada instantânea $(x_1, y_1)$ do pino da Roda Traseira.

---

## 6. Cinemática da Biela Motriz e Teorema de Pitágoras

A biela motriz (*connecting rod*) conecta o pino excêntrico da Roda 2 à cruzeta deslizante do cilindro de vapor:

```text
        (pino2X, pino2Y)
               O=============================O (xCruzeta, Y=210)
             /      BIELA MOTRIZ (L = 82px)   |
           / r=22                             | Altura (ΔY)
         /                                    |
       O--------------------------------------+
    (270, 210)         Distância Horizontal (ΔX)
```

### Dedução Analítica da Posição da Cruzeta

A cruzeta está mecanicamente confinada pelas guias paralelas de aço (*slide bars*), fixando sua coordenada vertical estritamente na linha de centro dos eixos:
$$Y_{\text{cruzeta}} = 210.0\text{ px}$$

A biela motriz possui distância entre olhais indeformável $L = 82.0\text{ px}$. Pelo Teorema de Pitágoras no triângulo retângulo formado pela projeção da biela:
$$(\Delta X)^2 + (\Delta Y)^2 = L^2$$

Substituindo o cateto vertical $\Delta Y = 210.0 - \text{pino2Y}$:
$$(x_{\text{cruzeta}} - \text{pino2X})^2 + (\Delta Y)^2 = L^2$$

Isolando a coordenada horizontal do pino da cruzeta $x_{\text{cruzeta}}$:

```math
x_{\text{cruzeta}} = \text{pino2X} + \sqrt{L^2 - (\Delta Y)^2}
```

No código C# de `LocomotivaKinematics.cs`, a fórmula é avaliada com salvaguarda numérica contra truncamentos:

```csharp
double catetoVertical = CentroRodasY - pino2Y;
double termoRadical = Math.Max(0.0, (ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical));
double catetoHorizontal = Math.Sqrt(termoRadical);
double xCruzeta = pino2X + catetoHorizontal;
```

---

## 7. Orientação Angular da Biela Motriz (`Math.Atan2`)

Para que o corpo da biela motriz aponte com exatidão da manivela da Roda 2 em direção ao olhal da cruzeta, o elemento rígido deve sofrer rotação afim sobre seu ponto pivô no ângulo $\alpha$:

Utiliza-se a função trigonométrica de dois argumentos [`Math.Atan2`](https://learn.microsoft.com/pt-br/dotnet/api/system.math.atan2):

```math
\alpha = \text{atan2}(210.0 - \text{pino2Y}, \, x_{\text{cruzeta}} - \text{pino2X}) \times \left( \frac{180}{\pi} \right)
```

```csharp
double anguloBielaMotriz = Math.Atan2(CentroRodasY - pino2Y, xCruzeta - pino2X) * (180.0 / Math.PI);
```

Esse ângulo é atribuído à propriedade `BielaMotrizAngulo` da estrutura `LocomotivaFrameState`, sendo consumido pelo `RotateTransform` da biela motriz no XAML.

---

## 8. Análise Detalhada dos Componentes de Código em C# (.NET)

### 8.1 `Models/LocomotivaFrameState.cs` (DTO de Transporte Cinemático)

Estrutura imutável de dados responsável por transportar as coordenadas calculadas pelo motor físico para a camada de apresentação:

```csharp
namespace PI_T1.Models;

/// <summary>
/// Representa o estado cinemático e geométrico completo da locomotiva em um determinado quadro.
/// Definida como readonly record struct para alocação direta na Stack (Zero-Alloc no Garbage Collector).
/// </summary>
public readonly record struct LocomotivaFrameState(
    double LocomotivaX,          // Deslocamento horizontal global da locomotiva no cenário
    double AnguloRodas,          // Ângulo acumulado de rotação das rodas motrizes (graus)
    double BielaAcoplamentoX,    // Coordenada X da biela de acoplamento (side rod)
    double BielaAcoplamentoY,    // Coordenada Y da biela de acoplamento
    double CruzetaX,             // Coordenada X do bloco da cruzeta deslizante
    double CruzetaY,             // Coordenada Y da cruzeta (fixo em Y=201)
    double PinoCruzetaX,         // Coordenada X do pino de articulação da cruzeta
    double PinoCruzetaY,         // Coordenada Y do pino de articulação
    double HastePistaoX,         // Coordenada X da haste cromada do pistão
    double HastePistaoY,         // Coordenada Y da haste do pistão
    double BielaMotrizX,         // Coordenada X de ancoragem da biela motriz na Roda 2
    double BielaMotrizY,         // Coordenada Y de ancoragem da biela motriz
    double BielaMotrizAngulo,    // Ângulo horário de inclinação da biela motriz (graus)
    double EscalaDirecaoX,       // Fator de escala horizontal (+1.0 avanço à direita, -1.0 avanço à esquerda)
    string StatusDescritivo      // Descrição textual do regime dinâmico atual
);
```

- **Alocação na Stack (Zero Garbage Collection)**: Por ser declarada como `readonly record struct`, a instância é alocada diretamente na pilha (*Stack*) da thread de execução. Isso impede que milhares de instâncias temporárias poluam a memória heap a 60 ou 144 quadros por segundo, garantindo ausência total de pausas para coleta de lixo (*GC pause*).

---

### 8.2 `Models/LocomotivaKinematics.cs` (Motor Físico e Cinemático)

Classe pura em C#, completamente desacoplada de dependências de interface ou classes do subsistema `System.Windows`:

```csharp
namespace PI_T1.Models;

public class LocomotivaKinematics
{
    public const double RaioRoda = 40.0;               // Raio primitivo da roda (diâmetro 80px)
    public const double RaioManivela = 22.0;           // Raio do pino excêntrico da manivela
    public const double ComprimentoBielaMotriz = 82.0; // Distância entre centros dos olhais (L = 82px)
    public const double LarguraLocomotiva = 560.0;     // Extensão total do bloco da locomotiva com para-choques
    public const double MargemSegurancaLateral = 20.0; // Margem de segurança das bordas (100% visível)

    public const double CentroRoda1X = 130.0;          // Centro da Roda 1 (traseira) no Canvas
    public const double CentroRoda2X = 270.0;          // Centro da Roda 2 (dianteira) no Canvas
    public const double CentroRodasY = 210.0;          // Altura do eixo das rodas e da cruzeta (Y = 210px)

    public const double TempoAceleracao = 1.5;         // Aceleração suave na partida (s)
    public const double TempoCruzeiro = 3.5;           // Deslocamento em velocidade de cruzeiro (s)
    public const double TempoFrenagem = 1.5;           // Frenagem suave até repouso (s)
    public const double TempoPausaManobra = 1.0;       // Parada na estação para manobra e inversão (s)
    public const double DuracaoMeioCiclo = 7.5;        // 1.5 + 3.5 + 1.5 + 1.0 = 7.5s

    public static LocomotivaFrameState CalcularQuadro(double segundos, double larguraCenario = 1100.0)
    {
        // 1. Definição dos limites espaciais visíveis na janela
        double xMin = MargemSegurancaLateral;
        double xMax = Math.Max(xMin + 50.0, larguraCenario - LarguraLocomotiva - MargemSegurancaLateral);
        double cursoTotal = xMax - xMin;

        // 2. Tempo efetivo e velocidades do perfil trapezoidal
        double tempoEfetivoDeslocamento = (0.5 * TempoAceleracao) + TempoCruzeiro + (0.5 * TempoFrenagem);
        double velocidadeCruzeiro = cursoTotal / tempoEfetivoDeslocamento;
        double distanciaAceleracao = 0.5 * velocidadeCruzeiro * TempoAceleracao;
        double distanciaCruzeiro = velocidadeCruzeiro * TempoCruzeiro;
        double distanciaFrenagem = 0.5 * velocidadeCruzeiro * TempoFrenagem;

        long indiceMeioCiclo = (long)(segundos / DuracaoMeioCiclo);
        double tempoNoMeioCiclo = segundos % DuracaoMeioCiclo;
        bool indoParaDireita = (indiceMeioCiclo % 2) == 0;

        double distanciaNoTrecho;
        string status;

        if (tempoNoMeioCiclo <= TempoAceleracao)
        {
            // Fase 1: Partida e aceleração suave via curva cosseno
            double u = tempoNoMeioCiclo / TempoAceleracao;
            double fatorIntegral = u - (Math.Sin(Math.PI * u) / Math.PI);
            distanciaNoTrecho = distanciaAceleracao * fatorIntegral;
            status = indoParaDireita
                ? "Partida da estação oeste: acelerando suavemente para a direita..."
                : "Partida da estação leste: acelerando suavemente para a esquerda...";
        }
        else if (tempoNoMeioCiclo <= TempoAceleracao + TempoCruzeiro)
        {
            // Fase 2: Velocidade constante de cruzeiro
            double tCruzeiro = tempoNoMeioCiclo - TempoAceleracao;
            distanciaNoTrecho = distanciaAceleracao + (velocidadeCruzeiro * tCruzeiro);
            status = indoParaDireita
                ? $"Em trânsito de cruzeiro: deslocando-se para a direita (~{velocidadeCruzeiro:F0} px/s)..."
                : $"Em trânsito de cruzeiro: deslocando-se para a esquerda (~{velocidadeCruzeiro:F0} px/s)...";
        }
        else if (tempoNoMeioCiclo <= TempoAceleracao + TempoCruzeiro + TempoFrenagem)
        {
            // Fase 3: Frenagem progressiva suave
            double tFrenagem = tempoNoMeioCiclo - (TempoAceleracao + TempoCruzeiro);
            double u = tFrenagem / TempoFrenagem;
            double fatorIntegral = u + (Math.Sin(Math.PI * u) / Math.PI);
            distanciaNoTrecho = distanciaAceleracao + distanciaCruzeiro + (distanciaFrenagem * fatorIntegral);
            status = indoParaDireita
                ? "Aproximação da estação leste: frenagem suave nos limites..."
                : "Aproximação da estação oeste: frenagem suave nos limites...";
        }
        else
        {
            // Fase 4: Breve pausa de manobra para reversão
            distanciaNoTrecho = cursoTotal;
            status = indoParaDireita
                ? "Estação leste alcançada: manobra e inversão de sentido..."
                : "Estação oeste alcançada: manobra e inversão de sentido...";
        }

        double xLocoAtual = indoParaDireita ? (xMin + distanciaNoTrecho) : (xMax - distanciaNoTrecho);
        double escalaDirecaoX = indoParaDireita ? 1.0 : -1.0;

        // Rotação pura das rodas acoplada à distância percorrida
        double distanciaTotalRolada = (indiceMeioCiclo * cursoTotal) + distanciaNoTrecho;
        double theta = (distanciaTotalRolada / RaioRoda) * (180.0 / Math.PI);
        double rad = theta * (Math.PI / 180.0);

        // Pinos de manivela excêntricos
        double dxManivela = RaioManivela * Math.Cos(rad);
        double dyManivela = RaioManivela * Math.Sin(rad);

        double pino1X = CentroRoda1X + dxManivela;
        double pino1Y = CentroRodasY + dyManivela;
        double pino2X = CentroRoda2X + dxManivela;
        double pino2Y = CentroRodasY + dyManivela;

        // Resolução analítica da cruzeta (Pitágoras)
        double catetoVertical = CentroRodasY - pino2Y;
        double termoRadical = Math.Max(0.0, (ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical));
        double catetoHorizontal = Math.Sqrt(termoRadical);
        double xCruzeta = pino2X + catetoHorizontal;

        // Ângulo analítico da biela motriz
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
            BielaMotrizAngulo: anguloBielaMotriz,
            EscalaDirecaoX: escalaDirecaoX,
            StatusDescritivo: status
        );
    }
}
```

---

### 8.3 `MainWindow.xaml.cs` (Orquestração do Ciclo Gráfico)

A janela principal conecta a fonte de tempo de alta resolução ao ciclo de amostragem física:

```csharp
public MainWindow()
{
    InitializeComponent();

    Loaded += (_, _) =>
    {
        _cronometro.Restart();
        CompositionTarget.Rendering += AtualizarQuadroMecanico;
    };

    Unloaded += (_, _) =>
    {
        CompositionTarget.Rendering -= AtualizarQuadroMecanico;
        _cronometro.Stop();
    };
}

private void AtualizarQuadroMecanico(object? sender, EventArgs e)
{
    double largura = CenarioCanvas.ActualWidth > 0 ? CenarioCanvas.ActualWidth : 1100.0;
    LocomotivaFrameState estado = LocomotivaKinematics.CalcularQuadro(_cronometro.Elapsed.TotalSeconds, largura);

    // Atualiza a visão da locomotiva via ViewModel
    Locomotiva.AtualizarEstado(estado);

    // Atualiza o status descritivo no rodapé da janela principal
    _mainViewModel.StatusTrajetoria = estado.StatusDescritivo;
}
```

---

### 8.4 Separação MVVM Declarativa e Otimização Zero-Allocation

Na camada de apresentação em [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml), todas as 9 transformações afins são vinculadas declarativamente via `{Binding}`:

```xml
<Canvas.RenderTransform>
    <TransformGroup>
        <ScaleTransform ScaleX="{Binding EscalaDirecaoX}" CenterX="280"/>
        <TranslateTransform X="{Binding LocomotivaX}" Y="0"/>
    </TransformGroup>
</Canvas.RenderTransform>
```

No [`ViewModels/LocomotivaViewModel.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/ViewModels/LocomotivaViewModel.cs), todas as notificações de alteração utilizam instâncias cacheadas estáticas de `PropertyChangedEventArgs`:

```csharp
private static readonly PropertyChangedEventArgs LocomotivaXArgs = new(nameof(LocomotivaX));
private static readonly PropertyChangedEventArgs EscalaDirecaoXArgs = new(nameof(EscalaDirecaoX));
// ...
public double LocomotivaX
{
    get => _locomotivaX;
    set => SetProperty(ref _locomotivaX, value, LocomotivaXArgs);
}
```

Essa arquitetura elimina a criação recorrente de objetos na heap durante a execução em alta taxa de quadros, mantendo o Garbage Collector inerte.

---

## Referências Oficiais da Microsoft

- [Microsoft Learn — Como renderizar em um intervalo por quadro usando CompositionTarget](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/how-to-render-on-a-per-frame-interval-using-compositiontarget)
- [Microsoft Learn — Evento CompositionTarget.Rendering](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering)
- [Microsoft Learn — Classe Stopwatch (Cronômetro de Alta Resolução)](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch)
- [Microsoft Learn — Método Math.Atan2 (Cálculo de Ângulos)](https://learn.microsoft.com/pt-br/dotnet/api/system.math.atan2)
- [Microsoft Learn — Visão Geral de Transformações Afins no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/)
- [Microsoft Learn — Estruturas de Registro (Record Structs em C#)](https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/builtin-types/record#structs)
