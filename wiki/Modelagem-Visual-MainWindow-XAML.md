# 🎨 Modelagem Visual e Geometria XAML: Desenhando a Locomotiva com Formas

Bem-vindo ao guia de modelagem visual da locomotiva a vapor! Aqui explicamos, de forma simples e direta para qualquer pessoa (mesmo sem experiência prévia com computação gráfica ou programação), como transformamos simples formas geométricas do computador em uma locomotiva histórica detalhada e funcional.

Toda a locomotiva foi desenhada em vetor dentro do arquivo [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml), e o cenário com o céu noturno e os trilhos foi montado na janela principal [`MainWindow.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/MainWindow.xaml).

---

## 1. Como o Computador Desenha na Tela? (Para Quem Nunca Mexeu com Gráficos)

Imagine uma folha de papel milimetrado ou a tela do seu celular:
- **Canto Superior Esquerdo $(0,0)$**: É o ponto de partida de tudo.
- **Eixo $X$ (Horizontal)**: Aumenta para a **direita** $(\to)$. Quanto maior o valor de $X$, mais para a direita a peça vai.
- **Eixo $Y$ (Vertical)**: No computador, o eixo $Y$ aumenta para **baixo** $(\downarrow)$! Diferente da matemática da escola onde o $Y$ sobe, nas telas de computador ele desce. Quanto maior o $Y$, mais perto do chão a peça está.

```
(0,0) Canto Superior Esquerdo
  +-------------------------------------> Eixo X (Cresce para a Direita)
  |
  |      [Sua Peça Aqui em (X, Y)]
  |
  v
Eixo Y (Cresce para Baixo)
```

### Primitivas Gráficas Vetoriais do WPF

Para evitar o carregamento de bitmaps e assegurar escalabilidade com independência de resolução gráfica, a modelagem foi integralmente desenvolvida por meio de **primitivas vetoriais analíticas**. O subsistema gráfico do WPF provê quatro classes fundamentais derivadas da classe base [`Shape`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/):

1. [`Rectangle`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle) (**Retângulos**): Utilizado na estruturação dos componentes ortogonais, como a viga principal do chassi, caixas de água laterais, cabine de comando e hastes lineares. Permite bordas arredondadas pelas propriedades `RadiusX` e `RadiusY`.
2. [`Ellipse`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse) (**Círculos e Elipses**): Aplicado aos cubos e aros das rodas, mancais das bielas, domo de vapor e esferas de exaustão de vapor da chaminé.
3. [`Polygon`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon) (**Polígonos Fechados Arbitrários**): Define geometrias complexas a partir de uma lista ordenada de vértices (`Points`), tais como o perfil do teto da cabine, a cunha do limpa-trilhos (*cowcatcher*) e a projeção cônica do farol.
4. [`Line`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line) (**Segmentos de Reta**): Elementos com coordenadas extremas $(X_1, Y_1)$ e $(X_2, Y_2)$. Empregado nos 8 raios de cada roda, corrimãos de acesso e barras de guia horizontal (*slide bars*) da cruzeta.

---

## 2. O Cenário: Céu Noturno e Traçado Ferroviário

A composição visual da cena requer o estabelecimento de um plano de fundo contínuo e da linha de referência do traçado ferroviário.

### 2.1 Céu Noturno com Gradiente Linear
Para evitar superfícies monocromáticas estáticas, aplicou-se um [`LinearGradientBrush`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.lineargradientbrush). O pincel interpola quatro patamares tonais de azul e cinza escuro na direção vertical:
- Topo: Azul noite profundo (`#0B1319`).
- Meio: Azul ardósia industrial (`#284659`).
- Base: Carvão escuro (`#17202A`).

### 2.2 Alinhamento do Trilho e Tangência das Rodas
Para evitar descontinuidades visuais ou interpenetrações de malha (*mesh penetration*), a geometria de contato foi calculada analiticamente:
- Topo da superfície do trilho: $Y = 405\text{ px}$.
- Origem global da locomotiva: $Y = 155\text{ px}$.
- Centro do eixo de rotação das rodas: $Y = 210\text{ px}$ relativo ao contêiner.
- Raio externo da roda: $R = 40\text{ px}$ (diâmetro de $80\text{ px}$).

```math
Y_{\text{base da roda}} = 155\text{ (origem do contêiner)} + 210\text{ (eixo)} + 40\text{ (raio)} = \mathbf{405\text{ px}}
```

Como o ponto inferior das rodas coincide exatamente com a cota superior do trilho ($405\text{ px}$), obtém-se **tangência matemática exata**, garantindo rolamento sem penetração de malha.

---

## 3. Decomposição Estrutural da Locomotiva (0-4-0T)

A locomotiva segue a classificação técnica **0-4-0T (Tank Engine)** segundo o sistema de notação Whyte:
- **0**: Ausência de rodeiros guia dianteiros (*leading truck*).
- **4**: Quatro rodas motrizes acopladas sustentando o peso aderente (duas por lado).
- **0**: Ausência de rodeiros de apoio traseiros (*trailing truck*).
- **T (*Tank*)**: Armazenamento de água em tanques montados lateralmente à caldeira e de carvão em compartimento posterior à cabine, dispensando veículo de suprimento separado (*tender*).

### Diagrama Estrutural da Locomotiva (Orientação Frontal à Direita $\to$)

Abaixo apresenta-se o esquema dimensional da locomotiva na convenção adotada no código-fonte (da traseira em $X=0$ até a proa em $X \approx 520$):

```
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

### Decomposição Detalhada dos Componentes

#### Seção 1: Chassi Inferior e Para-Choque Traseiro
- **Viga Principal do Chassi**: Perfil estrutural em aço carbono (`#1C2833`) de $500\text{ px}$ de comprimento, transladado para $(15, 190)$, responsável pela sustentação de toda a superestrutura.
- **Para-choque e Prato Amortecedor Posterior**: Viga de amortecimento em $(8, 188)$ com batente circular em $(2, 192)$ para dissipação de impacto em manobras de acoplamento.

#### Seção 2: Para-Choque Dianteiro e Limpa-Trilhos (*Cowcatcher*)
- **Viga Dianteira (*Buffer Beam*)**: Elemento transversal de fixação em $(496, 187)$.
- **Gancho e Manilha de Tração**: Conjunto forjado em $(506, 203)$ para reboque de material rodante.
- **Limpa-Trilhos em Cunha (*Cowcatcher*)**: Estrutura deflectora triangular montada em $(496, 208)$ com o objetivo de desobstruir a via de corpos estranhos. Possui ranhuras diagonais de advertência visual em esmalte amarelo.

#### Seção 3: Depósito Traseiro de Carvão (*Bunker*)
- Compartimento volumétrico situado imediatamente atrás da cabine em $(20, 95)$, modelado com polígonos texturizados para representar o leito de combustível sólido mineral.
- Lanterna de cauda em $(12, 110)$ com emissão luminosa em tom rubro para sinalização de fim de composição.

#### Seção 4: Cabine de Comando
- Estrutura habitável da tripulação em esmalte azul prussiano (`#21618C`), posicionada em $(55, 65)$.
- **Teto Curvo de Cobertura**: Projeção aerodinâmica superior com abaulamento e drenagem lateral.
- **Porta de Acesso com Maçaneta de Bronze**: Posicionada em $(62, 95)$.
- **Estribos de Acesso**: Conjunto de dois degraus em ferro forjado em $(66, 192)$, alinhado ao vão de entrada.
- **Janela Panorâmica de Observação**: Abertura envidraçada em $(112, 88)$ com montante divisório central.

#### Seção 5: Caldeira Cilíndrica e Cintas de Fixação
- Reservatório pressurizado horizontal centrado em $(175, 95)$, onde ocorre a vaporização contínua de água sob alta pressão.
- **Cintas de Fixação Térmica**: Abraçadeiras em liga de bronze em $(235, 95)$ e $(300, 95)$, representando as cintas de contenção do isolamento térmico externo.

#### Seção 6: Tanques Laterais de Água (*Side Tanks*)
- Reservatórios retangulares de suprimento instalados nas laterais da caldeira em $(175, 135)$, destinados ao fornecimento contínuo de água de alimentação.

#### Seção 7: Domo de Vapor, Sino, Chaminé e Farol
- **Domo de Vapor**: Câmara esférica superior em $(220, 70)$, concebida para reter vapor seco saturado e minimizar arraste de condensado para a tubulação do motor.
- **Sino de Latão**: Dispositivo de sinalização acústica em $(295, 80)$.
- **Chaminé Cônica de Exaustão**: Duto de dispersão em $(383, 46)$ para liberação dos gases de combustão e exaustão dos cilindros.
- **Farol de Proa e Refletor**: Lanterna frontal montada em $(428, 102)$, dotada de cone de projeção volumétrica translúcida.

#### Seção 8: Bloco do Cilindro e Guias da Cruzeta
- **Bloco do Cilindro**: Câmara de ferro fundido em $(390, 192)$, atuando como invólucro de expansão de vapor onde a energia térmica é convertida em movimento linear alternativo.
- **Barras de Guia da Cruzeta (*Slide Bars*)**: Perfis prismáticos cromados dispostos em $Y = 200$ e $Y = 220$, restringindo o deslocamento da cruzeta estritamente ao eixo axial horizontal.

---

## 4. Ordem de Renderização e Profundidade (Z-Order)

No WPF, a ordenação em profundidade é determinada pela sequência de nós declarados no contêiner XAML. Elementos declarados previamente situam-se nos planos de fundo, enquanto os nós subsequentes são sobrepostos:

```
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

## 5. 📖 Análise Detalhada da Estrutura XAML

Abaixo examina-se a especificação formal de cada trecho de marcação presente no projeto:

### 5.1 Cenário e Infraestrutura em `MainWindow.xaml`

No arquivo [`MainWindow.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/MainWindow.xaml), dividimos a tela com um `<Grid>` em três linhas:
- Linha 0: Barra superior escura com o título.
- Linha 1: O cenário com céu, brita, trilhos e a locomotiva.
- Linha 2: Barra inferior com atalhos e status.

