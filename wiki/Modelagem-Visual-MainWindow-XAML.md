# Modelagem Visual e Geometria Vetorial em XAML

![Módulo](https://img.shields.io/badge/M%C3%B3dulo-Modelagem%20Visual%20XAML-007ACC?style=flat-square)
![Geometria](https://img.shields.io/badge/Geometria-0--4--0T%20Tank%20Engine-2ecc71?style=flat-square)
![Renderização](https://img.shields.io/badge/Renderiza%C3%A7%C3%A3o-Primitivas%20Vetoriais%20(0%2C0)-brightgreen?style=flat-square)
![Camadas](https://img.shields.io/badge/Pipeline-Z--Order%20Hier%C3%A1rquico-blue?style=flat-square)

Neste capítulo, detalha-se a engenharia de modelagem visual, a taxonomia vetorial e a estruturação geométrica do projeto. Toda a composição gráfica da locomotiva a vapor foi desenvolvida com primitivas vetoriais analíticas em [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml), enquanto a infraestrutura ferroviária e o cenário atmosférico encontram-se definidos na janela principal [`MainWindow.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/MainWindow.xaml).

---

## 1. Sistema de Coordenadas e Primitivas Gráficas Vetoriais

### 1.1 Espaço Canônico do WPF

No subsistema de apresentação do WPF (*Windows Presentation Foundation*), o espaço euclidiano bidimensional adota a seguinte convenção canônica:

- **Origem $(0,0)$**: Localizada no vértice superior esquerdo da área cliente do contêiner.
- **Eixo $X$ (Abscissa)**: Cresce positivamente no sentido horizontal da esquerda para a direita $(\to)$.
- **Eixo $Y$ (Ordenada)**: Cresce positivamente no sentido vertical de cima para baixo $(\downarrow)$, em conformidade com o padrão de endereçamento de quadros da computação gráfica matricial.

```text
(0,0) Origem Canônica
  +-------------------------------------> Eixo X (Horizontal: Esquerda -> Direita)
  |
  |      [Instância Vetorial em (X, Y)]
  |
  v
Eixo Y (Vertical: Topo -> Base)
```

### 1.2 Primitivas Derivadas de `System.Windows.Shapes.Shape`

A renderização do projeto é puramente vetorial analítica, dispensando o uso de bitmaps rasterizados. O subsistema gráfico do WPF provê quatro classes fundamentais derivadas da classe abstrata [`Shape`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/):

1. [`Rectangle`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle) (**Retângulos**): Utilizado na estruturação dos componentes ortogonais, como a viga principal do chassi, os tanques laterais de água, a cabine de comando e hastes lineares. Permite curvatura de vértices através das propriedades `RadiusX` e `RadiusY`.
2. [`Ellipse`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse) (**Círculos e Elipses**): Aplicado aos cubos e aros das rodas, mancais das bielas, cúpula do domo de vapor e esferas de condensação da chaminé.
3. [`Polygon`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon) (**Polígonos Fechados Arbitrários**): Define geometrias complexas a partir de uma coleção ordenada de vértices (`Points`), tais como o perfil do teto da cabine, a cunha do limpa-trilhos (*cowcatcher*) e o cone de dispersão luminosa do farol.
4. [`Line`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line) (**Segmentos de Reta**): Elementos unindo as coordenadas $(X_1, Y_1)$ a $(X_2, Y_2)$. Empregado nos 8 raios de cada roda, corrimãos de acesso e barras de guia horizontal (*slide bars*) da cruzeta.

---

## 2. Cenário: Atmosfera e Traçado Ferroviário

### 2.1 Atmosfera com Interpolação Linear de Cores

Para estabelecer o plano de fundo da cena sem o custo de imagens bitmap, aplicou-se um [`LinearGradientBrush`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.lineargradientbrush) vertical em `MainWindow.xaml`:

- Topo ($0\%$ da altura): Azul noturno denso (`#0B1319`).
- Intermediário Superior ($50\%$): Azul ardósia escuro (`#1A2A38`).
- Horizonte Inferior ($85\%$): Azul petróleo industrial (`#284659`).
- Solo ($100\%$): Carvão escuro de transição para o lastro (`#17202A`).

### 2.2 Tangência Matemática entre Rodas e Linha Férrea

Para evitar descontinuidades visuais ou interpenetrações de malha (*mesh penetration*), a geometria de contato foi calibrada analiticamente:

- Topo da superfície do trilho: $Y_{\text{trilho}} = 405.0\text{ px}$.
- Origem vertical da locomotiva no cenário: $Y_{\text{loco}} = 155.0\text{ px}$.
- Centro do eixo de rotação das rodas no contêiner: $Y_{\text{eixo}} = 210.0\text{ px}$.
- Raio primitivo externo da roda: $R = 40.0\text{ px}$ (diâmetro de $80.0\text{ px}$).

```math
Y_{\text{base da roda}} = 155.0\text{ (origem)} + 210.0\text{ (eixo)} + 40.0\text{ (raio)} = \mathbf{405.0\text{ px}}
```

Como o ponto inferior tangencial das rodas coincide rigorosamente com a superfície superior do boleto do trilho ($405.0\text{ px}$), obtém-se tangência geométrica estrita, assegurando rolamento sem flutuações ou afundamentos.

---

## 3. Decomposição Estrutural da Locomotiva (0-4-0T)

A locomotiva segue a classificação clássica **0-4-0T (Tank Engine)** segundo a notação ferroviária Whyte:

- **0**: Sem rodeiros de guiamento dianteiro (*leading truck*).
- **4**: Quatro rodas motrizes acopladas sustentando a totalidade do peso aderente (duas por lado).
- **0**: Sem rodeiros de suporte traseiro (*trailing truck*).
- **T (*Tank*)**: Armazenamento de água em tanques montados lateralmente à caldeira e de carvão em compartimento traseiro à cabine, eliminando a necessidade de veículo de abastecimento separado (*tender*).

### Diagrama Estrutural da Composição (Orientação Frontal à Direita)

```text
                                      [DOMO]  [SINO]     [CHAMINÉ]
                                       (  )     ||          ||
                [TETO CABINE]       +-------+-------+----+----+
               /=============\      |       |       |    |    |
[CARVÃO] ===|                 |======|        CALDEIRA    |    |===[>] [FAROL]
[BUNKER]    |     CABINE      |      |                    |    |
            |                 |      +--------------------+----+
            |  [PORTA]  [JAN] |      |   TANQUE LATERAL   |      [CILINDRO]
+-----------+-----------------+------+--------------------+      [======]
| [ESTRIBOS]====[RODA 1]====[BIELA]====[RODA 2]====[MOTRIZ]====[CRUZETA] \> [PILOT]
================================================================================= [TRILHO]
   (Traseira / Esquerda)  <------------------------>  (Frente / Direita)
```

---

### Decomposição das 9 Seções Estruturais

#### Seção 1: Chassi Inferior e Batentes Traseiros

- **Viga Principal do Chassi**: Perfil estrutural em aço carbono (`#1C2833`) com $500\text{ px}$ de comprimento, transladado para $(15, 190)$, responsável pela rigidez estrutural da superestrutura.
- **Para-choque e Amortecedor Posterior**: Viga de amortecimento em $(8, 188)$ com batente circular em $(2, 192)$ para absorção de impacto em manobras de engate.

#### Seção 2: Para-Choque Dianteiro e Limpa-Trilhos (*Cowcatcher*)

- **Viga Dianteira (*Buffer Beam*)**: Elemento transversal de montagem em $(496, 187)$.
- **Gancho e Manilha de Tração**: Conjunto forjado em $(506, 203)$ para reboque ferroviário.
- **Limpa-Trilhos em Cunha (*Cowcatcher*)**: Estrutura triangular montada em $(496, 208)$ para desobstrução da via. Possui ranhuras diagonais de advertência visual em esmalte amarelo (`#F4D03F`).

#### Seção 3: Depósito Traseiro de Combustível (*Bunker*)

- Compartimento volumétrico situado imediatamente atrás da cabine em $(20, 95)$, modelado com polígonos escuros simulando o leito de carvão mineral.
- Lanterna de cauda em $(12, 110)$ com emissão luminosa em tom rubro para sinalização traseira da composição.

#### Seção 4: Cabine de Comando do Maquinista

- Cabine principal em esmalte azul prussiano (`#21618C`), posicionada em $(55, 65)$.
- **Teto Curvo**: Polígono aerodinâmico superior com beirais laterais para drenagem pluviométrica.
- **Porta de Acesso com Maçaneta de Bronze**: Posicionada em $(62, 95)$.
- **Estribos de Ferro Forjado**: Dois degraus estruturais em $(66, 192)$, alinhados ao vão de entrada.
- **Janela de Observação**: Abertura envidraçada em $(112, 88)$ com montante divisório central simulando caixilhos duplos corrediços.

#### Seção 5: Caldeira Cilíndrica e Cintas de Fixação

- Reservatório pressurizado horizontal centrado em $(175, 95)$, onde ocorre a vaporização contínua de água sob alta pressão.
- **Cintas de Fixação Térmica**: Abraçadeiras em liga de bronze em $(235, 95)$ e $(300, 95)$, modelando as cintas de contenção do isolamento térmico externo.

#### Seção 6: Tanques Laterais de Água (*Side Tanks*)

- Reservatórios prismáticos montados nas laterais da caldeira em $(175, 135)$, destinados ao suprimento gravitacional contínuo da caldeira.

#### Seção 7: Domo de Vapor, Sino, Chaminé e Farol

- **Domo de Vapor**: Câmara esférica superior em $(220, 70)$, concebida para reter vapor seco saturado e minimizar arraste de condensado para a tubulação do motor.
- **Sino de Latão**: Dispositivo de sinalização acústica em $(295, 80)$.
- **Chaminé Cônica de Exaustão**: Duto de dispersão em $(383, 46)$ para liberação dos gases de combustão e exaustão dos cilindros.
- **Farol de Proa e Refletor**: Lanterna frontal montada em $(428, 102)$, dotada de cone de projeção volumétrica translúcida em tom amarelo suave (`#18FFF59D`).

#### Seção 8: Bloco do Cilindro e Guias da Cruzeta

- **Bloco do Cilindro**: Câmara de ferro fundido em $(390, 192)$, atuando como invólucro de expansão de vapor onde a energia térmica é convertida em movimento linear alternativo.
- **Barras de Guia da Cruzeta (*Slide Bars*)**: Perfis prismáticos cromados dispostos em $Y = 200$ e $Y = 220$, restringindo o deslocamento da cruzeta estritamente ao eixo axial horizontal.

#### Seção 9: Sistema Mecânico Biela-Manivela e Rodas

- Conjunto de duas rodas motrizes com diâmetro de $80\text{ px}$ instanciadas via `RodaTemplate`.
- Biela de acoplamento horizontal (*side rod*) em translação circular pura.
- Biela motriz (*connecting rod*) inclinada com rotação angular calculada por `Math.Atan2`.
- Haste e bloco deslizante da cruzeta (*crosshead*).

---

## 4. Ordem de Renderização e Profundidade (Z-Order)

No WPF, a ordenação em profundidade é determinada estritamente pela sequência de nós declarados no contêiner XAML. Elementos declarados previamente situam-se nos planos de fundo, enquanto os nós subsequentes sobrepõem-se:

```text
[Camada 11]  Vapor e Partículas de Exaustão           (Primeiro plano superior)
[Camada 10]  Bloco de Ferro do Cilindro             (Oclusão da haste)
[Camada 9]   Cruzeta e Mancal Frontal               (Acoplamento articular)
[Camada 8]   Biela Motriz Inclinada                 (Transmissão intermediária)
[Camada 7]   Biela de Acoplamento Horizontal        (Sincronização dos eixos)
[Camada 6]   Conjuntos de Rodas e Raios             (Rodeiros acoplados)
[Camada 5]   Haste Cromada do Pistão                (Translação alternativa)
[Camada 4]   Barras de Guia da Cruzeta (Slide Bars) (Guiamento mecânico)
[Camada 3]   Superestrutura da Locomotiva           (Caldeira, cabine, chassi)
[Camada 2]   Infraestrutura de Linha Férrea         (Trilhos e lastro de brita)
[Camada 1]   Céu Noturno e Gradiente Atmosférico    (Plano de fundo estático)
```

### Dinâmica de Oclusão do Êmbolo e Cilindro

A interação visual entre as **Camadas 5, 9 e 10** exemplifica o uso eficiente do Z-Order:

1. A haste do pistão é definida como um elemento retangular com comprimento de $75\text{ px}$.
2. A cruzeta (Camada 9) articula-se à extremidade esquerda da haste.
3. O bloco do cilindro (Camada 10) é renderizado em sobreposição direta à extremidade direita da haste.
4. No curso de avanço, a porção distal da haste é encoberta pelo corpo opaco do cilindro; no recuo, a haste reaparece progressivamente. Essa alternância dinâmica produz o efeito de inserção volumétrica contínua no motor a vapor com custo computacional mínimo, sem necessidade de algoritmos de corte booleano (*CSG*) ou máscara de recorte (*clip path*).

---

## 5. Análise Detalhada da Estrutura XAML

### 5.1 Cenário e Linha Férrea em `MainWindow.xaml`

A janela principal estrutura o layout vertical através de um `<Grid>` com três linhas:

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="50"/>  <!-- Barra superior com título e metadados -->
        <RowDefinition Height="*"/>   <!-- Área do cenário (céu, trilhos e locomotiva) -->
        <RowDefinition Height="36"/>  <!-- Barra inferior com status dinâmico -->
    </Grid.RowDefinitions>

    <!-- Cenário Ferroviário -->
    <Canvas x:Name="CenarioCanvas" Grid.Row="1" ClipToBounds="True">
        <!-- Fundo com gradiente vertical -->
        <Canvas.Background>
            <LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
                <GradientStop Color="#0B1319" Offset="0.0"/>
                <GradientStop Color="#1A2A38" Offset="0.5"/>
                <GradientStop Color="#284659" Offset="0.85"/>
                <GradientStop Color="#17202A" Offset="1.0"/>
            </LinearGradientBrush>
        </Canvas.Background>

        <!-- Linha Férrea: Solo, brita e trilho de contato em Y=405 -->
        <Canvas>
            <Canvas.RenderTransform>
                <TranslateTransform X="-700" Y="405"/>
            </Canvas.RenderTransform>
            <Rectangle Width="4000" Height="250" Fill="#17202A"/>
            <Rectangle Width="4000" Height="30" Fill="#283747">
                <Rectangle.RenderTransform>
                    <TranslateTransform X="0" Y="6"/>
                </Rectangle.RenderTransform>
            </Rectangle>
            <Rectangle Width="4000" Height="6" Fill="#BDC3C7" Stroke="#7F8C8D" StrokeThickness="1">
                <Rectangle.RenderTransform>
                    <TranslateTransform X="0" Y="0"/>
                </Rectangle.RenderTransform>
            </Rectangle>
        </Canvas>

        <!-- Instância do Controle da Locomotiva 2D -->
        <controls:LocomotivaControl x:Name="Locomotiva" Canvas.Left="0" Canvas.Top="155"/>
    </Canvas>
</Grid>
```

---

### 5.2 Elementos da Superestrutura em `Controls/LocomotivaControl.xaml`

O contêiner principal da locomotiva encapsula todos os subsistemas em um `Canvas` de $560 \times 260\text{ px}$.

#### Viga Principal do Chassi

```xml
<Rectangle Width="500" Height="18" Fill="{StaticResource ChassiPretoBrush}"
           Stroke="{StaticResource BordaPretaBrush}" StrokeThickness="2"
           RadiusX="3" RadiusY="3">
    <Rectangle.RenderTransform>
        <TranslateTransform X="15" Y="190"/>
    </Rectangle.RenderTransform>
</Rectangle>
```

#### Limpa-Trilhos em Cunha (*Cowcatcher*)

```xml
<Polygon Points="0,0 26,38 0,38" Fill="{StaticResource VermelhoFerroviarioBrush}"
         Stroke="{StaticResource BordaVermelhaBrush}" StrokeThickness="2">
    <Polygon.RenderTransform>
        <TranslateTransform X="496" Y="208"/>
    </Polygon.RenderTransform>
</Polygon>
<Line X1="0" Y1="0" X2="18" Y2="26" Stroke="#F4D03F" StrokeThickness="2">
    <Line.RenderTransform>
        <TranslateTransform X="500" Y="216"/>
    </Line.RenderTransform>
</Line>
```

#### Cabine de Comando e Janela Bipartida

```xml
<!-- Corpo principal da cabine -->
<Rectangle Width="120" Height="135" Fill="#21618C"
           Stroke="#154360" StrokeThickness="2" RadiusX="3" RadiusY="3">
    <Rectangle.RenderTransform>
        <TranslateTransform X="55" Y="65"/>
    </Rectangle.RenderTransform>
</Rectangle>

<!-- Teto com beirais de proteção -->
<Polygon Points="0,14 10,0 142,0 137,14" Fill="#1A5276"
         Stroke="#113349" StrokeThickness="2">
    <Polygon.RenderTransform>
        <TranslateTransform X="45" Y="52"/>
    </Polygon.RenderTransform>
</Polygon>

<!-- Janela panorâmica com moldura divisória -->
<Rectangle Width="42" Height="40" Fill="#A9CCE3" Stroke="#1B4F72" StrokeThickness="2.5" RadiusX="4" RadiusY="4">
    <Rectangle.RenderTransform>
        <TranslateTransform X="112" Y="88"/>
    </Rectangle.RenderTransform>
</Rectangle>
<Line X1="21" Y1="0" X2="21" Y2="40" Stroke="#1B4F72" StrokeThickness="2">
    <Line.RenderTransform>
        <TranslateTransform X="112" Y="88"/>
    </Line.RenderTransform>
</Line>
```

#### Caldeira Pressurizada e Cintas Térmicas

```xml
<Rectangle Width="235" Height="65" Fill="#2874A6" Stroke="#1B4F72" StrokeThickness="2" RadiusX="3" RadiusY="3">
    <Rectangle.RenderTransform>
        <TranslateTransform X="175" Y="95"/>
    </Rectangle.RenderTransform>
</Rectangle>
<Rectangle Width="6" Height="65" Fill="{StaticResource OleoCopoBrush}">
    <Rectangle.RenderTransform>
        <TranslateTransform X="235" Y="95"/>
    </Rectangle.RenderTransform>
</Rectangle>
<Rectangle Width="6" Height="65" Fill="{StaticResource OleoCopoBrush}">
    <Rectangle.RenderTransform>
        <TranslateTransform X="300" Y="95"/>
    </Rectangle.RenderTransform>
</Rectangle>
```

#### Feixe de Luz Volumétrico do Farol

```xml
<!-- Caixa do refletor -->
<Polygon Points="0,6 28,0 28,34 0,28" Fill="{StaticResource OleoCopoBrush}"
         Stroke="{StaticResource BordaOleoBrush}" StrokeThickness="2">
    <Polygon.RenderTransform>
        <TranslateTransform X="428" Y="102"/>
    </Polygon.RenderTransform>
</Polygon>

<!-- Feixe cônico de luz com canal alfa (transparência a 9.4%) -->
<Polygon Points="0,15 240,-35 240,65" Fill="#18FFF59D">
    <Polygon.RenderTransform>
        <TranslateTransform X="475" Y="100"/>
    </Polygon.RenderTransform>
</Polygon>
```

---

## Referências Oficiais da Microsoft

- [Microsoft Learn — Formas e Desenho Básico no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/)
- [Microsoft Learn — Classe Rectangle (Retângulo)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle/)
- [Microsoft Learn — Classe Ellipse (Elipse e Círculo)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse/)
- [Microsoft Learn — Classe Polygon (Polígono)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon/)
- [Microsoft Learn — Classe Line (Linha Reta)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line/)
- [Microsoft Learn — Pincéis e Cores no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/wpf-brushes-overview/)
