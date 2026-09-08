# 📐 Cinemática Analítica do Mecanismo Biela-Manivela

Neste capítulo, explora-se a modelagem matemática exata implementada no arquivo [`MainWindow.xaml.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/MainWindow.xaml.cs), que atende aos requisitos de sincronismo mecânico do **[Trabalho C1 (Normas)](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L48)** (Etapa 3 - 8,0 pontos e Etapa 4 - 10,0 pontos).

---

## 1. O Loop de Renderização: `CompositionTarget.Rendering`

Ao contrário de abordagens rudimentares baseadas em `DispatcherTimer` ou `System.Threading.Thread.Sleep` (que introduzem *jitter*, engasgos e descompasso com a taxa de atualização do monitor), o projeto conecta o cálculo físico diretamente ao manipulador de eventos de alta prioridade do WPF:

```csharp
CompositionTarget.Rendering += AtualizarQuadroMecanico;
```

Conforme especificado no **Microsoft Learn**, o evento [`CompositionTarget.Rendering`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering) é acionado pelo subsistema gráfico do WPF **uma vez por quadro** (tipicamente a 60 Hz, 120 Hz ou 144 Hz conforme o monitor do usuário), logo após a passagem de layout e imediatamente antes da composição final da árvore visual pela GPU.

### Temporização de Alta Resolução (`Stopwatch`)
Para evitar qualquer defasagem acumulada por variações de *frame-rate*, o tempo decorrido $t$ é mensurado com o cronômetro nativo do hardware ([`System.Diagnostics.Stopwatch`](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch)):
```csharp
double segundos = _cronometro.Elapsed.TotalSeconds;
```

---

## 2. Etapa 1: Movimento Harmônico de Vai-e-Volta Suave

Para atender ao critério de movimento contínuo entre os limites da janela sem reversões bruscas que quebrem a ilusão mecânica, utiliza-se uma função de modulação harmônica cosenoidal com período `T = 14 s`:

```math
\tau = \frac{t \pmod T}{T} \in [0, 1)
```

O progresso normalizado suave `P(\tau)` é obtido por:

```math
P(\tau) = \frac{1 - \cos(2\pi \tau)}{2} \in [0, 1]
```

A posição horizontal instantânea da locomotiva `x_loco(t)` é dada por:

```math
x_{\text{loco}}(t) = X_{\text{min}} + (X_{\text{max}} - X_{\text{min}}) \cdot P(\tau)
```

Onde `X_min = -100 px` e `X_max = 540 px`.

### Propriedades Físicas da Função Harmônica:
- **Velocidade nos Extremos**: A derivada temporal da posição é:

```math
v(t) = \frac{dx}{dt} = \frac{(X_{\text{max}} - X_{\text{min}})\pi}{T} \sin(2\pi\tau)
```

Nos instantes `\tau = 0` (extremo esquerdo) e `\tau = 0.5` (extremo direito), `\sin(2\pi\tau) = 0 \implies v = 0`. A locomotiva **desacelera suavemente até parar**, inverte a marcha e reacelera progressivamente, eliminando trancos.

---

## 3. Etapa 2: Dinâmica de Rolamento Puro das Rodas (*Pure Rolling*)

Para que as rodas não pareçam "patinar" no gelo sobre os trilhos, a rotação angular deve ser matematicamente acoplada ao deslocamento linear da locomotiva através da lei fundamental da cinemática circular:

```math
s = R \cdot \Delta\theta_{\text{rad}} \implies \Delta\theta_{\text{rad}} = \frac{\Delta X}{R_{\text{roda}}}
```

Convertendo para graus e considerando o raio primitivo `R_roda = 40 px`:

```math
\Delta\theta_{\text{graus}} = \left(\frac{\Delta X}{R_{\text{roda}}}\right) \cdot \left(\frac{180}{\pi}\right)
```

No código, mantemos um acumulador de fase θ:
```csharp
double deltaX = xLocoAtual - _xLocoAnterior;
_xLocoAnterior = xLocoAtual;
double deltaAnguloGraus = (deltaX / RaioRoda) * (180.0 / Math.PI);
_anguloRodaAcumulado += deltaAnguloGraus;
```

Essa formulação garante que:
- Quando a locomotiva anda para a direita ($\Delta X > 0$), as rodas giram no sentido horário ($\Delta\theta > 0$).
- Quando ela inverte o movimento para a esquerda ($\Delta X < 0$), as rodas giram perfeitamente em marcha a ré ($\Delta\theta < 0$).

---

## 4. Etapa 3: Coordenadas Analíticas dos Pinos de Manivela

Os centros das duas rodas no espaço local do `LocomotivaCanvas` estão em:
- Centro Roda 1: `(X1, Y) = (130, 210)`
- Centro Roda 2: `(X2, Y) = (270, 210)`

Como cada manivela possui raio excêntrico `r = 22 px` e o ângulo instantâneo é `θ` (em radianos, `rad = θ × π / 180`), as posições cartesianas exatas dos dois pinos são calculadas a cada quadro por decomposição trigonométrica:

```math
\begin{cases}
pino1X = 130 + 22 \cdot \cos(\theta) \\
pino1Y = 210 + 22 \cdot \sin(\theta)
\end{cases}
```

```math
\begin{cases}
pino2X = 270 + 22 \cdot \cos(\theta) \\
pino2Y = 210 + 22 \cdot \sin(\theta)
\end{cases}
```

---

## 5. Etapa 4: Biela de Acoplamento Horizontal (*Side Rod*)

A biela de acoplamento conecta as duas rodas motrizes para que operem com o mesmo torque.
- **Geometria**: É uma barra rígida de aço com dois olhais: Olhal Traseiro em `(0,0)` e Olhal Dianteiro em `(140,0)`.
- **Cinemática**: As duas manivelas têm o mesmo raio `r = 22 px` e giram com a mesma velocidade angular `θ(t)`. Logo, a distância vetorial entre os pinos é constante e horizontal:

```math
\vec{P}_2 - \vec{P}_1 = (270 - 130, 0) = (140, 0)
```

- **Transformação Aplicada**: O corpo da biela de acoplamento não precisa de rotação sobre si mesmo (`ω = 0`). Ele executa **translação circular pura**:
  ```csharp
  TranslacaoBielaAcoplamento.X = pino1X;
  TranslacaoBielaAcoplamento.Y = pino1Y;
  ```

### Demonstração de Coincidência Geométrica:

- Posição do Olhal 1:
```math
(pino1X + 0, pino1Y + 0) = (pino1X, pino1Y)
```

- Posição do Olhal 2:
```math
(pino1X + 140, pino1Y + 0) = (130 + 22\cos\theta + 140, 210 + 22\sin\theta) = (pino2X, pino2Y)
```

Ambos os olhais coincidem com precisão absoluta de sub-pixel com os pinos das duas rodas durante os 360° da trajetória.

---

## 6. Etapa 5: Mecanismo Biela-Manivela-Pistão (*Slider-Crank*)

O mecanismo biela-manivela converte o movimento retilíneo do pistão a vapor em rotação da roda motriz.

```
       (pino2X, pino2Y)
              O============================O (xCruzeta, 210)
            /        BIELA MOTRIZ (L=95)   |
          / r=22                           | \Delta Y
        /                                  |
      O------------------------------------+
   (270, 210)          \Delta X
```

### 6.1 Restrição Cinemática da Cruzeta (*Crosshead*)
A cruzeta está mecanicamente restrita a deslizar dentro das guias de aço horizontais, fixando sua ordenada estritamente em:

```math
Y_{\text{cruzeta}} = 210\text{ px}
```

A biela motriz tem comprimento fixo entre olhais `L = 95 px`. Aplicando o **Teorema de Pitágoras** no triângulo retângulo formado pelo pino da Roda 2 e a cruzeta:

```math
(x_{\text{cruzeta}} - pino2X)^2 + (Y_{\text{cruzeta}} - pino2Y)^2 = L^2
```

Substituindo `Y_cruzeta = 210`:

```math
\Delta Y = 210 - pino2Y = -22 \cdot \sin(\theta)
```

```math
(x_{\text{cruzeta}} - pino2X)^2 + \Delta Y^2 = L^2
```

Como a cruzeta fica à frente da roda (`x_cruzeta > pino2X`):

```math
x_{\text{cruzeta}} = pino2X + \sqrt{L^2 - \Delta Y^2}
```

No código C#:
```csharp
double catetoVertical = CentroRodasY - pino2Y;
double termoRadical = Math.Max(0.0, (ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical));
double catetoHorizontal = Math.Sqrt(termoRadical);
double xCruzeta = pino2X + catetoHorizontal;
```
> [!NOTE]
> O uso de `Math.Max(0.0, ...)` é uma salvaguarda numérica para evitar raiz quadrada de números negativos em caso de anomalias de ponto flutuante. Como `L = 95 px` e `|ΔY| ≤ 22 px`, temos `L² - ΔY² ≥ 95² - 22² = 9025 - 484 = 8541 > 0`, garantindo que o radical seja sempre positivo.

---

## 7. Etapa 6: Orientação Angular da Biela Motriz (`Math.Atan2`)

A biela motriz possui seu Olhal Traseiro em `(0,0)` e seu Olhal Dianteiro a uma distância `L = 95 px` ao longo de seu eixo local X.

1. **Translação**: O olhal traseiro é transladado diretamente para o pino da Roda 2:
   ```csharp
   TranslacaoBielaMotriz.X = pino2X;
   TranslacaoBielaMotriz.Y = pino2Y;
   ```
2. **Ângulo de Apontamento**: A biela precisa rotacionar em torno do seu olhal traseiro para que o olhal dianteiro atinja a cruzeta em `(xCruzeta, 210)`.
   O vetor que liga o pino à cruzeta é:

```math
\vec{V} = (x_{\text{cruzeta}} - pino2X, 210 - pino2Y)
```

No WPF, onde o eixo Y aponta para baixo e rotações positivas são no sentido horário:

```math
\alpha = \text{atan2}(210 - pino2Y, x_{\text{cruzeta}} - pino2X) \cdot \left(\frac{180}{\pi}\right)
```

No código:
```csharp
double anguloBielaMotriz = Math.Atan2(CentroRodasY - pino2Y, xCruzeta - pino2X) * (180.0 / Math.PI);
RotacaoBielaMotriz.Angle = anguloBielaMotriz;
```

### Prova Matemática de Fechamento do Mecanismo:
A posição final do olhal dianteiro na tela é:

```math
X_{\text{frente}} = pino2X + L \cdot \cos\alpha = pino2X + L \cdot \frac{x_{\text{cruzeta}} - pino2X}{L} = x_{\text{cruzeta}}
```

```math
Y_{\text{frente}} = pino2Y + L \cdot \sin\alpha = pino2Y + L \cdot \frac{210 - pino2Y}{L} = 210
```

Portanto, **o olhal dianteiro coincide com o pino da cruzeta em 100% dos quadros**, sem qualquer desvio ou atraso perceptível.

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Como renderizar em um intervalo por quadro usando CompositionTarget](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/how-to-render-on-a-per-frame-interval-using-compositiontarget)
- [Microsoft Learn — Evento CompositionTarget.Rendering](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering)
- [Microsoft Learn — Classe Stopwatch](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch)
- [Microsoft Learn — Método Math.Atan2](https://learn.microsoft.com/pt-br/dotnet/api/system.math.atan2)
- [Microsoft Learn — Visão Geral de Transformações](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/)