#### O Gradiente do Céu:
```xml
<LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
    <GradientStop Color="#0B1319" Offset="0.0"/>   <!-- Topo: quase preto, noite fechada -->
    <GradientStop Color="#1A2A38" Offset="0.5"/>   <!-- Meio superior: azul escuro -->
    <GradientStop Color="#284659" Offset="0.85"/>  <!-- Perto do horizonte: azul petróleo claro -->
    <GradientStop Color="#17202A" Offset="1.0"/>   <!-- Chão: transição para a brita escura -->
</LinearGradientBrush>
```
- `StartPoint="0,0"` e `EndPoint="0,1"`: Informam ao WPF que o gradiente é **vertical** (de cima para baixo).
- `Offset`: É a porcentagem da altura onde cada cor atinge sua intensidade máxima ($0.0 = 0\%$ até $1.0 = 100\%$).

#### A Linha Férrea e o Trilho de Contato:
```xml
<Canvas>
    <Canvas.RenderTransform>
        <!-- Começa em X=-700 para cobrir telas ultra-largas sem faltar chão -->
        <TranslateTransform X="-700" Y="405"/>
    </Canvas.RenderTransform>

    <!-- Solo escuro de sustentação (largura de 4000px e altura de 250px) -->
    <Rectangle Width="4000" Height="250" Fill="#17202A"/>

    <!-- Lastro de brita (pedras de sustentação da ferrovia) -->
    <Rectangle Width="4000" Height="30" Fill="#283747">
        <Rectangle.RenderTransform>
            <TranslateTransform X="0" Y="6"/>
        </Rectangle.RenderTransform>
    </Rectangle>

    <!-- Trilho de Aço Cromado superior (onde as rodas apoiam) -->
    <Rectangle Width="4000" Height="6" Fill="#BDC3C7" Stroke="#7F8C8D" StrokeThickness="1">
        <Rectangle.RenderTransform>
            <TranslateTransform X="0" Y="0"/>
        </Rectangle.RenderTransform>
    </Rectangle>
</Canvas>
```
- Observe a propriedade `TranslateTransform X="0" Y="0"` no trilho de aço: ele fica exatamente em $Y = 405\text{ px}$, criando a linha guia perfeita para o rolamento das rodas.

