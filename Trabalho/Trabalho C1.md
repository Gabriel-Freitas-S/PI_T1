# Trabalho C1 Processamento de Imagens

## VISÃO GERAL E OBJETIVO

O objetivo deste trabalho é a aplicação prática dos conceitos de elementos gráficos e transformações 2D utilizando a plataforma Windows Presentation Foundation (WPF). Através da construção de uma animação, os alunos irão aprofundar seus conhecimentos sobre a manipulação de objetos, o uso de templates e a aplicação de animações de transformação.

- Formato: O trabalho deverá ser feito em grupos de três alunos.

- Apresentação: O trabalho deverá ser apresentado ao professor em sala de aula no dia 16/09/2025.

## INSTRUÇÕES GERAIS

O desafio deste trabalho é construir uma aplicação WPF que exiba uma animação de uma locomotiva a vapor em uma vista lateral. A locomotiva deve se mover horizontalmente pela tela, com suas rodas girando e um sistema de bielas (braços de conexão) se movendo de forma sincronizada.

O gráfico 2D deverá conter:

- O corpo principal da locomotiva (chassi, cabine e chaminé).

- Pelo menos duas rodas que giram em torno de seus próprios eixos.

- Bielas que conectem as rodas e simulem o movimento mecânico.

- A locomotiva completa deve se deslocar para a direita e para a esquerda na tela.


Os conceitos utilizados neste trabalho são análogos aos apresentados no exemplo do relógio, desenvolvido em sala de aula. Alguns códigos do exemplo do relógio podem ser reaproveitados neste trabalho. Recomenda-se que todos os elementos gráficos sejam criados com o ponto de referência em (0,0) e que transformações (RenderTransform) sejam aplicadas para posicioná-los e animá-los corretamente.

## ROTEIRO DE IMPLEMENTAÇÃO E PONTUAÇÃO

A seguir, é apresentado um roteiro de tarefas sugerido para guiar o desenvolvimento do projeto. A pontuação será distribuída de acordo com a conclusão de cada etapa.

1. (3,0) Criação do Corpo da Locomotiva e das Rodas:

- Utilizando formas básicas como Rectangle, Polygon e Ellipse, desenhe o corpo estático da locomotiva.

- Crie um ControlTemplate para o modelo de uma roda. A roda deve conter detalhes internos, como raios, para que a rotação seja visível.

- Instancie pelo menos duas rodas a partir do template, posicionando-as corretamente sob o chassi da locomotiva.

2. (6,0) Animação de Rotação e Translação:

- Agrupe todos os elementos da locomotiva (corpo e rodas) dentro de um Canvas.

- Aplique uma RotateTransform em cada uma das instâncias das rodas e crie uma Storyboard para animar o ângulo de rotação continuamente.

- Aplique uma TranslateTransform no Canvas que contém toda a locomotiva e crie uma animação para mover o conjunto horizontalmente pela tela.

3. (8,0) Animação das Bielas de Conexão:

- Desenhe as bielas (braços de conexão) que ligam as rodas, utilizando Rectangle ou Line.

- Crie uma animação para as bielas que simula o movimento mecânico real, acompanhando a rotação das rodas. Esta é a parte mais desafiadora e exigirá a combinação de diferentes transformações para posicionar e girar as bielas corretamente a cada quadro da animação.

4. (10,0) Projeto completo:

- Faça com que a locomotiva se movimente horizontalmente na janela principal da aplicação, indo e voltando até alcançar os limites da janela.
