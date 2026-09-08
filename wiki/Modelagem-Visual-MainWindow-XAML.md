# 🎨 Modelagem Visual e Geometria XAML

Este capítulo detalha a construção geométrica de todas as partes da locomotiva a vapor encapsuladas no componente [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml) e o cenário ferroviário em [`MainWindow.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/MainWindow.xaml), demonstrando como cada componente foi modelado a partir de primitivas vetoriais na origem canônica $(0,0)$ e transformado via `RenderTransform`.

---

## 1. Visão Geral das Primitivas Vetoriais no WPF

O WPF oferece classes derivadas de [`Shape`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/) para desenho vetorial de alto desempenho:

- [`Rectangle`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle): Utilizado para vigas de chassi, blocos de cilindro, caixas de água, cabine e hastes metálicas retilíneas. Suporta cantos arredondados via `RadiusX` e `RadiusY`.
- [`Ellipse`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse): Utilizada para rodas, cubos, parafusos de eixos, olhais de bielas, faróis, cúpula do domo de vapor e partículas esféricas de fumaça.
- [`Polygon`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon): Utilizado para superfícies poligonais complexas como o teto curvo da cabine, o limpa-trilhos em cunha (*cowcatcher*), o bico do farol e o feixe de luz volumétrico.
- [`Line`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line): Utilizada para os raios internos das rodas, corrimãos de acesso, grades protetoras e guias lineares da cruzeta (*slide bars*).

---

## 2. Cenário de Fundo e Linha Férrea

### 2.1 Gradiente do Céu Noturno (`CenarioCanvas`)
O fundo da cena é composto por um gradiente vertical [`LinearGradientBrush`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.lineargradientbrush) de quatro paradas (`GradientStop`), simulando uma atmosfera noturna industrial:
- `0.0`: Azul escuro profundo (`#0B1319`).
- `0.5`: Azul meia-noite (`#1A2A38`).
- `0.85`: Azul ardósia médio (`#284659`).
- `1.0`: Carvão escuro (`#17202A`).

### 2.2 Lastro de Brita e Trilhos de Aço Estendidos (Edge-to-Edge)
O `CenarioCanvas` ocupa 100% da área da linha central da janela (`Grid.Row="1"`), sem margens ou bordas laterais. O conjunto da ferrovia é posicionado em $Y = 405\text{ px}$ e se estende por toda a largura com fundação descendo até a barra inferior:
```xml
<Canvas>
    <Canvas.RenderTransform>
        <TranslateTransform X="-700" Y="405"/>
    </Canvas.RenderTransform>
    <!-- Solo inferior preenchendo solidamente até o rodapé sem vazamentos -->
    <Rectangle Width="4000" Height="250" Fill="#17202A"/>
    <!-- Lastro de brita -->
    <Rectangle Width="4000" Height="30" Fill="#283747">
        <Rectangle.RenderTransform>
            <TranslateTransform X="0" Y="6"/>
        </Rectangle.RenderTransform>
    </Rectangle>
    <!-- Base do Trilho -->
    <Rectangle Width="4000" Height="10" Fill="#11171E">
        <Rectangle.RenderTransform>
            <TranslateTransform X="0" Y="6"/>
        </Rectangle.RenderTransform>
    </Rectangle>
    <!-- Trilho Superior de Aço Cromado (tangente exata à roda em Y=405) -->
    <Rectangle Width="4000" Height="6" Fill="#BDC3C7" Stroke="#7F8C8D" StrokeThickness="1">
        <Rectangle.RenderTransform>
            <TranslateTransform X="0" Y="0"/>
        </Rectangle.RenderTransform>
    </Rectangle>
</Canvas>
```
O topo do trilho fica rigorosamente em `Y = 405 px`. Como a locomotiva tem sua origem em `Y = 155 px` e o centro das rodas fica em `Y_local = 210 px` com raio `R = 40 px`, a base inferior da roda toca exatamente:

```math
Y_{\text{contato}} = 155 + 210 + 40 = 405\text{ px}
```

Isso estabelece **tangência perfeita** entre a roda e o trilho superior sem qualquer penetração ou flutuação visual.

---

## 3. Decomposição Estrutural da Locomotiva (0-4-0T)

A locomotiva é do tipo autônomo **0-4-0T (Tank Engine)**, com 4 rodas motrizes acopladas, tanques de água laterais e depósito traseiro de carvão, dispensando o uso de vagão tender traseiro.

```
       [CHAMINÉ]     [SINO]  [DOMO]
          ||           ||     (  )
        +----+---------------------+-------+     [TETO CABINE]
        |    |                     |       |    /=============\
 [FAROL]|    |       CALDEIRA      |       |===|               |=== [CARVÃO]
  [>]===|    |                     |  CAB  |   |    CABINE     |    [BUNKER]
        +----+---------------------+       |   |               |
 [CILINDRO]     |  TANQUE LATERAL  |       |   |  [PORTA] [JAN]|
  [======]      +------------------+-------+---+---------------+
 [CRUZETA] ====[BIELA]=================[RODA 1]========[RODA 2]==== [ESTRIBOS]
================================================================================= [TRILHO]
```

### Seção 1: Chassi e Estrutura Inferior Rígida
- **Viga Principal do Chassi**: `Rectangle` com dimensões $500 \times 18\text{ px}$, posicionado em $(15, 190)$ com cor grafite industrial (`#1C2833`) e bordas arredondadas.
- **Para-choque Traseiro**: Viga de madeira de lei vermelha em $(8, 188)$ e disco de absorção de impacto metálico em $(2, 192)$.

### Seção 2: Para-choques Dianteiros, Engate e Limpa-Trilhos (*Cowcatcher*)
- **Buffer Beam Dianteiro**: Bloco em $(496, 187)$ de cor vermelha ferroviária (`#C0392B`).
- **Prato Amortecedor**: Disco elíptico em $(514, 191)$ com haste em $(508, 196)$.
- **Engate Ferroviário**: Gancho forjado em $(506, 203)$ com elo de corrente móvel em $(514, 204)$.
- **Limpa-Trilhos em Cunha**: Polígono em cunha com vértices `Points="0,0 26,38 0,38"` em $(496, 208)$, descendo até rente ao trilho ($Y = 246\text{ px}$ local), equipado com duas hastes protetoras diagonais em amarelo de segurança (`#F4D03F`).

### Seção 3: Depósito Traseiro de Carvão (*Bunker*)
- Compartimento em chapa de aço em $(20, 95)$ com volume $35 \times 100\text{ px}$.
- Carvão mineral irregular no topo desenhado com `Polygon` texturizado em carvão bruto (`#17202A`).
- Lanterna de cauda com luz vermelha em $(12, 110)$.

### Seção 4: Cabine do Maquinista
- **Corpo da Cabine**: Dimensões $120 \times 135\text{ px}$ posicionado em $(55, 65)$ em azul prussiano industrial (`#21618C`).
- **Teto Curvo**: Polígono aerodinâmico com beiral protetor em $(45, 52)$.
- **Porta de Acesso**: Retângulo em $(62, 95)$ com maçaneta de bronze dourado em $(90, 145)$ e corrimão de latão em $(60, 102)$.
- **Estribos de Acesso**: Escada metálica com dois degraus de aço e montantes verticais em $(66, 192)$ para subida da tripulação.
- **Janela Panorâmica**: Janela envidraçada em azul céu claro com divisória central metálica em $(112, 88)$.

### Seção 5: Caldeira Cilíndrica e Cintas de Reforço
- **Tambor de Vapor**: Cilindro de vapor de alta pressão em $(175, 95)$ com dimensões $235 \times 65\text{ px}$.
- **Cintas de Bronze**: Duas cintas verticais douradas em $(235, 95)$ e $(300, 95)$ simulando as braçadeiras de retenção da manta térmica de amianto/madeira.

### Seção 6: Tanques Laterais de Água (*Side Tanks*)
- Tanque retangular em chapa rebitada em $(175, 135)$ com friso de acabamento em $(175, 145)$.
- Bocal de abastecimento de água no topo com tampa de latão polido em $(254, 124)$.

### Seção 7: Domo de Vapor, Sino, Chaminé e Farol
- **Domo de Vapor**: Cúpula elíptica em $(220, 70)$ com base de assento em $(218, 90)$.
- **Sino de Alerta Ferroviário**: Sino de latão dourado em $(295, 80)$ com suporte estrutural de montagem.
- **Chaminé Cônica**: Tubo em tronco de cone com alargamento superior em $(383, 46)$ e anel de borda vermelho em $(382, 42)$.
- **Farol de Proa e Feixe de Luz**: Caixa do refletor em $(428, 102)$ com lente de vidro amarela em $(452, 106)$ e um feixe poligonal translúcido (`#18FFF59D`) projetado para a frente em $(475, 100)$.

### Seção 8: Bloco do Cilindro de Vapor e Guias da Cruzeta
O conjunto propulsor dianteiro foi projetado com rigor mecânico:
- **Suporte Estrutural (*Motion Bracket / Guide Yoke*)**: Coluna vertical em $(314, 192)$ que ancora as guias ao chassi.
- **Bloco de Ferro Fundido**: Retângulo em $(390, 192)$ de $75 \times 36\text{ px}$ perfeitamente centrado no eixo $Y = 210\text{ px}$.
- **Cabeçote Frontal**: Tampa frontal do cilindro em $(457, 192)$.
- **Gaxeta Traseira**: Retentor de vedação por onde a haste cromada entra no cilindro em $(386, 197)$.
- **Guias da Cruzeta (*Slide Bars*)**: Dois trilhos horizontais paralelos de aço cromado em $Y = 200\text{ px}$ e $Y = 220\text{ px}$, criando a canaleta de 20 px onde a cruzeta desliza ao longo do eixo $Y = 210\text{ px}$.

---

## 4. Ordenação de Camadas Visuais (Z-Index)

A ordem dos elementos declarados no XAML define a pilha de desenho (*painter's algorithm*):
1. **Cenário de Fundo**: Céu noturno e gradientes atmosféricos no `CenarioCanvas`.
2. **Trilhos e Lastro de Brita**: Estendidos de $X = -700$ a $X = 3300\text{ px}$.
3. **Corpo Estrutural da Locomotiva**: Caldeira, cabine, bunker de carvão, tanques e chassi em $(0,0)$.
4. **Suporte Estrutural e Guias da Cruzeta (*Slide Bars*)**: Canaleta horizontal em $Y = 210\text{ px}$.
5. **Haste Cromada do Pistão** (`TranslacaoHastePistao`): Desenha-se sob as rodas e sob o cilindro.
6. **Rodas da Locomotiva** (instâncias do `RodaTemplate` com seus `RotateTransform`).
7. **Biela de Acoplamento Horizontal** (*Side Rod* com `MancalBielaTemplate`).
8. **Biela Motriz Articulada** (*Connecting Rod*, $L=82\text{ px}$): O olhal dianteiro conecta atrás do bloco da cruzeta.
9. **Cruzeta e Pino Frontal** (`TranslacaoCruzeta` e `TranslacaoPinoCruzeta`): Desenhados sobre a biela motriz e sobre a haste.
10. **Bloco do Cilindro de Vapor e Gaxeta Traseira**: Renderizados **sobrepostos à haste**, ocultando sua extremidade frontal e produzindo a ilusão visual perfeita de diminuição e aumento do curso à medida que o trem anda.
11. **Partículas de Vapor e Fumaça** saindo da chaminé com animação de escala e opacidade via `Storyboard`.

Essa sobreposição garante precisão mecânica total: a cruzeta segura a haste, a biela motriz movimenta a cruzeta, e a haste entra e sai realisticamente de dentro do bloco do cilindro.

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Formas e Desenho Básico no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/)
- [Microsoft Learn — Classe Rectangle](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle/)
- [Microsoft Learn — Classe Ellipse](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse/)
- [Microsoft Learn — Classe Polygon](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon/)
- [Microsoft Learn — Classe Line](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line/)
- [Microsoft Learn — Pinceis e Cores no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/wpf-brushes-overview/)
