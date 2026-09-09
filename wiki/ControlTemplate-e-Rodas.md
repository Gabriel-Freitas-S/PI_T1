# ⚙️ ControlTemplate e Parametrização Modular de Rodas

Neste capítulo, aborda-se a arquitetura e a geometria do `ControlTemplate` utilizado na modelagem das rodas da locomotiva, atendendo ao requisito normativo do **[Trabalho C1 (Normas)](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L36)** e reproduzindo o padrão de projeto demonstrado nos materiais teóricos (**[Slide 2D:258-299](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Slide/2D.md#L258-L299)**).

---

## 1. Fundamentos do `ControlTemplate`: Modularização e Reúso Visual

No subsistema visual do WPF, o [`ControlTemplate`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/) atua como uma **matriz de definição estrutural**, desacoplando a representação gráfica de um controle de suas instâncias de apresentação:

- Em vez de replicar no código XAML mais de 20 linhas de primitivas vetoriais para a Roda 1 (aro, pneu, 8 raios, contrapeso, cubo, manivela e pino) e duplicar esse bloco para a Roda 2, constrói-se um modelo reutilizável único denominado `RodaTemplate`.
- Esse modelo fica encapsulado no dicionário de recursos compartilhado ([`Resources/LocomotivaResources.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Resources/LocomotivaResources.xaml)).
- Na montagem do chassi, a aplicação declara instâncias de controles autônomos vinculados a esse recurso (`<Control Template="{StaticResource RodaTemplate}"/>`), aplicando transformações afins independentes de translação e rotação em cada elemento.

```
      [ Matriz Estrutural: RodaTemplate ]
     (Aro + 8 Raios + Contrapeso + Manivela)
                        |
            +-----------+-----------+
            |                       |
            v                       v
    [ Instância 1 ]         [ Instância 2 ]
    Roda 1 (Traseira)       Roda 2 (Dianteira)
```

### Vantagens Técnicas e Arquiteturais:
1. **Eliminação de Redundância (Princípio DRY - *Don't Repeat Yourself*)**: Ajustes dimensionais ou refinamentos visuais são efetuados em um único ponto e refletidos imediatamente em todas as rodas da composição.
2. **Consistência Geométrica Absoluta**: Ambas as rodas compartilham rigorosamente as mesmas tolerâncias dimensionais ($80\text{ px}$), centro pivô analítico $(40,40)$ e raio de manivela ($r = 22\text{ px}$), garantindo o sincronismo mecânico com as bielas.
3. **Otimização de Recursos**: O subsistema gráfico do WPF instancia a árvore de templates compilada em memória compartilhada, minimizando a sobrecarga de renderização a 60 quadros por segundo.

---

## 2. Relação com o Padrão do Relógio Analógico (Slides Teóricos)

Nos materiais teóricos da disciplina (**[Slide 2D:258-299](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Slide/2D.md#L258-L299)**), o modelo de desenvolvimento para elementos rotacionais simétricos é estabelecido a partir da construção de um relógio analógico:
- O mostrador circular estabelece o espaço canônico local.
- Os marcadores angulares convergem de forma simétrica para o centro do círculo.
- Os ponteiros operam sobre um pivô central fixo, girando em torno da origem local.

A modelagem da roda da locomotiva implementa essa exata metodologia:
- O aro externo e o pneu delimitam o contêiner circular local de $80 \times 80\text{ px}$.
- Os 8 raios estruturais convergem rigorosamente para o centro analítico $(40,40)$.
- O braço da manivela atua como elemento rotativo excêntrico, posicionando o pino de ancoragem da biela a uma distância radial constante $r = 22\text{ px}$.

---

## 3. Anatomia da Roda: Parametrização em $(80 \times 80\text{ px})$

O template foi delimitado dentro de um contêiner `Canvas` com dimensões fixas de $80\text{ pixels}$ de largura por $80\text{ pixels}$ de altura. A origem canônica local $(0,0)$ coincide com o vértice superior esquerdo do contêiner:

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

## 4. Cinemática de Rotação Local: Definição do Centro Pivô (`CenterX` e `CenterY`)

Para que uma primitiva ou contêiner gire sobre seu próprio centro sem sofrer oscilações indesejadas (*wobble*), a transformação afim de rotação ([`RotateTransform`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/)) requer a definição explícita do ponto pivô em coordenadas locais:

- As dimensões do contêiner da roda são $80 \times 80\text{ px}$.
- O centro geométrico local situa-se exatamente em:
  $$X_{\text{centro}} = \frac{80}{2} = 40\text{ px}, \quad Y_{\text{centro}} = \frac{80}{2} = 40\text{ px}$$

No XAML, a parametrização:
```xml
<RotateTransform Angle="0" CenterX="40" CenterY="40"/>
```
estabelece o ponto $(40,40)$ como o pivô invariante de rotação. Com isso, independentemente da magnitude angular atribuída a `Angle`, a circunferência do aro permanece estacionária em torno de seu eixo, enquanto o pino excêntrico percorre uma trajetória circular uniforme de raio $r = 22\text{ px}$.

---

## 5. Instanciação e Posicionamento no Chassi

Com o modelo devidamente encapsulado no dicionário de recursos, a composição visual em [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml) declara as instâncias das duas rodas acopladas sob o chassi:

```xml
<!-- Roda 1 (Traseira): Posicionada em X=90, Y=170 -->
<Control Template="{StaticResource RodaTemplate}" Width="80" Height="80">
    <Control.RenderTransform>
        <TransformGroup>
            <RotateTransform x:Name="RotacaoRoda1" Angle="0" CenterX="40" CenterY="40"/>
            <TranslateTransform X="90" Y="170"/>
        </TransformGroup>
    </Control.RenderTransform>
</Control>

<!-- Roda 2 (Dianteira): Posicionada em X=230, Y=170 -->
<Control Template="{StaticResource RodaTemplate}" Width="80" Height="80">
    <Control.RenderTransform>
        <TransformGroup>
            <RotateTransform x:Name="RotacaoRoda2" Angle="0" CenterX="40" CenterY="40"/>
            <TranslateTransform X="230" Y="170"/>
        </TransformGroup>
    </Control.RenderTransform>
</Control>
```

### Coordenadas Globais dos Eixos no Canvas da Locomotiva:
- **Centro da Roda 1 (Traseira)**: $X = 90 + 40 = 130\text{ px}, \quad Y = 170 + 40 = 210\text{ px}$.
- **Centro da Roda 2 (Dianteira)**: $X = 230 + 40 = 270\text{ px}, \quad Y = 170 + 40 = 210\text{ px}$.
- **Distância entre Eixos (*Wheelbase*)**:
  $$D = 270 - 130 = \mathbf{140\text{ pixels}}$$

A constante $D = 140\text{ px}$ constitui a restrição geométrica fundamental para o dimensionamento e ancoragem da biela de acoplamento (*Side Rod*).

---

## 6. 📖 Dissecando o Código do `RodaTemplate` e do `MancalBielaTemplate`

Abaixo é detalhada a estrutura interna declarada em [`Resources/LocomotivaResources.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Resources/LocomotivaResources.xaml):

### 6.1 Paleta Semântica de Pincéis (Materiais Estruturais)
Para assegurar modularidade e consistência visual, as cores são abstraídas em instâncias de [`SolidColorBrush`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.solidcolorbrush) identificadas por chaves semânticas:

```xml
<!-- Aços e Metais Estruturais -->
<SolidColorBrush x:Key="FerroEscuroBrush" Color="#17202A"/>     <!-- Ferro fundido estrutural -->
<SolidColorBrush x:Key="AcoTemperadoBrush" Color="#2C3E50"/>    <!-- Aço de alta resistência -->
<SolidColorBrush x:Key="AcoPolidoBrush" Color="#BDC3C7"/>       <!-- Aço usinado e polido -->
<SolidColorBrush x:Key="AcoEscovadoBrush" Color="#7F8C8D"/>      <!-- Superfície metálica fosca -->

<!-- Elementos de Segurança Ferroviária e Lubrificação -->
<SolidColorBrush x:Key="VermelhoFerroviarioBrush" Color="#C0392B"/> <!-- Pinos e para-choques -->
<SolidColorBrush x:Key="OleoCopoBrush" Color="#F39C12"/>           <!-- Bronze e latão usinado -->
<SolidColorBrush x:Key="BrancoGeloBrush" Color="#ECF0F1"/>         <!-- Destaque dos raios -->
```

- **Centralização e Manutenibilidade**: A definição centralizada dos pincéis garante que qualquer refinamento na paleta de materiais seja realizado em uma única entrada, propagando-se automaticamente para todos os componentes do sistema.

---

### 6.2 O Molde Completo da Roda (`RodaTemplate`) Linha a Linha

Estruturação das primitivas vetoriais delimitadas no contêiner de $80 \times 80\text{ px}$:

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

        <!-- 2. Pneu intermediário metálico (rebaixo concêntrico de 5px) -->
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
- **Aro e Pneu**: Duas elipses concêntricas. O aro externo ($80\text{ px}$) possui borda reforçada (`StrokeThickness="5"`), enquanto o pneu interno ($70\text{ px}$) é transladado para $(5,5)$, assegurando alinhamento concêntrico perfeito ($5 + 70 + 5 = 80\text{ px}$).
- **Contrapeso de Balanceamento**: Polígono posicionado no setor esquerdo ($X=10$ a $38$). Como a manivela situa-se no setor direito ($X=40$ a $62$), o contrapeso compensa dinamicamente a inércia rotacional a $180^\circ$.

#### Distribuição dos 8 Raios em Cruz e Diagonais:
```xml
        <!-- Raio Vertical (eixo X=40, Y de 8 a 72) -->
        <Line X1="0" Y1="0" X2="0" Y2="64" Stroke="{StaticResource BrancoGeloBrush}" StrokeThickness="3">
            <Line.RenderTransform>
                <TranslateTransform X="40" Y="8"/>
            </Line.RenderTransform>
        </Line>

        <!-- Raio Horizontal (eixo Y=40, X de 8 a 72) -->
        <Line X1="0" Y1="0" X2="64" Y2="0" Stroke="{StaticResource BrancoGeloBrush}" StrokeThickness="3">
            <Line.RenderTransform>
                <TranslateTransform X="8" Y="40"/>
            </Line.RenderTransform>
        </Line>

        <!-- Raios Diagonais cruzando o centro (40,40) -->
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
- Cada um dos 4 segmentos de linha intersecta com exatidão o ponto central $(40,40)$, reproduzindo os raios forjados de rodas motrizes reais.

#### Braço de Manivela e Pino Excêntrico:
```xml
        <!-- Cubo central da roda centrado em (40,40) -->
        <Ellipse Width="24" Height="24" Fill="#5D6D7E" Stroke="{StaticResource AcoTemperadoBrush}" StrokeThickness="2">
            <Ellipse.RenderTransform>
                <TranslateTransform X="28" Y="28"/>  <!-- Centro: 28 + 12 = 40 -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- Braço rígido de aço unindo o centro (40,40) ao pino (62,40) -->
        <Rectangle Width="26" Height="12" Fill="{StaticResource AcoPolidoBrush}"
                   Stroke="{StaticResource AcoEscovadoBrush}" StrokeThickness="1.5" RadiusX="3" RadiusY="3">
            <Rectangle.RenderTransform>
                <TranslateTransform X="38" Y="34"/>  <!-- Eixo Y: 34 + 6 = 40 -->
            </Rectangle.RenderTransform>
        </Rectangle>

        <!-- Cabeça de fixação do pino de manivela -->
        <Ellipse Width="16" Height="16" Fill="#5D6D7E" Stroke="{StaticResource AcoTemperadoBrush}" StrokeThickness="1.5">
            <Ellipse.RenderTransform>
                <TranslateTransform X="54" Y="32"/>  <!-- Centro: 54 + 8 = 62, 32 + 8 = 40 -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- Pino Excêntrico de Tração (ancoragem da biela) -->
        <Ellipse Width="8" Height="8" Fill="{StaticResource VermelhoFerroviarioBrush}" Stroke="{StaticResource BordaVermelhaBrush}" StrokeThickness="1">
            <Ellipse.RenderTransform>
                <TranslateTransform X="58" Y="36"/>  <!-- Centro: 58 + 4 = 62, 36 + 4 = 40 -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- Parafuso central do eixo primário -->
        <Ellipse Width="8" Height="8" Fill="{StaticResource FerroEscuroBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="36" Y="36"/>  <!-- Centro: 36 + 4 = 40 -->
            </Ellipse.RenderTransform>
        </Ellipse>
    </Canvas>
</ControlTemplate>
```
- O parafuso central localiza-se em $(40,40)$ e o pino de tração em $(62,40)$. A distância radial resultante é de $r = 62 - 40 = 22\text{ px}$, constituindo o parâmetro `RaioManivela` utilizado nas equações cinemáticas.

---

### 6.3 O Modelo do Mancal (`MancalBielaTemplate`)

Cada terminal articular de biela encapsula quatro elementos mecânicos padronizados:

```xml
<ControlTemplate x:Key="MancalBielaTemplate" TargetType="{x:Type Control}">
    <Canvas Width="0" Height="0">
        <!-- 1. Copo de alimentação de óleo lubrificante -->
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
                <TranslateTransform X="-9" Y="-9"/>  <!-- Ancoragem centrada em (0,0) -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 3. Rolamento de rolagem usinado (diâmetro 10px) -->
        <Ellipse Width="10" Height="10" Fill="{StaticResource AcoTemperadoBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="-5" Y="-5"/>  <!-- Ancoragem centrada em (0,0) -->
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 4. Pino central de retenção (diâmetro 6px) -->
        <Ellipse Width="6" Height="6" Fill="{StaticResource VermelhoFerroviarioBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="-3" Y="-3"/>  <!-- Ancoragem centrada em (0,0) -->
            </Ellipse.RenderTransform>
        </Ellipse>
    </Canvas>
</ControlTemplate>
```
- `Width="0" Height="0"`: A definição dimensional nula estabelece o mancal como uma **primitiva articular pontual**. Toda a geometria é distribuída simetricamente em torno de sua origem $(0,0)$. Dessa forma, para posicionar o mancal sobre qualquer coordenada de pino, basta transladar o controle diretamente para a posição calculada, sem necessidade de compensações dimensionais manuais.

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Visão Geral de Modelos de Controle (ControlTemplate)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/)
- [Microsoft Learn — Como aplicar um ControlTemplate](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/how-to-apply-a-controltemplate/)
- [Microsoft Learn — Classe RotateTransform (Rotação)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/)
- [Microsoft Learn — Classe Control (Controle Reutilizável)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.controls.control/)
- [Microsoft Learn — Dicionários de Recursos (ResourceDictionary)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/systems/xaml-resources/)