---

### 5.2 O Corpo da Locomotiva em `Controls/LocomotivaControl.xaml`

Agora abrimos o coração do trem: [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml).

#### 1. A Viga do Chassi (O Esqueleto do Trem):
```xml
<Rectangle Width="500" Height="18" Fill="{StaticResource ChassiPretoBrush}"
           Stroke="{StaticResource BordaPretaBrush}" StrokeThickness="2"
           RadiusX="3" RadiusY="3">
    <Rectangle.RenderTransform>
        <TranslateTransform X="15" Y="190"/>
    </Rectangle.RenderTransform>
</Rectangle>
```
- `Width="500"` e `Height="18"`: A barra tem 500 pixels de comprimento e 18 pixels de espessura.
- `RadiusX="3"` e `RadiusY="3"`: Arredonda suavemente os cantos para dar acabamento de chapa usinada.
- `TranslateTransform X="15" Y="190"`: Posiciona a viga logo acima do eixo das rodas ($Y=210$).

#### 2. O Limpa-Trilhos em Cunha (*Cowcatcher*):
```xml
<!-- Cunha triangular vermelha -->
<Polygon Points="0,0 26,38 0,38" Fill="{StaticResource VermelhoFerroviarioBrush}"
         Stroke="{StaticResource BordaVermelhaBrush}" StrokeThickness="2">
    <Polygon.RenderTransform>
        <TranslateTransform X="496" Y="208"/>
    </Polygon.RenderTransform>
</Polygon>

<!-- Grades diagonais amarelas de alerta visual -->
<Line X1="0" Y1="0" X2="18" Y2="26" Stroke="#F4D03F" StrokeThickness="2">
    <Line.RenderTransform>
        <TranslateTransform X="500" Y="216"/>
    </Line.RenderTransform>
</Line>
```
- `Points="0,0 26,38 0,38"`: Cria um triângulo retângulo onde a ponta em $(0,0)$ fica perto do chassi e a base desce até $Y = 208 + 38 = 246\text{ px}$, rente aos trilhos.
- As `Line` amarelas (`#F4D03F`) simulam as hastes protetoras clássicas que afastam obstáculos.

