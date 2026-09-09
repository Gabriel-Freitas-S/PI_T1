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

### As 4 Ferramentas Básicas de Desenho do WPF

Para não precisar de imagens pesadas da internet, desenhamos tudo com **cálculos matemáticos vetoriais puros**. O WPF nos dá quatro "canetas" mágicas derivadas da classe [`Shape`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/):

1. [`Rectangle`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle) (**Retângulos e Quadrados**): Nossos blocos de construção tipo Lego. Usamos para o chassi comprido, as caixas de água laterais, a cabine do maquinista e as hastes de aço. Podemos até arredondar as bordas (`RadiusX` e `RadiusY`) para parecer metal polido.
2. [`Ellipse`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse) (**Círculos e Elipses**): Usadas para as rodas do trem, rebites, os miolos das bielas, a cúpula do domo de vapor e as bolhas de fumaça que saem da chaminé.
3. [`Polygon`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon) (**Polígonos com Vários Lados**): Funciona como um jogo de "ligar os pontos". Usamos quando a peça tem formato inclinado especial, como o teto curvado da cabine, o limpa-trilhos triangular da frente e o feixe amarelo brilhante do farol.
4. [`Line`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line) (**Linhas Retas**): Fios de aço de alta precisão. Usamos para os 8 raios que sustentam cada roda, os corrimãos de subida do maquinista e os trilhos horizontais onde o pistão desliza.

---

## 2. O Cenário: Céu da Noite e a Linha Férrea

Antes de colocar o trem no mundo, precisamos de um chão firme e de um céu bonito para compor a atmosfera.

### 2.1 O Céu Noturno com Gradiente
Em vez de um fundo preto chapado e sem graça, usamos um [`LinearGradientBrush`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.lineargradientbrush). Ele mistura quatro tons de azul e cinza escuro de cima para baixo, criando a ilusão de uma noite fresca e enluarada no pátio ferroviário:
- Topo: Azul noite profundo (`#0B1319`).
- Meio: Azul ardósia industrial (`#284659`).
- Base: Carvão escuro (`#17202A`).

### 2.2 O Trilho e a Matemática do Contato Perfeito
Um erro muito comum em jogos e animações é a roda "afundar" no trilho ou "flutuar no ar". Para evitar isso, fizemos uma conta exata:
- O topo do trilho prateado foi cravado na altura $Y = 405\text{ px}$.
- A locomotiva fica na altura global $Y = 155\text{ px}$.
- O centro do eixo da roda fica a $210\text{ px}$ do topo da locomotiva.
- O raio da roda é de exatamente $40\text{ px}$ (diâmetro de $80\text{ px}$).

```math
Y_{\text{base da roda}} = 155\text{ (altura do trem)} + 210\text{ (eixo)} + 40\text{ (raio)} = \mathbf{405\text{ px}}
```

Como $405 = 405$, a roda toca o trilho com **tangência matemática perfeita**! Não sobra nem um milímetro de ar nem entra no aço.

---

## 3. Decomposição Estrutural da Locomotiva (0-4-0T)

Nossa locomotiva é do modelo histórico **0-4-0T (Tank Engine)**. O que significa esse código estranho?
- **0**: Nenhuma roda pequena guia na frente.
- **4**: Quatro rodas motrizes grandes acopladas que puxam o peso (duas de cada lado).
- **0**: Nenhuma roda de apoio na traseira.
- **T (*Tank*)**: Ela carrega a água em tanques nas suas próprias laterais e o carvão em um compartimento atrás da cabine. Ou seja, ela é compacta e não precisa de um vagão separado (*tender*) puxado atrás dela!

### Diagrama Estrutural da Locomotiva (Frente Voltada para a Direita $\to$)

Abaixo está o mapa esquemático da locomotiva no mesmo sentido em que ela foi programada no código (da traseira à esquerda para a frente à direita):

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

### O Que É Cada Peça? (Explicado em Detalhes)

#### Seção 1: Chassi Inferior e Para-Choque Traseiro (Extrema Esquerda)
- **Viga Principal do Chassi**: Uma longa barra de aço carbono grafite (`#1C2833`) de $500\text{ px}$ que sustenta todo o peso da máquina. Fica em $(15, 190)$.
- **Para-choque de Madeira e Prato Amortecedor Traseiro**: Uma viga vermelha em $(8, 188)$ com um prato circular em $(2, 192)$ para amortecer impactos caso outro vagão engate atrás.

#### Seção 2: Para-Choque Dianteiro e Limpa-Trilhos (*Cowcatcher*) (Extrema Direita)
- **Viga Dianteira (*Buffer Beam*)**: Bloco vermelho vibrante em $(496, 187)$.
- **Gancho e Elo de Corrente**: Engate ferroviário forjado em $(506, 203)$ para puxar composições.
- **Limpa-Trilhos em Cunha (*Cowcatcher*)**: Aquela grade triangular clássica na frente do trem em $(496, 208)$. Sua função histórica é empurrar pedras, galhos ou animais para fora da linha férrea, impedindo descarrilamentos. Possui faixas amarelas de alerta visual.

#### Seção 3: Depósito Traseiro de Carvão (*Bunker*)
- Fica logo atrás da cabine em $(20, 95)$. É um cofre metálico cheio de pedras de carvão mineral texturizadas (`Polygon`). O foguista pega o carvão dali para alimentar a fornalha.
- Inclui uma lanterna de cauda com luz vermelha em $(12, 110)$ para segurança na via.

