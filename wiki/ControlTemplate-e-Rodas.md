# ⚙️ ControlTemplate e Parametrização de Rodas: A "Forma de Bolo" do Trem

Neste capítulo, explicamos de maneira simples e intuitiva o que é um `ControlTemplate` no WPF, por que ele foi exigido nas normas do trabalho (**[Trabalho C1 (Normas)](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L36)**) e como ele se baseia diretamente no exemplo do **relógio analógico** ensinado em sala de aula (**[Slide 2D:258-299](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Slide/2D.md#L258-L299)**).

---

## 1. O que é um `ControlTemplate`? (A Analogia do Molde de Silicone / Carimbo)

Imagine que você está na cozinha fazendo biscoitos ou bolos:
- Se você tentar esculpir à mão cada biscoito do zero, vai demorar o dobro do tempo, e um sempre vai ficar ligeiramente maior, mais torto ou diferente do outro.
- O que uma pessoa esperta faz? Ela compra um **molde de corte** (ou um carimbo). Ela desenha o molde uma única vez com perfeição e depois só vai carimbando a massa!

No WPF, o [`ControlTemplate`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/) é exatamente essa **forma de corte**:
- Em vez de escrever no código XAML mais de 20 linhas de desenho para a Roda 1 (aro, pneu, 8 raios, contrapeso, cubo, manivela e pino) e depois **copiar e colar** tudo de novo para a Roda 2, nós criamos um molde reutilizável chamado `RodaTemplate`.
- Esse molde fica guardado na nossa "gaveta de utensílios" (o arquivo [`Resources/LocomotivaResources.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Resources/LocomotivaResources.xaml)).
- Depois, na locomotiva, a gente só chama `<Control Template="{StaticResource RodaTemplate}"/>` duas vezes: uma para a **Roda Traseira** e outra para a **Roda Dianteira**!

```
      [ Molde: RodaTemplate ]
     (Aro + 8 Raios + Manivela)
                 |
      +----------+----------+
      |                     |
      v                     v
[ Carimbo 1 ]         [ Carimbo 2 ]
Roda 1 (Traseira)     Roda 2 (Dianteira)
```

### Por que isso é incrível?
1. **Zero Código Repetido (Regra DRY - *Don't Repeat Yourself*)**: Se você quiser mudar a cor do aro de cinza para dourado, você muda em um só lugar no molde, e todas as rodas do trem mudam automaticamente.
2. **Gêmeas Perfeitas**: Ambas as rodas têm exatamente o mesmo tamanho ($80\text{ px}$), o mesmo centro e a mesma manivela ($r = 22\text{ px}$). Isso é vital para que as bielas de aço não entortem durante o movimento.
3. **Economia de Memória do Computador**: A placa de vídeo reaproveita a mesma receita de desenho, deixando a animação rodando a 60 quadros por segundo sem engasgar.

---

## 2. A Ideia do Relógio Analógico dos Slides de Aula

No slide da disciplina de Processamento de Imagens, o professor ensinou como fazer um relógio de parede:
- Um mostrador circular.
- Marcadores de horas apontando para os ângulos de $0^\circ, 30^\circ, 60^\circ, 90^\circ...$
- Ponteiros que giram presos no centro.

Uma **roda de locomotiva a vapor histórica** funciona sob o mesmo princípio geométrico:
- O aro da roda é o mostrador do relógio.
- Os 8 raios de aço são como ponteiros estáticos apontando para as horas ($12\text{h}, 3\text{h}, 6\text{h}, 9\text{h}$ e as quatro diagonais).
- A manivela do pistão é como o ponteiro dos minutos que gira e carrega na sua ponta o pino de tração!

---

## 3. Anatomia da Roda: Desenhada na Caixinha $(80 \times 80\text{ px})$

Nosso molde foi desenhado dentro de uma caixinha neutra de $80\text{ pixels}$ de largura por $80\text{ pixels}$ de altura. A origem local $(0,0)$ fica no topo esquerdo do molde:

```
          (0,0) +-------------------------------+ (80,0)
                |          [ARO EXTERNO]        |
                |         /             \       |
                |       /    CONTRAPESO   \     |
                |      |       [===]       |    |
                |      |  RAIO   O   MANIVELA===O (62,40) [PINO EXCÊNTRICO]
                |      |      (40,40)      |    |
                |       \    (Centro)   /     |
                |         \             /       |
         (0,80) +-------------------------------+ (80,80)
```

---

## 4. O Segredo do Giro Perfeito: A "Tachinha no Centro"

Como fazer a roda girar no computador sem ficar "rebolando" ou saindo do lugar?  
Usamos a ferramenta [`RotateTransform`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/).

Imagine que você recortou uma roda de papelão de $8\text{ cm} \times 8\text{ cm}$ e quer prender ela na parede com uma tachinha para ela girar:
- Se você espetar a tachinha no canto da folha $(0,0)$, ao girar, a folha vai fazer um círculo enorme e desengonçado pela parede.
- Para a roda girar certinha sobre o próprio eixo, **onde você deve espetar a tachinha?** Exatamente no centro da folha!
- Se a folha tem $80\text{ px}$ de largura e $80\text{ px}$ de altura, a metade exata é:
  $$X = \frac{80}{2} = 40, \quad Y = \frac{80}{2} = 40$$

É exatamente por isso que no código XAML escrevemos:
```xml
<RotateTransform Angle="0" CenterX="40" CenterY="40"/>
```
O `CenterX="40"` e `CenterY="40"` são a "tachinha" espetada no centro do molde! Assim, não importa quantos graus a roda gire, o aro não sai do lugar e o pino orbita macio e perfeito.

---

## 5. Colocando as Duas Rodas na Locomotiva

Com o molde pronto, só precisamos colar dois carimbos embaixo do chassi da locomotiva:

```xml
<!-- Roda 1 (Traseira): Colada em X=90, Y=170 -->
<Control Template="{StaticResource RodaTemplate}" Width="80" Height="80">
    <Control.RenderTransform>
        <TransformGroup>
            <RotateTransform x:Name="RotacaoRoda1" Angle="0" CenterX="40" CenterY="40"/>
            <TranslateTransform X="90" Y="170"/>
        </TransformGroup>
    </Control.RenderTransform>
</Control>

<!-- Roda 2 (Dianteira): Colada em X=230, Y=170 -->
<Control Template="{StaticResource RodaTemplate}" Width="80" Height="80">
    <Control.RenderTransform>
        <TransformGroup>
            <RotateTransform x:Name="RotacaoRoda2" Angle="0" CenterX="40" CenterY="40"/>
            <TranslateTransform X="230" Y="170"/>
        </TransformGroup>
    </Control.RenderTransform>
</Control>
```

### Onde Ficam os Centros no Trem?
- **Centro da Roda 1 (Traseira)**: $X = 90 + 40 = 130\text{ px}, \quad Y = 170 + 40 = 210\text{ px}$.
- **Centro da Roda 2 (Dianteira)**: $X = 230 + 40 = 270\text{ px}, \quad Y = 170 + 40 = 210\text{ px}$.
- **Distância entre as duas rodas**:
  $$270 - 130 = \mathbf{140\text{ pixels}}$$

Guardem esse número mágico **$140\text{ px}$**! É exatamente a distância entre os eixos que usamos para conectar a biela de acoplamento (*Side Rod*).

---

## 6. 📖 Dissecando o Código do `RodaTemplate` e do `MancalBielaTemplate`

Vamos agora analisar linha por linha o código real dentro de [`Resources/LocomotivaResources.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Resources/LocomotivaResources.xaml)!

### 6.1 A Paleta Semântica de Pincéis (Materiais da Locomotiva)
Em vez de espalhar códigos de cores como `#BDC3C7` aleatoriamente pelo projeto, nós definimos pincéis nomeados pelo seu material real:

```xml
<!-- Aços e Metais Estruturais -->
<SolidColorBrush x:Key="FerroEscuroBrush" Color="#17202A"/>     <!-- Ferro fundido pesado -->
<SolidColorBrush x:Key="AcoTemperadoBrush" Color="#2C3E50"/>    <!-- Aço azulado resistente -->
<SolidColorBrush x:Key="AcoPolidoBrush" Color="#BDC3C7"/>       <!-- Aço brilhante usinado -->
<SolidColorBrush x:Key="AcoEscovadoBrush" Color="#7F8C8D"/>      <!-- Metal fosco -->

<!-- Cores de Alerta Ferroviário e Lubrificação -->
<SolidColorBrush x:Key="VermelhoFerroviarioBrush" Color="#C0392B"/> <!-- Pinos e para-choques -->
<SolidColorBrush x:Key="OleoCopoBrush" Color="#F39C12"/>           <!-- Bronze/Latão dourado -->
<SolidColorBrush x:Key="BrancoGeloBrush" Color="#ECF0F1"/>         <!-- Raios destacados -->
```
- **Por que fazer isso?** Se o professor pedir para trocar a cor do aço polido por um metal mais escuro, mudamos apenas a linha do `AcoPolidoBrush` e todas as bielas e aros do trem atualizam juntas na hora!

---

### 6.2 O Molde Completo da Roda (`RodaTemplate`) Linha a Linha

Veja como o `RodaTemplate` foi montado dentro do `Canvas` de $80 \times 80\text{ px}$:

```xml
<ControlTemplate x:Key="RodaTemplate" TargetType="{x:Type Control}">
    <Canvas Width="80" Height="80">

        <!-- 1. Aro externo de aço forjado -->
        <Ellipse Width="80" Height="80" Stroke="{StaticResource FerroEscuroBrush}"
                 StrokeThickness="5" Fill="#212F3D">
            <Ellipse.RenderTransform>
                <TranslateTransform X="0" Y="0"/>
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 2. Pneu intermediário metálico (rebaixo de 5px em cada borda) -->
        <Ellipse Width="70" Height="70" Stroke="{StaticResource AcoEscovadoBrush}"
                 StrokeThickness="2" Fill="{StaticResource AcoTemperadoBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="5" Y="5"/>
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 3. Contrapeso em meia-lua (balanceamento dinâmico a 180° da manivela) -->
        <Polygon Points="10,24 38,24 38,56 10,56" Fill="{StaticResource FerroEscuroBrush}">
            <Polygon.RenderTransform>
                <TranslateTransform X="0" Y="0"/>
            </Polygon.RenderTransform>
        </Polygon>
```
- **Aro Externo**: Círculo de diâmetro $80\text{ px}$ com borda grossa (`StrokeThickness="5"`).
- **Pneu Interno**: Círculo de diâmetro $70\text{ px}$ transladado para $(5,5)$ ($5 + 70 + 5 = 80\text{ px}$, perfeitamente concêntrico!).
- **Contrapeso**: Polígono denso posicionado no quadrante esquerdo ($X=10$ a $38$). Como a manivela fica no quadrante direito ($X=40$ a $62$), o contrapeso equilibra o peso físico exatamente a $180^\circ$!

#### Os 8 Raios em Cruz e Diagonais:
```xml
        <!-- Raio Vertical (passa por X=40 de Y=8 até Y=72) -->
        <Line X1="0" Y1="0" X2="0" Y2="64" Stroke="{StaticResource BrancoGeloBrush}" StrokeThickness="3">
            <Line.RenderTransform>
                <TranslateTransform X="40" Y="8"/>
            </Line.RenderTransform>
        </Line>

        <!-- Raio Horizontal (passa por Y=40 de X=8 até X=72) -->
        <Line X1="0" Y1="0" X2="64" Y2="0" Stroke="{StaticResource BrancoGeloBrush}" StrokeThickness="3">
            <Line.RenderTransform>
                <TranslateTransform X="8" Y="40"/>
            </Line.RenderTransform>
        </Line>

        <!-- Raios Diagonais em X cruzando o centro (40,40) -->
        <Line X1="0" Y1="0" X2="46" Y2="46" Stroke="{StaticResource AcoPolidoBrush}" StrokeThickness="3">
            <Line.RenderTransform>
                <TranslateTransform X="17" Y="17"/>
            </Line.RenderTransform>
        </Line>
        <Line X1="0" Y1="46" X2="46" Y2="0" Stroke="{StaticResource AcoPolidoBrush}" StrokeThickness="3">
            <Line.RenderTransform>
                <TranslateTransform X="17" Y="17"/>
            </Line.RenderTransform>
        </Line>
```
- Todos os 4 segmentos de reta passam exatamente pelas coordenadas $(40,40)$.  
  Por exemplo, a diagonal começa em $(17,17)$ e tem tamanho $46 \times 46$, passando por $(17 + 23, 17 + 23) = (40,40)$!

#### O Braço da Manivela e o Pino Excêntrico:
```xml
        <!-- Cubo central da roda em (40,40) -->
        <Ellipse Width="24" Height="24" Fill="#5D6D7E" Stroke="{StaticResource AcoTemperadoBrush}" StrokeThickness="2">
            <Ellipse.RenderTransform>
                <TranslateTransform X="28" Y="28"/>  <!-- 28 + 12 = 40 (Centro exato!) -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- Braço sólido de aço ligando o centro (40,40) ao pino (62,40) -->
        <Rectangle Width="26" Height="12" Fill="{StaticResource AcoPolidoBrush}"
                   Stroke="{StaticResource AcoEscovadoBrush}" StrokeThickness="1.5" RadiusX="3" RadiusY="3">
            <Rectangle.RenderTransform>
                <TranslateTransform X="38" Y="34"/>  <!-- Centro Y: 34 + 6 = 40 -->
            </Rectangle.RenderTransform>
        </Rectangle>

        <!-- Cabeça externa onde o pino se fixa -->
        <Ellipse Width="16" Height="16" Fill="#5D6D7E" Stroke="{StaticResource AcoTemperadoBrush}" StrokeThickness="1.5">
            <Ellipse.RenderTransform>
                <TranslateTransform X="54" Y="32"/>  <!-- Centro: 54 + 8 = 62, 32 + 8 = 40 -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- Pino Excêntrico Vermelho (onde a biela encaixa) -->
        <Ellipse Width="8" Height="8" Fill="{StaticResource VermelhoFerroviarioBrush}" Stroke="{StaticResource BordaVermelhaBrush}" StrokeThickness="1">
            <Ellipse.RenderTransform>
                <TranslateTransform X="58" Y="36"/>  <!-- Centro: 58 + 4 = 62, 36 + 4 = 40 -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- Parafuso central do eixo principal -->
        <Ellipse Width="8" Height="8" Fill="{StaticResource FerroEscuroBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="36" Y="36"/>  <!-- Centro: 36 + 4 = 40, 36 + 4 = 40 -->
            </Ellipse.RenderTransform>
        </Ellipse>
    </Canvas>
</ControlTemplate>
```
- Observe o alinhamento trigonométrico: o parafuso central fica centrado em $(40,40)$ e o pino vermelho em $(62,40)$. A distância é de $62 - 40 = 22\text{ px}$. Esse é o raio exato de manivela ($r = 22\text{ px}$) usado no motor físico!

---

### 6.3 O Molde do Mancal (`MancalBielaTemplate`)

Cada olhal de biela possui 4 elementos mecânicos modelados com precisão:

```xml
<ControlTemplate x:Key="MancalBielaTemplate" TargetType="{x:Type Control}">
    <Canvas Width="0" Height="0">
        <!-- 1. Copo superior de abastecimento de óleo lubrificante -->
        <Rectangle Width="4" Height="4" Fill="{StaticResource OleoCopoBrush}"
                   Stroke="{StaticResource BordaOleoBrush}" StrokeThickness="0.8">
            <Rectangle.RenderTransform>
                <TranslateTransform X="-2" Y="-11"/>
            </Rectangle.RenderTransform>
        </Rectangle>

        <!-- 2. Bucha externa de bronze forjado (diâmetro 18px) -->
        <Ellipse Width="18" Height="18" Fill="{StaticResource OleoCopoBrush}"
                 Stroke="{StaticResource BordaOleoBrush}" StrokeThickness="1.5">
            <Ellipse.RenderTransform>
                <TranslateTransform X="-9" Y="-9"/>  <!-- Centrado em (0,0) -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 3. Rolamento interno de aço (diâmetro 10px) -->
        <Ellipse Width="10" Height="10" Fill="{StaticResource AcoTemperadoBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="-5" Y="-5"/>  <!-- Centrado em (0,0) -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 4. Pino excêntrico vermelho central (diâmetro 6px) -->
        <Ellipse Width="6" Height="6" Fill="{StaticResource VermelhoFerroviarioBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="-3" Y="-3"/>  <!-- Centrado em (0,0) -->
            </Ellipse.RenderTransform>
        </Ellipse>
    </Canvas>
</ControlTemplate>
```
- `Width="0" Height="0"`: Definir a largura e altura do canvas como zero garante que o mancal atua como uma **primitiva pontual**. Toda a geometria é desenhada simetricamente ao redor de sua própria origem $(0,0)$ (com translações como `X="-9" Y="-9"` para o círculo de raio 9).
- Isso significa que, para posicionar o mancal em cima de qualquer pino, basta transladar o controle para a coordenada exata daquele pino, sem somar nem subtrair nenhum deslocamento manual!

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Visão Geral de Modelos de Controle (ControlTemplate)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/)
- [Microsoft Learn — Como aplicar um ControlTemplate](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/how-to-apply-a-controltemplate/)
- [Microsoft Learn — Classe RotateTransform (Rotação)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/)
- [Microsoft Learn — Classe Control (Controle Reutilizável)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.controls.control/)
- [Microsoft Learn — Dicionários de Recursos (ResourceDictionary)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/systems/xaml-resources/)