#### 3. A Cabine do Maquinista:
```xml
<!-- Parede da cabine em azul prussiano -->
<Rectangle Width="120" Height="135" Fill="#21618C"
           Stroke="#154360" StrokeThickness="2" RadiusX="3" RadiusY="3">
    <Rectangle.RenderTransform>
        <TranslateTransform X="55" Y="65"/>
    </Rectangle.RenderTransform>
</Rectangle>

<!-- Teto curvo aerodinâmico -->
<Polygon Points="0,14 10,0 142,0 137,14" Fill="#1A5276"
         Stroke="#113349" StrokeThickness="2">
    <Polygon.RenderTransform>
        <TranslateTransform X="45" Y="52"/>
    </Polygon.RenderTransform>
</Polygon>

<!-- Janela panorâmica com vidro azul céu e moldura de divisão -->
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
- O teto curvo foi feito com `Polygon Points="0,14 10,0 142,0 137,14"`: note que ele é $22\text{ px}$ mais largo que a cabine ($142\text{ px}$ contra $120\text{ px}$), criando beirais nas pontas para proteger contra a chuva!
- A janela tem `X1="21" X2="21"`, dividindo exatamente no meio ($42 / 2 = 21$) para simular duas folhas de vidro corrediças.

#### 4. A Caldeira e as Cintas de Bronze:
```xml
<!-- Tambor principal da caldeira -->
<Rectangle Width="235" Height="65" Fill="#2874A6" Stroke="#1B4F72" StrokeThickness="2" RadiusX="3" RadiusY="3">
    <Rectangle.RenderTransform>
        <TranslateTransform X="175" Y="95"/>
    </Rectangle.RenderTransform>
