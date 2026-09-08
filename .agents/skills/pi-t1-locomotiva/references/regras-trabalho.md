# Regras do trabalho — checklist (fonte: `Trabalho/Trabalho C1.md`)

## Etapa 1 — (3,0) Corpo + rodas (`Trabalho C1.md:32`)

- [ ] Corpo estatico com `Rectangle`, `Polygon`, `Ellipse`: chassi, cabine, chamine.
- [ ] `ControlTemplate` do modelo de roda, com raios/detalhes internos (rotacao visivel).
- [ ] Instanciar **>= 2 rodas** do template sob o chassi, posicionadas corretamente.

## Etapa 2 — (6,0) Rotacao + translacao (`Trabalho C1.md:40`)

- [ ] Agrupar corpo + rodas num `Canvas`.
- [ ] `RotateTransform` em cada roda + `Storyboard` continuo do angulo.
- [ ] `TranslateTransform` no `Canvas` da locomotiva + animacao horizontal.

## Etapa 3 — (8,0) Bielas (`Trabalho C1.md:48`)

- [ ] Bielas com `Rectangle` ou `Line` ligando as rodas.
- [ ] Animacao sincronizada com a rotacao (combinar transforms por quadro).

## Etapa 4 — (10,0) Completo (`Trabalho C1.md:54`)

- [ ] Locomotiva vai-e-volta na janela principal ate os limites.

## Constraints globais

- Tudo em `(0,0)` + `RenderTransform` para posicionar/animar (`Trabalho C1.md:26`).
- Reaproveitar padrao do relogio dos slides onde couber.
- Grupo de 3; apresentacao em sala (`Trabalho C1.md:7`).
