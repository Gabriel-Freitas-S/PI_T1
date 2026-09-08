# Padroes — do relogio (slides) para a locomotiva

Fonte: `Slide/2D.md:225` em diante. Citacoes como `2D.md:<linha>`.

## 1. Canvas como area grafica (`2D.md:230`)

- `Canvas` posiciona filhos por coordenadas relativas ao pai.
- `(0,0)` = canto superior esquerdo; 1 unidade = 1/96 pol (`2D.md:240`).
- Padrao: desenhar pequeno em `(0,0)` e levar ao lugar com escala + translacao (`2D.md:245`).

## 2. Template em resources (`2D.md:258`)

- Ponteiros do relogio: `Polygon` definido em `Canvas.Resources` como template/controle.
- Reuso: ponteiro minutos instanciado do template (`2D.md:263`); horas reusa + escala + rotacao (`2D.md:268`).
- Locomotiva: mesmo papel para **rodas** — `ControlTemplate` com `Ellipse` + raios (`Polygon`/`Line`),
  instanciar 2x, variar `RenderTransform` por instancia (`2D.md:275` mostra duas secoes de transform).

## 3. Animacao XAML (`2D.md:281`)

- Apontar para posicao inicial (ex.: 12:00, `2D.md:283`), depois animar `RotateTransform.Angle`
  com `DoubleAnimation` dentro de `Storyboard`, disparado por `triggers` do `Canvas` (`2D.md:288`).
- Locomotiva: rodas = `DoubleAnimation` continua no angulo; conjunto = animacao da
  `TranslateTransform.X` do `Canvas` (ir e voltar nos limites da janela).

## 4. Hierarquia de transforms

- Global (Canvas inteiro) + local (por controle). Nao misturar: posicionamento estatico vai no
  `RenderTransform` do elemento; movimento continuo vai no `Storyboard`.
