namespace PI_T1.Models;

/// <summary>
/// Motor de cinemática mecânica e física analítica da locomotiva a vapor 2D.
///
/// Classe pura em C#, sem dependência direta do subsistema gráfico do WPF,
/// responsável por calcular o movimento contínuo da locomotiva, o rolamento puro
/// sem deslizamento das rodas e a decomposição trigonométrica exata do sistema
/// biela-manivela e cruzeta deslizante a cada quadro.
///
/// Normas de referência:
/// - Trabalho C1.md:40-56 (Cinemática contínua, bielas sincronizadas e limites de janela)
/// - Slide 2D.md:225-299 (Transformações afins 2D e padrão do relógio)
/// </summary>
public class LocomotivaKinematics
{
    // # =======================================================================
    // # PARÂMETROS GEOMÉTRICOS E MECÂNICOS DA LOCOMOTIVA
    // # =======================================================================
    public const double RaioRoda = 40.0;               // * Raio primitivo da roda (diâmetro 80px)
    public const double RaioManivela = 22.0;           // * Raio do pino excêntrico da manivela
    public const double ComprimentoBielaMotriz = 82.0; // * Distância entre centros dos olhais (L = 82px)

    // * Posicionamento no Canvas da Locomotiva:
    public const double CentroRoda1X = 130.0;          // * Centro da Roda 1 (traseira) no Canvas
    public const double CentroRoda2X = 270.0;          // * Centro da Roda 2 (dianteira) no Canvas
    public const double CentroRodasY = 210.0;          // * Altura do eixo das rodas e da cruzeta (Y = 210px)

    // * Ciclo Global e Limites de Janela (Trabalho C1.md:54):
    public const double DuracaoCiclo = 14.0;           // * Duração total do ciclo completo (ida e volta em segundos)
    public const double LimiteEsquerdo = -100.0;       // * Limite esquerdo da janela
    public const double LimiteDireito = 540.0;         // * Limite direito da janela

    // # =======================================================================
    // # ESTADO DINÂMICO INTERNO
    // # =======================================================================
    private double _xLocoAnterior = LimiteEsquerdo;
    private double _anguloRodaAcumulado = 0.0;

    /// <summary>
    /// Reinicia o estado dinâmico da locomotiva.
    /// </summary>
    /// <param name="xInicial">Posição horizontal inicial no cenário.</param>
    public void Reset(double xInicial = LimiteEsquerdo)
    {
        _xLocoAnterior = xInicial;
        _anguloRodaAcumulado = 0.0;
    }