#### Seção 4: Cabine do Maquinista
- O abrigo da tripulação em azul prussiano industrial (`#21618C`), posicionado em $(55, 65)$.
- **Teto Protetor Curvo**: Evita que a chuva e as fagulhas entrem na cabine.
- **Porta com Maçaneta Dourada**: Em $(62, 95)$, por onde o maquinista entra.
- **Estribos de Subida**: Escada de ferro de dois degraus em $(66, 192)$, posicionada estrategicamente bem embaixo da porta.
- **Janela Panorâmica**: Janela com vidro azul claro e moldura reforçada em $(112, 88)$ para enxergar os sinais da via.

#### Seção 5: Caldeira Cilíndrica e Cintas de Bronze
- O grande "tanque" horizontal de alta pressão em $(175, 95)$. É onde a água ferve a centenas de graus para gerar o vapor comprimido.
- **Cintas de Fixação**: Fitas douradas verticais em $(235, 95)$ e $(300, 95)$. Na vida real, elas prendem uma camada isolante térmica de madeira e amianto para que a caldeira não perca calor.

#### Seção 6: Tanques Laterais de Água (*Side Tanks*)
- Caixas retangulares montadas dos lados da caldeira em $(175, 135)$. Armazenam a água fria que abastece continuamente a caldeira através de válvulas no topo.

#### Seção 7: Domo de Vapor, Sino, Chaminé e Farol
- **Domo de Vapor**: Cúpula arredondada em $(220, 70)$. Fica no ponto mais alto da caldeira para capturar apenas o "vapor seco" (sem respingos de água fervente) e mandá-lo para os pistões.
- **Sino de Latão**: Em $(295, 80)$, usado para alertar pedestres e trabalhadores nas estações.
- **Chaminé Cônica**: Tubo em $(383, 46)$ por onde sai a fumaça da fornalha e o vapor gasto do motor.
- **Farol de Proa e Feixe de Luz**: Uma grande lanterna em $(428, 102)$ com lente amarela que projeta um cone de luz brilhante e translúcido para a frente na escuridão.

#### Seção 8: Bloco do Cilindro de Vapor e Guias da Cruzeta
- **Bloco do Cilindro**: A "caixa de força" em ferro fundido cinza em $(390, 192)$. É onde o vapor entra com força absurda para empurrar o êmbolo.
- **Guias da Cruzeta (*Slide Bars*)**: Dois trilhos horizontais cromados em $Y = 200$ e $Y = 220$. Eles forçam a cruzeta a andar rigorosamente em linha reta, sem desviar para cima nem para baixo.

---

## 4. Camadas Visuais (Z-Index): A Analogia das Folhas de Celofane

Como o computador sabe o que desenhar na frente e o que desenhar atrás?  
Imagine que estamos montando um quadro colando **folhas transparentes de acetato** uma em cima da outra. Quem for desenhado primeiro fica no fundo; quem for desenhado por último fica por cima de tudo:

```
[Camada 11]  Fumaça e Vapor saindo da chaminé       (Na frente de tudo)
[Camada 10]  Bloco de Ferro do Cilindro             (Tapa a ponta da haste)
[Camada 9]   Cruzeta e Pino Frontal                 (Segura a haste e a biela)
[Camada 8]   Biela Motriz Inclinada                 (Encaixa atrás da cruzeta)
[Camada 7]   Biela de Acoplamento Horizontal        (Une as duas rodas)
[Camada 6]   Rodas da Locomotiva com Raios          (Giram com as manivelas)
[Camada 5]   Haste Cromada do Pistão                (Entra e sai do cilindro)
[Camada 4]   Guias da Cruzeta (Slide Bars)          (Trilhos horizontais)
[Camada 3]   Corpo da Locomotiva                    (Caldeira, cabine, chassi)
[Camada 2]   Trilhos e Lastro de Brita              (Onde as rodas apoiam)
[Camada 1]   Céu Noturno com Estrelas               (Fundo estático)
```

### O Segredo da Ilusão do Pistão
Observe a mágica visual entre as **Camadas 5, 9 e 10**:
1. A haste do pistão é uma barra de metal de $75\text{ px}$.
2. A cruzeta (Camada 9) agarra a ponta esquerda da haste.
3. O bloco do cilindro (Camada 10) é desenhado **por cima da ponta direita da haste**.
4. Conforme o trem anda, a haste vai para a frente e para trás. Quando ela vai para a frente, ela entra debaixo do desenho do cilindro, ficando invisível! Quando volta, ela reaparece.  
Isso cria a ilusão mecânica perfeita de que o pistão está realmente penetrando no motor a vapor, sem precisar recortar a imagem ou fazer cálculos complicados de corte 3D!

---

## 5. 📖 Análise Linha a Linha do Código XAML

Agora vamos olhar os arquivos reais do projeto e entender exatamente cada linha escrita!

### 5.1 O Cenário e os Trilhos em `MainWindow.xaml`

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
- A cor `#18FFF59D` possui apenas `18` no canal alfa (transparência de cerca de $10\%$). Isso permite enxergar o céu da noite por trás da luz, criando uma atmosfera cinematográfica linda!

---

## 🔗 Referências Oficiais da Microsoft
Para quem quiser conferir como essas funções existem oficialmente na linguagem:
- [Microsoft Learn — Formas e Desenho Básico no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/)
- [Microsoft Learn — Classe Rectangle (Retângulo)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.rectangle/)
- [Microsoft Learn — Classe Ellipse (Elipse e Círculo)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse/)
- [Microsoft Learn — Classe Polygon (Polígono)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.polygon/)
- [Microsoft Learn — Classe Line (Linha Reta)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.line/)
- [Microsoft Learn — Pincéis e Cores no WPF](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/wpf-brushes-overview/)