</Rectangle>

<!-- Cintas douradas verticais de fixação térmica -->
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
- As cintas usam a mesma altura da caldeira (`Height="65"`), transladadas exatamente para as posições $X=235$ e $X=300$, criando o visual histórico das abraçadeiras de latão polido.

#### 5. O Bloco do Cilindro e Guias da Cruzeta:
```xml
<!-- Guias horizontais (Slide Bars) onde a cruzeta desliza -->
<Line X1="0" Y1="0" X2="72" Y2="0" Stroke="{StaticResource AcoPolidoBrush}" StrokeThickness="3.5">
    <Line.RenderTransform>
        <TranslateTransform X="318" Y="200"/>
    </Line.RenderTransform>
</Line>
<Line X1="0" Y1="0" X2="72" Y2="0" Stroke="{StaticResource AcoPolidoBrush}" StrokeThickness="3.5">
    <Line.RenderTransform>
        <TranslateTransform X="318" Y="220"/>
    </Line.RenderTransform>
</Line>

<!-- Haste cromada do pistão (desenhada por baixo do cilindro) -->
<Rectangle Width="75" Height="6" Fill="{StaticResource BrancoGeloBrush}" Stroke="{StaticResource AcoEscovadoBrush}" StrokeThickness="1">
    <Rectangle.RenderTransform>
        <TranslateTransform x:Name="TranslacaoHastePistao" X="330" Y="207"/>
    </Rectangle.RenderTransform>
</Rectangle>
```
- As guias em $Y=200$ e $Y=220$ criam uma fenda de $20\text{ px}$. A haste do pistão fica centrada em $Y=207$ ($207 + 3 = 210$, altura perfeita do eixo!).

#### 6. O Farol e o Feixe de Luz Volumétrico:
```xml
<!-- Caixa do refletor -->
<Polygon Points="0,6 28,0 28,34 0,28" Fill="{StaticResource OleoCopoBrush}" Stroke="{StaticResource BordaOleoBrush}" StrokeThickness="2">
    <Polygon.RenderTransform>
        <TranslateTransform X="428" Y="102"/>
    </Polygon.RenderTransform>
</Polygon>

<!-- Feixe cônico de luz amarela transparente projetado para a frente -->
<Polygon Points="0,15 240,-35 240,65" Fill="#18FFF59D">
    <Polygon.RenderTransform>
        <TranslateTransform X="475" Y="100"/>
    </Polygon.RenderTransform>
</Polygon>
```
- A cor `#18FFF59D` possui o canal alfa definido como `0x18` (aproximadamente $9.4\%$ de opacidade). Essa transparência permite a visualização translúcida do plano de fundo e dos trilhos sob o cone de iluminação.

---

## 🔗 Referências Oficiais da Microsoft
Para aprofundamento nas classes e subsistemas gráficos do WPF:
- [Microsoft Learn — Formas e Desenho Básico no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/)
- [Microsoft Learn — Classe Rectangle (Retângulo)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle/)
- [Microsoft Learn — Classe Ellipse (Elipse e Círculo)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse/)
- [Microsoft Learn — Classe Polygon (Polígono)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon/)
- [Microsoft Learn — Classe Line (Linha Reta)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line/)
- [Microsoft Learn — Pincéis e Cores no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/wpf-brushes-overview/)
