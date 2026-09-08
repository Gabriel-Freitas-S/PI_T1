# ⚙️ ControlTemplate e Parametrização de Rodas

Neste capítulo, detalha-se a arquitetura e a geometria do `ControlTemplate` utilizado para modelar as rodas da locomotiva, atendendo ao requisito normativo do **[Trabalho C1 (Normas)](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Trabalho/Trabalho%20C1.md#L36)** e reproduzindo o padrão de projeto ensinado em sala de aula (**[Slide 2D:258-299](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Slide/2D.md#L258-L299)**).

---

## 1. Por que utilizar `ControlTemplate`?

No desenvolvimento de interfaces em WPF, o [`ControlTemplate`](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/) permite desacoplar completamente a **estrutura visual** de um controle da sua **lógica comportamental**.

Nos slides da disciplina (padrão do relógio analógico), o mostrador e os marcadores são declarados dentro de `<ControlTemplate>` nos recursos (`Resources`). No projeto, seguindo as melhores práticas de modularização e separação de responsabilidades (SRP), o `RodaTemplate` foi isolado em seu próprio dicionário de recursos: [`Resources/LocomotivaResources.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Resources/LocomotivaResources.xaml), mesclado globalmente em `App.xaml`. Em seguida, a janela simplesmente declara instâncias de `<Control Template="{StaticResource RodaTemplate}"/>`, aplicando transformações independentes de translação e rotação.

### Vantagens Técnicas:
1. **DRY (Don't Repeat Yourself)**: Os mais de 10 elementos que compõem uma roda completa (aros, raios, contrapeso, braço de manivela e pinos) são escritos uma única vez no código XAML.
2. **Modularização e Baixo Acoplamento**: O arquivo XAML principal (`MainWindow.xaml`) fica limpo e desacoplado dos detalhes internos da roda, que ficam confinados no `ResourceDictionary`.
3. **Coerência Mecânica Absoluta**: Todas as rodas da locomotiva compartilham rigorosamente as mesmas tolerâncias dimensionais, raios de manivela ($r = 22\text{ px}$) e centro geométrico ($(40,40)$).
4. **Desempenho de Memória**: O WPF compartilha a mesma árvore de recursos compilada em tempo de execução para todas as instâncias do controle.

---

## 2. Anatomia Geométrica do `RodaTemplate`

O template é delimitado por um contêiner `Canvas` com dimensões fixas de $80 \times 80\text{ px}$. A origem canônica local é $(0,0)$ e o centro analítico de rotação é o ponto $(40,40)$.

```
            (0,0) +-------------------------------+ (80,0)
                  |          [ARO EXTERNO]        |
                  |         /             \       |
                  |       /    CONTRAPESO   \     |
                  |      |       [===]       |    |
                  |      |  RAIO   O   MANIVELA===O (62,40) [PINO]
                  |      |      (40,40)      |    |
                  |       \                 /     |
                  |         \             /       |
           (0,80) +-------------------------------+ (80,80)
```

Abaixo está o detalhamento de cada elemento interno do template:

### 2.1 Aro Externo e Pneu Intermediário
- **Aro Externo**: [`Ellipse`](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.shapes.ellipse) com $80 \times 80\text{ px}$ de diâmetro, preenchida em cinza escuro forjado (`#212F3D`) com borda espessa de $5\text{ px}$ (`#17202A`).
- **Pneu de Rodagem**: `Ellipse` com $70 \times 70\text{ px}$ transladada para $(5,5)$, simulando o ressalto metálico que apoia no trilho.

### 2.2 Contrapeso de Balanceamento Dinâmico (*Counterweight*)
Em locomotivas reais, as bielas e os pinos excêntricos criam forças centrífugas pesadas que desestabilizariam o trem em alta velocidade. Por isso, funde-se um bloco de chumbo/ferro no setor da roda oposto à manivela ($180^\circ$ de defasagem).
- Modelado com `Polygon` ocupando a meia-lua esquerda entre $X=10$ e $X=38$, centrado verticalmente:
  ```xml
  <Polygon Points="10,24 38,24 38,56 10,56" Fill="#17202A"/>
  ```

### 2.3 Raios em Cruz e Diagonais Simétricos (8 Raios)
Oito raios estruturais convergem rigorosamente para o centro analítico $(40,40)$:
- **Raio Vertical (90° e 270°)**: `Line` com comprimento $64\text{ px}$, transladada para $(40, 8)$.
- **Raio Horizontal (0° e 180°)**: `Line` com comprimento $64\text{ px}$, transladada para $(8, 40)$.
- **Raios Diagonais (45°, 135°, 225° e 315°)**: Duas `Lines` de $46\text{ px}$ transladadas para $(17, 17)$.

### 2.4 Manivela Sólida (*Crank Arm*) e Pino Excêntrico
Diferente de desenhos simplistas onde o pino fica flutuando solto sobre a roda, no projeto foi modelado o braço mecânico real de manivela:
1. **Cubo Central**: `Ellipse` de $24 \times 24\text{ px}$ transladada para $(28, 28)$ (centro exato $(40,40)$).
2. **Braço da Manivela**: `Rectangle` maciço de $26 \times 12\text{ px}$ com cantos arredondados, transladado para $(38, 34)$, ligando o centro $(40,40)$ até o ponto $(62,40)$.
3. **Cabeça de Fixação do Pino**: `Ellipse` de $16 \times 16\text{ px}$ transladada para $(54, 32)$ (centro $(62,40)$).
4. **Pino Excêntrico**: `Ellipse` de 8×8 px em vermelho/bronze (`#C0392B`) transladada para (58, 36) (centro (62, 40)).
   - O raio da manivela é:

```math
r_{\text{manivela}} = 62 - 40 = 22\text{ px}
```

5. **Parafuso Central do Eixo**: `Ellipse` preta de 8×8 px transladada para (36, 36) (centro (40, 40)), marcando com precisão o pivô de rotação.

---

## 3. Instanciação e Posicionamento no Chassi

No corpo do `LocomotivaCanvas`, foram instanciadas duas rodas sob a viga do chassi:

```xml
<!-- Roda 1 (Traseira): Centro no Canvas da Locomotiva em (130, 210) -->
<Control Template="{StaticResource RodaTemplate}" Width="80" Height="80">
    <Control.RenderTransform>
        <TransformGroup>
            <RotateTransform x:Name="RotacaoRoda1" Angle="0" CenterX="40" CenterY="40"/>
            <TranslateTransform X="90" Y="170"/>
        </TransformGroup>
    </Control.RenderTransform>
</Control>

<!-- Roda 2 (Dianteira): Centro no Canvas da Locomotiva em (270, 210) -->
<Control Template="{StaticResource RodaTemplate}" Width="80" Height="80">
    <Control.RenderTransform>
        <TransformGroup>
            <RotateTransform x:Name="RotacaoRoda2" Angle="0" CenterX="40" CenterY="40"/>
            <TranslateTransform X="230" Y="170"/>
        </TransformGroup>
    </Control.RenderTransform>
</Control>
```

### 3.1 Verificação Matemática da Rotação
No `RotateTransform`, as propriedades foram definidas como:
```xml
CenterX="40" CenterY="40"
```
Como a roda possui largura e altura de $80\text{ px}$, o ponto $(40,40)$ é o centro do seu sistema de coordenadas local. Ao aplicar qualquer ângulo $\theta$ através de `RotacaoRoda1.Angle` ou `RotacaoRoda2.Angle`:
- O aro gira em torno de si mesmo sem oscilação excêntrica (*wobble*).
- O pino vermelho orbita em uma trajetória perfeitamente circular de raio $r = 22\text{ px}$ ao redor do centro $(40,40)$.

### 3.2 Posições Globais no Canvas da Locomotiva
- **Roda 1**: Transladada para $(90, 170) \implies \text{Centro em } (90+40, 170+40) = \mathbf{(130, 210)}$.
- **Roda 2**: Transladada para $(230, 170) \implies \text{Centro em } (230+40, 170+40) = \mathbf{(270, 210)}$.
- **Distância entre Eixos (*Wheelbase*)**:

```math
D = 270 - 130 = 140\text{ px}
```

Essa distância entre eixos de 140 px é o parâmetro geométrico fundamental utilizado na modelagem da biela de acoplamento (*Side Rod*).

---

## 4. Reuso Modular: `MancalBielaTemplate`

Além do `RodaTemplate`, o arquivo [`Resources/LocomotivaResources.xaml`](https://github.com/Gabriel-Freitas-S/PI_T1/blob/main/Resources/LocomotivaResources.xaml) introduz o **`MancalBielaTemplate`**, um modelo de controle reutilizável que encapsula a unidade mecânica de um mancal de biela completo:
- **Copo de lubrificação de latão**: `Rectangle` $4 \times 4\text{ px}$ em $(0, -11)$.
- **Bucha externa de bronze forjado**: `Ellipse` $18 \times 18\text{ px}$ centrada em $(0,0)$.
- **Rolamento interno de aço usinado**: `Ellipse` $10 \times 10\text{ px}$ centrada em $(0,0)$.
- **Pino excêntrico central vermelho**: `Ellipse` $6 \times 6\text{ px}$ centrada em $(0,0)$.

### Instanciações no Projeto:
1. **Olhal Traseiro da Biela de Acoplamento**: `<Control Template="{StaticResource MancalBielaTemplate}"/>` em $(0,0)$.
2. **Olhal Dianteiro da Biela de Acoplamento**: `<Control Template="{StaticResource MancalBielaTemplate}">` transladado para $X=140\text{ px}$.
3. **Olhal Traseiro da Biela Motriz**: `<Control Template="{StaticResource MancalBielaTemplate}"/>` em $(0,0)$.

Isso elimina mais de 30 linhas de primitivas repetidas e garante uniformidade geométrica perfeita em todos os mancais da máquina.

---

## 🔗 Referências Oficiais da Microsoft
- [Microsoft Learn — Visão Geral de Modelos de Controle (ControlTemplate)](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/)
- [Microsoft Learn — Como aplicar um ControlTemplate](https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/how-to-apply-a-controltemplate/)
- [Microsoft Learn — Classe Control](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.controls.control/)
- [Microsoft Learn — Classe RotateTransform](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/)
- [Microsoft Learn — Pincéis e SolidColorBrush](https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.solidcolorbrush)
