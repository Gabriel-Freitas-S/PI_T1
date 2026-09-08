# 📐 Cinemática Analítica do Mecanismo Biela-Manivela

Neste capítulo, explora-se a modelagem matemática exata encapsulada no motor desacoplado [`Models/LocomotivaKinematics.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Models/LocomotivaKinematics.cs) e orquestrada em [`MainWindow.xaml.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/MainWindow.xaml.cs), atendendo aos requisitos de sincronismo mecânico do **[Trabalho C1 (Normas)](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L48)** (Etapa 3 - 8,0 pontos e Etapa 4 - 10,0 pontos).

---

## 1. O Loop de Renderização: `CompositionTarget.Rendering`

Ao contrário de abordagens rudimentares baseadas em `DispatcherTimer` ou `System.Threading.Thread.Sleep` (que introduzem *jitter*, engasgos e descompasso com a taxa de atualização do monitor), o projeto conecta a atualização visual diretamente ao manipulador de eventos de alta prioridade do WPF:

```csharp
CompositionTarget.Rendering += AtualizarQuadroMecanico;
```

Conforme especificado no **Microsoft Learn**, o evento [`CompositionTarget.Rendering`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering) é acionado pelo subsistema gráfico do WPF **uma vez por quadro** (tipicamente a 60 Hz, 120 Hz ou 144 Hz conforme o monitor do usuário), logo após a passagem de layout e imediatamente antes da composição final da árvore visual pela GPU.

A `MainWindow.xaml.cs` atua apenas como orquestradora: captura o tempo com `_cronometro.Elapsed.TotalSeconds`, solicita o quadro calculado ao `LocomotivaKinematics` e delega o [`LocomotivaFrameState`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Models/LocomotivaFrameState.cs) diretamente ao controle autônomo através de `Locomotiva.AtualizarEstado(estado)`.

### Temporização de Alta Resolução (`Stopwatch`)
Para evitar qualquer defasagem acumulada por variações de *frame-rate*, o tempo decorrido $t$ é mensurado com o cronômetro nativo do hardware ([`System.Diagnostics.Stopwatch`](https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch)):
```csharp
double segundos = _cronometro.Elapsed.TotalSeconds;
LocomotivaFrameState estado = _kinematics.CalcularQuadro(segundos);
```

---

## 2. Etapa 1: Translação Horizontal Contínua em Circuito de Túnel Infinito

Para atender à experiência de travessia ferroviária contínua solicitada, o motor físico [`Models/LocomotivaKinematics.cs`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Models/LocomotivaKinematics.cs) opera exclusivamente no modo de **Loop Contínuo (Túnel Ferroviário)**.

A locomotiva avança continuamente para a frente em sentido único:
1. **Ponto de Partida**: Inicia sua trajetória completamente oculta fora da janela à esquerda ($X_{\text{start}} = -560\text{ px}$, correspondendo à largura total do corpo da locomotiva).
2. **Travessia Completa**: Percorre toda a extensão visível do cenário ferroviário sobre os trilhos contínuos.
3. **Ponto de Saída**: Ultrapassa e desaparece completamente pela margem direita ($X_{\text{end}} = \text{larguraCenario}$, capturada dinamicamente de `CenarioCanvas.ActualWidth`).
4. **Reentrada Instantânea**: No exato instante em que o último milímetro do chassi sai pela direita, a locomotiva reaparece na extrema esquerda, simulando um circuito fechado de túnel contínuo.

### Modelagem Matemática da Posição Instantânea:

A distância total percorrida por ciclo de travessia é:

```math
\text{distanciaTotal} = \text{larguraCenario} - X_{\text{start}} = \text{larguraCenario} + 560\text{ px}
```

Com velocidade escalar constante $v$ calibrada para uma duração de ciclo $T = 11.0\text{ s}$:

```math
v = \frac{\text{distanciaTotal}}{T} \approx \frac{1660\text{ px}}{11.0\text{ s}} \approx 150.91\text{ px/s}
```