    /// <summary>
    /// Calcula a cinemática física analítica para o instante de tempo informado.
    /// </summary>
    /// <param name="segundos">Tempo total decorrido em segundos desde o início da simulação.</param>
    /// <returns>Estrutura imutável com todos os valores posicionais e angulares calculados.</returns>
    public LocomotivaFrameState CalcularQuadro(double segundos)
    {
        // # ===================================================================
        // # ETAPA 1: MOVIMENTO HORIZONTAL CONTÍNUO (VAI-E-VOLTA SUAVE)
        // # ===================================================================
        // * Conforme Trabalho C1.md:54 — translação completa nos limites da janela
        // ? Fórmula harmônica suave: progresso = (1 - cos(2 * PI * tau)) / 2
        // ? Garante aceleração e frenagem progressivas e naturais nos pontos de retorno.
        double tau = (segundos % DuracaoCiclo) / DuracaoCiclo; // [0, 1)
        double progressoSuave = (1.0 - Math.Cos(tau * 2.0 * Math.PI)) / 2.0; // [0, 1]
        double amplitude = LimiteDireito - LimiteEsquerdo;
        double xLocoAtual = LimiteEsquerdo + amplitude * progressoSuave;

        // # ===================================================================
        // # ETAPA 2: ROLAMENTO PURO DAS RODAS SEM DESLIZAMENTO
        // # ===================================================================
        // * Princípio da Dinâmica: distância percorrida = raio * ângulo radiano (s = R * theta)
        // ? deltaAngulo = (deltaX / RaioRoda) * (180 / PI)
        double deltaX = xLocoAtual - _xLocoAnterior;
        _xLocoAnterior = xLocoAtual;
        double deltaAnguloGraus = (deltaX / RaioRoda) * (180.0 / Math.PI);
        _anguloRodaAcumulado += deltaAnguloGraus;

        double theta = _anguloRodaAcumulado;
        double rad = theta * (Math.PI / 180.0);

        // # ===================================================================
        // # ETAPA 3: COORDENADAS ANALÍTICAS DOS PINOS DE MANIVELA
        // # ===================================================================
        // ? Decomposição circular exata a partir dos centros das rodas (130,210) e (270,210)
        // ? Cálculo trigonométrico unificado para eliminar operações duplicadas por quadro
        double dxManivela = RaioManivela * Math.Cos(rad);
        double dyManivela = RaioManivela * Math.Sin(rad);

        double pino1X = CentroRoda1X + dxManivela;
        double pino1Y = CentroRodasY + dyManivela;

        double pino2X = CentroRoda2X + dxManivela;
        double pino2Y = CentroRodasY + dyManivela;

        // # ===================================================================
        // # ETAPA 4: BIELA DE ACOPLAMENTO HORIZONTAL (SIDE ROD)
        // # ===================================================================
        // * Conforme Trabalho C1.md:48-52 (Etapa 3 - 8,0 pontos)
        // * A barra possui olhais em (0,0) e (140,0), correspondendo à distância exata entre as rodas.
        // ? Ao transladar a origem da barra para (pino1X, pino1Y), ambos os olhais
        // ? coincidem matematicamente com os pinos das duas rodas em 100% do tempo.
        double bielaAcoplamentoX = pino1X;
        double bielaAcoplamentoY = pino1Y;

        // # ===================================================================
        // # ETAPA 5: CRUZETA E MECANISMO DO PISTÃO (CROSSHEAD)
        // # ===================================================================
        // * A cruzeta desliza rigorosamente no eixo horizontal Y = 210 entre as guias de aço.
        // ? Teorema de Pitágoras no triângulo da biela motriz de hipotenusa L = 82px:
        // ? (xCruzeta - pino2X)^2 + (CentroRodasY - pino2Y)^2 = L^2
        // ? xCruzeta = pino2X + sqrt(L^2 - (CentroRodasY - pino2Y)^2)
        double catetoVertical = CentroRodasY - pino2Y; // = -RaioManivela * sin(rad)
        double termoRadical = Math.Max(0.0, (ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical));
        double catetoHorizontal = Math.Sqrt(termoRadical);
        double xCruzeta = pino2X + catetoHorizontal;

        // * Coordenadas de translação para Cruzeta, Pino e Haste do Pistão
        double cruzetaX = xCruzeta - 9.0;
        double cruzetaY = CentroRodasY - 9.0;

        double pinoCruzetaX = xCruzeta - 4.0;
        double pinoCruzetaY = CentroRodasY - 4.0;

        double hastePistaoX = xCruzeta;
        double hastePistaoY = CentroRodasY - 3.0;

        // # ===================================================================
        // # ETAPA 6: BIELA MOTRIZ ARTICULADA (CONNECTING ROD)
        // # ===================================================================
        // * Olhal traseiro em (0,0) coincide exatamente com o pino da Roda 2 (pino2X, pino2Y).
        // ! No WPF o eixo Y aponta para baixo na tela e RotateTransform.Angle em graus gira no sentido horário.
        // ? Com o vetor (xCruzeta - pino2X, CentroRodasY - pino2Y), Math.Atan2(deltaY, deltaX)
        // ? fornece com precisão analítica o ângulo horário perfeito para o olhal dianteiro (L=82px)
        // ? coincidir rigorosamente com a cruzeta em (xCruzeta, CentroRodasY).
        double bielaMotrizX = pino2X;
        double bielaMotrizY = pino2Y;
        double anguloBielaMotriz = Math.Atan2(CentroRodasY - pino2Y, xCruzeta - pino2X) * (180.0 / Math.PI);

        return new LocomotivaFrameState(
            LocomotivaX: xLocoAtual,
            AnguloRodas: theta,
            BielaAcoplamentoX: bielaAcoplamentoX,
            BielaAcoplamentoY: bielaAcoplamentoY,
            CruzetaX: cruzetaX,
            CruzetaY: cruzetaY,
            PinoCruzetaX: pinoCruzetaX,
            PinoCruzetaY: pinoCruzetaY,
            HastePistaoX: hastePistaoX,
            HastePistaoY: hastePistaoY,
            BielaMotrizX: bielaMotrizX,
            BielaMotrizY: bielaMotrizY,
            BielaMotrizAngulo: anguloBielaMotriz
        );
    }
}
