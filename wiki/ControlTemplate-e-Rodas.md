# ControlTemplate e Parametrização Modular de Rodas e Mancais

![Módulo](https://img.shields.io/badge/M%C3%B3dulo-Templates%20Modulares-007ACC?style=flat-square)
![WPF](https://img.shields.io/badge/WPF-ControlTemplate-512BD4?style=flat-square)
![Slide 2D](https://img.shields.io/badge/Slide%202D-Padr%C3%A3o%20do%20Rel%C3%B3gio%20(258--299)-orange?style=flat-square)
![Norma](https://img.shields.io/badge/Norma-Trabalho%20C1%20(Linha%2036)-success?style=flat-square)

Neste capítulo, aborda-se a arquitetura e a geometria do `ControlTemplate` utilizado na modelagem das rodas e mancais da locomotiva, atendendo rigorosamente ao requisito normativo do [Trabalho C1](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L36) (*"A roda deve ser implementada como um modelo (Template) reutilizável para ambas as rodas da locomotiva"*) e reproduzindo o padrão de projeto demonstrado nos materiais teóricos da disciplina ([Slide 2D:258-299](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Slide/2D.md#L258-L299)).

---

## 1. Fundamentos do `ControlTemplate`: Modularização e Reúso Visual

No subsistema gráfico do WPF, o [`ControlTemplate`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/) atua como uma matriz de definição estrutural, desacoplando completamente a representação visual de um controle de suas instâncias de apresentação:

- Em vez de duplicar no código XAML dezenas de linhas de primitivas vetoriais para a Roda 1 (aro, pneu, 8 raios, contrapeso, cubo, manivela e pino excêntrico) e replicar o mesmo bloco para a Roda 2, define-se um único molde reutilizável denominado `RodaTemplate`.
- Esse modelo fica encapsulado no dicionário de recursos compartilhado ([`Resources/LocomotivaResources.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Resources/LocomotivaResources.xaml)).
- Na montagem do chassi em [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml), a aplicação declara instâncias autônomas de `<Control Template="{StaticResource RodaTemplate}"/>`, aplicando transformações afins independentes de rotação (`RotateTransform`) e translação (`TranslateTransform`) em cada elemento.

```text
      [ Matriz Estrutural: RodaTemplate ]
     (Aro + 8 Raios + Contrapeso + Manivela)
                        |
            +-----------+-----------+
            |                       |
            v                       v
    [ Instância 1 ]         [ Instância 2 ]
    Roda 1 (Traseira)       Roda 2 (Dianteira)
    Posição: (90, 170)      Posição: (230, 170)
```

### Vantagens Técnicas e Arquiteturais

1. **Eliminação de Redundância (Princípio DRY - *Don't Repeat Yourself*)**: Qualquer ajuste estético, proporcional ou estrutural efetuado no template reflete-se imediatamente em todas as rodas da composição.
2. **Consistência Geométrica Estrita**: Ambas as rodas compartilham exatamente as mesmas tolerâncias dimensionais ($80\text{ px}$), centro pivô analítico $(40,40)$ e raio de manivela ($r = 22\text{ px}$), garantindo sincronismo mecânico perfeito com as bielas.
3. **Desempenho de Renderização**: A árvore de objetos visuais compilada no dicionário de recursos é instanciada de forma otimizada pela infraestrutura do WPF, minimizando a pressão de alocação de memória na GPU.

---

## 2. Relação com o Padrão do Relógio Analógico (Slide 2D)

Nos materiais teóricos da disciplina ([Slide 2D:258-299](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Slide/2D.md#L258-L299)), o modelo de desenvolvimento para elementos rotacionais simétricos é introduzido através da construção de um relógio analógico:

- O mostrador circular estabelece o espaço canônico local.
- Os marcadores e divisores convergem simetricamente para o centro geométrico.
- Os ponteiros móveis giram em torno da origem local invariante $(x_{\text{pivô}}, y_{\text{pivô}})$.

A modelagem das rodas da locomotiva implementa essa exata metodologia:

- O aro externo e o pneu metálico delimitam o contêiner circular local de $80 \times 80\text{ px}$.
- Os 8 raios estruturais convergem rigorosamente para o centro analítico $(40,40)$.
- O braço de manivela atua como elemento excêntrico rígido, posicionando o pino de ancoragem da biela à distância radial constante de $r = 22\text{ px}$.

---

## 3. Anatomia da Roda: Parametrização em $(80 \times 80\text{ px})$

O template foi delimitado dentro de um contêiner `Canvas` com dimensões fixas de $80\text{ pixels}$ de largura por $80\text{ pixels}$ de altura. A origem canônica local $(0,0)$ coincide com o vértice superior esquerdo da caixa delimitadora (*bounding box*):

```text
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

Para que uma primitiva ou contêiner gire sobre seu próprio centro sem oscilações espúrias (*wobble*), a transformação afim de rotação ([`RotateTransform`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/)) requer a definição explícita do ponto pivô em coordenadas locais:

- As dimensões do contêiner da roda são $80 \times 80\text{ px}$.
- O centro geométrico local situa-se em:
  $$X_{\text{centro}} = \frac{80}{2} = 40.0\text{ px}, \quad Y_{\text{centro}} = \frac{80}{2} = 40.0\text{ px}$$

No arquivo XAML, a parametrização:

```xml
<RotateTransform Angle="{Binding AnguloRodas}" CenterX="40" CenterY="40"/>
```

estabelece o ponto $(40,40)$ como o pivô invariante da rotação. Como resultado, a circunferência do aro e os raios permanecem estacionários em torno de seu eixo, enquanto o pino excêntrico de manivela descreve uma órbita circular uniforme de raio $r = 22.0\text{ px}$.

---

## 5. Instanciação e Posicionamento no Chassi

Com o modelo devidamente isolado no dicionário de recursos, a composição visual em [`Controls/LocomotivaControl.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Controls/LocomotivaControl.xaml) declara as instâncias das duas rodas acopladas sob o chassi:

```xml
<!-- Roda 1 (Traseira): Posicionada em X=90, Y=170 -->
<Control Template="{StaticResource RodaTemplate}" Width="80" Height="80">
    <Control.RenderTransform>
        <TransformGroup>
            <RotateTransform Angle="{Binding AnguloRodas}" CenterX="40" CenterY="40"/>
            <TranslateTransform X="90" Y="170"/>
        </TransformGroup>
    </Control.RenderTransform>
</Control>

<!-- Roda 2 (Dianteira): Posicionada em X=230, Y=170 -->
<Control Template="{StaticResource RodaTemplate}" Width="80" Height="80">
    <Control.RenderTransform>
        <TransformGroup>
            <RotateTransform Angle="{Binding AnguloRodas}" CenterX="40" CenterY="40"/>
            <TranslateTransform X="230" Y="170"/>
        </TransformGroup>
    </Control.RenderTransform>
</Control>
```

### Coordenadas Globais dos Eixos no Canvas da Locomotiva

- **Centro da Roda 1 (Traseira)**: $X = 90 + 40 = 130.0\text{ px}, \quad Y = 170 + 40 = 210.0\text{ px}$.
- **Centro da Roda 2 (Dianteira)**: $X = 230 + 40 = 270.0\text{ px}, \quad Y = 170 + 40 = 210.0\text{ px}$.
- **Distância entre Eixos (*Wheelbase*)**:
  $$D = 270.0 - 130.0 = \mathbf{140.0\text{ pixels}}$$

A constante $D = 140.0\text{ px}$ constitui a restrição geométrica fundamental para o dimensionamento e ancoragem da biela de acoplamento (*Side Rod*).

---

## 6. Análise Detalhada do Código de `RodaTemplate` e `MancalBielaTemplate`

Abaixo é apresentada a especificação declarativa de cada componente contido em [`Resources/LocomotivaResources.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Resources/LocomotivaResources.xaml):

### 6.1 Paleta Semântica de Pincéis (Materiais Estruturais)

As cores são centralizadas em instâncias de [`SolidColorBrush`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.solidcolorbrush) identificadas por chaves semânticas:

```xml
<!-- Aços e Metais Estruturais -->
<SolidColorBrush x:Key="FerroEscuroBrush" Color="#17202A"/>     <!-- Ferro fundido estrutural -->
<SolidColorBrush x:Key="AcoTemperadoBrush" Color="#2C3E50"/>    <!-- Aço de alta resistência -->
<SolidColorBrush x:Key="AcoPolidoBrush" Color="#BDC3C7"/>       <!-- Aço usinado e polido -->
<SolidColorBrush x:Key="AcoEscovadoBrush" Color="#7F8C8D"/>      <!-- Superfície metálica fosca -->

<!-- Elementos de Sinalização e Lubrificação -->
<SolidColorBrush x:Key="VermelhoFerroviarioBrush" Color="#C0392B"/> <!-- Pinos e para-choques -->
<SolidColorBrush x:Key="OleoCopoBrush" Color="#F39C12"/>           <!-- Bronze e latão usinado -->
<SolidColorBrush x:Key="BrancoGeloBrush" Color="#ECF0F1"/>         <!-- Destaque dos raios -->
```

---

### 6.2 O Molde Completo da Roda (`RodaTemplate`)

O template estrutura as primitivas vetoriais delimitadas no contêiner de $80 \times 80\text{ px}$:

```xml
<ControlTemplate x:Key="RodaTemplate" TargetType="{x:Type Control}">
    <Canvas Width="80" Height="80">

        <!-- 1. Aro externo de aço forjado (diâmetro 80px) -->
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

        <!-- 4. Distribuição dos 8 Raios em Cruz e Diagonais -->
        <Line X1="0" Y1="0" X2="0" Y2="64" Stroke="{StaticResource BrancoGeloBrush}" StrokeThickness="3">
            <Line.RenderTransform>
                <TranslateTransform X="40" Y="8"/>
            </Line.RenderTransform>
        </Line>
        <Line X1="0" Y1="0" X2="64" Y2="0" Stroke="{StaticResource BrancoGeloBrush}" StrokeThickness="3">
            <Line.RenderTransform>
                <TranslateTransform X="8" Y="40"/>
            </Line.RenderTransform>
        </Line>
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

        <!-- 5. Cubo central da roda centrado em (40,40) -->
        <Ellipse Width="24" Height="24" Fill="#5D6D7E" Stroke="{StaticResource AcoTemperadoBrush}" StrokeThickness="2">
            <Ellipse.RenderTransform>
                <TranslateTransform X="28" Y="28"/>
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 6. Braço rígido unindo o centro (40,40) ao pino (62,40) -->
        <Rectangle Width="26" Height="12" Fill="{StaticResource AcoPolidoBrush}"
                   Stroke="{StaticResource AcoEscovadoBrush}" StrokeThickness="1.5" RadiusX="3" RadiusY="3">
            <Rectangle.RenderTransform>
                <TranslateTransform X="38" Y="34"/>
            </Rectangle.RenderTransform>
        </Rectangle>

        <!-- 7. Cabeça de fixação e pino excêntrico de manivela -->
        <Ellipse Width="16" Height="16" Fill="#5D6D7E" Stroke="{StaticResource AcoTemperadoBrush}" StrokeThickness="1.5">
            <Ellipse.RenderTransform>
                <TranslateTransform X="54" Y="32"/>
            </Ellipse.RenderTransform>
        </Ellipse>
        <Ellipse Width="8" Height="8" Fill="{StaticResource VermelhoFerroviarioBrush}" Stroke="{StaticResource BordaVermelhaBrush}" StrokeThickness="1">
            <Ellipse.RenderTransform>
                <TranslateTransform X="58" Y="36"/>
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 8. Parafuso de retenção do eixo primário -->
        <Ellipse Width="8" Height="8" Fill="{StaticResource FerroEscuroBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="36" Y="36"/>
            </Ellipse.RenderTransform>
        </Ellipse>
    </Canvas>
</ControlTemplate>
```

---

### 6.3 O Modelo do Mancal de Articulação (`MancalBielaTemplate`)

Cada terminal articular das bielas utiliza o template de mancal:

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
                <TranslateTransform X="-9" Y="-9"/>
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 3. Rolamento usinado (diâmetro 10px) -->
        <Ellipse Width="10" Height="10" Fill="{StaticResource AcoTemperadoBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="-5" Y="-5"/>
            </Ellipse.RenderTransform>
        </Ellipse>

        <!-- 4. Pino central de retenção (diâmetro 6px) -->
        <Ellipse Width="6" Height="6" Fill="{StaticResource VermelhoFerroviarioBrush}">
            <Ellipse.RenderTransform>
                <TranslateTransform X="-3" Y="-3"/>
            </Ellipse.RenderTransform>
        </Ellipse>
    </Canvas>
</ControlTemplate>
```

- **Primitiva Pontual Ancorada em $(0,0)$**: A declaração com `Width="0" Height="0"` estabelece o mancal como uma primitiva articular pontual. Todos os seus elementos internos são transladados com valores simétricos negativos (por exemplo, $-9$ para raio de $9\text{ px}$, $-5$ para raio de $5\text{ px}$). Com isso, transladar a instância diretamente para a coordenada $(X, Y)$ do pino garante ancoragem concêntrica imediata, sem necessidade de correções manuais de compensação.

---

## Referências Oficiais da Microsoft

- [Microsoft Learn — Visão Geral de Modelos de Controle (ControlTemplate)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/)
- [Microsoft Learn — Como aplicar um ControlTemplate](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/how-to-apply-a-controltemplate/)
- [Microsoft Learn — Classe RotateTransform (Rotação)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/)
- [Microsoft Learn — Classe Control (Controle Reutilizável)](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.controls.control/)
- [Microsoft Learn — Dicionários de Recursos (ResourceDictionary)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/systems/xaml-resources/)