A cada quadro, a distância total percorrida $s(t)$ e o progresso normalizado no ciclo $u(t) \in [0, 1)$ são dados por:

```math
s(t) = v \cdot t
```

```math
u(t) = \frac{t \pmod T}{T}
```

A posição horizontal instantânea do trem no `LocomotivaCanvas` é:

```math
x_{\text{loco}}(t) = X_{\text{start}} + \text{distanciaTotal} \cdot u(t)
```

### Rolamento Monotônico Contínuo sem Saltos Angulares:

Como o trem desloca-se em sentido único progressivo, a rotação acumulada das rodas $\theta(t)$ é calculada com base na distância temporal total $s(t)$, acoplada ao raio primitivo $R_{\text{roda}} = 40\text{ px}$:

```math
\theta(t) = \left(\frac{v \cdot t}{R_{\text{roda}}}\right) \cdot \left(\frac{180}{\pi}\right)
```

> [!TIP]
> Por ser derivada diretamente da função contínua do tempo $s(t)$, a rotação angular $\theta(t)$ cresce monotonicamente sem qualquer salto angular, tranco ou quebra de fase quando o trem reentra pela esquerda, mantendo o movimento mecânico 100% fluido e ininterrupto.

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
            /        BIELA MOTRIZ (L=82)   |
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

A biela motriz tem comprimento fixo entre olhais `L = 82 px`. Aplicando o **Teorema de Pitágoras** no triângulo retângulo formado pelo pino da Roda 2 e a cruzeta:

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
> O uso de `Math.Max(0.0, ...)` é uma salvaguarda numérica para evitar raiz quadrada de números negativos em caso de anomalias de ponto flutuante. Como `L = 82 px` e `|ΔY| ≤ 22 px`, temos `L² - ΔY² ≥ 82² - 22² = 6724 - 484 = 6240 > 0`, garantindo que o radical seja sempre positivo.

### 6.2 Curso Dinâmico e Oclusão Mecânica da Haste do Pistão
A haste cromada do pistão (`Width="75 px"`) conecta a cruzeta móvel ao êmbolo interno do cilindro de vapor. A sua extremidade traseira é solidária à cruzeta:

```csharp
TranslacaoHastePistao.X = xCruzeta;
```

- **Curso Operacional da Cruzeta**: Com `L = 82 px` e $r = 22\text{ px}$, $x_{\text{cruzeta}}$ oscila estritamente no intervalo $[330\text{ px}, 374\text{ px}]$ (amplitude de $44\text{ px}$).
- **Guias da Cruzeta (*Slide Bars*)**: Estendem-se de $X = 318\text{ px}$ até a gaxeta do cilindro em $X = 386\text{ px}$. O curso de $[330, 374]$ mantém uma folga simétrica de segurança de exatamente $12\text{ px}$ em relação ao retentor frontal ($386 - 374 = 12\text{ px}$).
- **Efeito Visual de Diminuição e Aumento Realista**: Na árvore visual, o bloco do cilindro (iniciado em $X = 390\text{ px}$) e a gaxeta de vedação ($X = 386\text{ px}$) são renderizados **sobrepostos à haste**. Quando as rodas giram:
  - Na posição de recuo máximo ($\theta = 180^\circ$, $x_{\text{cruzeta}} = 330\text{ px}$): a haste é puxada para fora do cilindro, ficando $47\text{ px}$ de sua extensão visíveis (*aumento*).
  - Na posição de avanço máximo ($\theta = 0^\circ$, $x_{\text{cruzeta}} = 374\text{ px}$): a haste penetra no corpo do cilindro, restando apenas $3\text{ px}$ visíveis (*diminuição*).

Essa oclusão por camadas no WPF reproduz com perfeição a cinemática industrial de um pistão de locomotiva a vapor real.

---

## 7. Etapa 6: Orientação Angular da Biela Motriz (`Math.Atan2`)

A biela motriz possui seu Olhal Traseiro em `(0,0)` e seu Olhal Dianteiro a uma distância `L = 82 px` ao longo de seu eixo local X.

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
