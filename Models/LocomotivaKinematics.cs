namespace PI_T1.Models;

/// <summary>
/// Motor de cinemática mecânica e física analítica da locomotiva a vapor 2D.
///
/// Classe pura em C#, sem dependência direta do subsistema gráfico do WPF,
/// responsável por calcular o movimento contínuo da locomotiva em circuito de túnel infinito
/// (entra pela esquerda, cruza toda a janela e sai pela direita), o rolamento puro sem
/// deslizamento das rodas e a decomposição trigonométrica exata do sistema biela-manivela
/// e cruzeta deslizante a cada quadro.
///
/// Normas de referência:
/// - Trabalho C1.md:40-56 (Cinemática contínua e bielas sincronizadas)
/// - Slide 2D.md:225-299 (Transformações afins 2D e padrão do relógio)
/// </summary>
public class LocomotivaKinematics
{
    //# =======================================================================
    //# PARÂMETROS GEOMÉTRICOS E MECÂNICOS DA LOCOMOTIVA
    //# =======================================================================
    public const double RaioRoda = 40.0;               //* Raio primitivo da roda (diâmetro 80px)
    public const double RaioManivela = 22.0;           //* Raio do pino excêntrico da manivela
    public const double ComprimentoBielaMotriz = 82.0; //* Distância entre centros dos olhais (L = 82px)
    public const double LarguraLocomotiva = 560.0;     //* Extensão total do bloco da locomotiva com para-choques
    public const double MargemSegurancaLateral = 20.0; //* Margem de segurança das bordas (100% visível na janela)

    //* Posicionamento no Canvas da Locomotiva:
    public const double CentroRoda1X = 130.0;          //* Centro da Roda 1 (traseira) no Canvas
    public const double CentroRoda2X = 270.0;          //* Centro da Roda 2 (dianteira) no Canvas
    public const double CentroRodasY = 210.0;          //* Altura do eixo das rodas e da cruzeta (Y = 210px)

    //* Parâmetros do Perfil Ferroviário Ping-Pong (Trapezoidal com Easing):
    public const double TempoAceleracao = 1.5;         //* Aceleração suave na partida (s)
    public const double TempoCruzeiro = 3.5;           //* Deslocamento em velocidade de cruzeiro (s)
    public const double TempoFrenagem = 1.5;           //* Frenagem suave até repouso (s)
    public const double TempoPausaManobra = 1.0;       //* Parada na estação para manobra e inversão (s)
    public const double DuracaoMeioCiclo = TempoAceleracao + TempoCruzeiro + TempoFrenagem + TempoPausaManobra; // 7.5s

    /// <summary>
    /// Reinicia o estado dinâmico da locomotiva (se necessário).
    /// </summary>
    public void Reset()
    {
        //* Estado puramente determinístico baseado em tempo contínuo
    }

    /// <summary>
    /// Calcula a cinemática física analítica para o instante de tempo informado.
    /// A locomotiva se desloca em movimento contínuo de vai-e-volta (Ping-Pong) entre os limites
    /// visíveis da janela, com aceleração suave, cruzeiro constante, frenagem realista nas extremidades
    /// e inversão visual de sentido (Trabalho C1.md:23, 56).
    /// </summary>
    /// <param name="segundos">Tempo total decorrido em segundos desde o início da simulação.</param>
    /// <param name="larguraCenario">Largura atual visível do cenário (para adaptação dinâmica ao redimensionamento).</param>
    /// <returns>Estrutura imutável com todos os valores posicionais, angulares e direcionais calculados.</returns>
    public static LocomotivaFrameState CalcularQuadro(double segundos, double larguraCenario = 1100.0)
    {
        //# ===================================================================
        //# ETAPA 1: LIMITES ESPACIAIS E VELOCIDADE ESCALAR DO PERCURSO PING-PONG
        //# ===================================================================
        double xMin = MargemSegurancaLateral;
        double xMax = Math.Max(xMin + 50.0, larguraCenario - LarguraLocomotiva - MargemSegurancaLateral);
        double cursoTotal = xMax - xMin;

        //* O tempo efetivo de deslocamento considera a média das velocidades nas transições (0.5 * T_acc + T_cru + 0.5 * T_fren)
        double tempoEfetivoDeslocamento = (0.5 * TempoAceleracao) + TempoCruzeiro + (0.5 * TempoFrenagem);
        double velocidadeCruzeiro = cursoTotal / tempoEfetivoDeslocamento;
        double distanciaAceleracao = 0.5 * velocidadeCruzeiro * TempoAceleracao;
        double distanciaCruzeiro = velocidadeCruzeiro * TempoCruzeiro;
        double distanciaFrenagem = 0.5 * velocidadeCruzeiro * TempoFrenagem;

        long indiceMeioCiclo = (long)(segundos / DuracaoMeioCiclo);
        double tempoNoMeioCiclo = segundos % DuracaoMeioCiclo;
        bool indoParaDireita = (indiceMeioCiclo % 2) == 0;

        double distanciaNoTrecho;
        string status;

        if (tempoNoMeioCiclo <= TempoAceleracao)
        {
            //* Fase 1: Partida e aceleração suave via curva cosseno (C1 contínua em velocidade)
            double u = tempoNoMeioCiclo / TempoAceleracao;
            double fatorIntegral = u - (Math.Sin(Math.PI * u) / Math.PI);
            distanciaNoTrecho = distanciaAceleracao * fatorIntegral;
            status = indoParaDireita
                ? "Partida da estação oeste: acelerando suavemente para a direita..."
                : "Partida da estação leste: acelerando suavemente para a esquerda...";
        }
        else if (tempoNoMeioCiclo <= TempoAceleracao + TempoCruzeiro)
        {
            //* Fase 2: Velocidade constante de cruzeiro no corpo central do cenário
            double tCruzeiro = tempoNoMeioCiclo - TempoAceleracao;
            distanciaNoTrecho = distanciaAceleracao + (velocidadeCruzeiro * tCruzeiro);
            status = indoParaDireita
                ? $"Em trânsito de cruzeiro: deslocando-se para a direita (~{velocidadeCruzeiro:F0} px/s)..."
                : $"Em trânsito de cruzeiro: deslocando-se para a esquerda (~{velocidadeCruzeiro:F0} px/s)...";
        }
        else if (tempoNoMeioCiclo <= TempoAceleracao + TempoCruzeiro + TempoFrenagem)
        {
            //* Fase 3: Frenagem progressiva suave até imobilização completa
            double tFrenagem = tempoNoMeioCiclo - (TempoAceleracao + TempoCruzeiro);
            double u = tFrenagem / TempoFrenagem;
            double fatorIntegral = u + (Math.Sin(Math.PI * u) / Math.PI);
            distanciaNoTrecho = distanciaAceleracao + distanciaCruzeiro + (distanciaFrenagem * fatorIntegral);
            status = indoParaDireita
                ? "Aproximação da estação leste: frenagem suave nos limites..."
                : "Aproximação da estação oeste: frenagem suave nos limites...";
        }
        else
        {
            //* Fase 4: Breve pausa de manobra para reversão de marcha
            distanciaNoTrecho = cursoTotal;
            status = indoParaDireita
                ? "Estação leste alcançada: manobra e inversão de sentido..."
                : "Estação oeste alcançada: manobra e inversão de sentido...";
        }

        double xLocoAtual;
        double escalaDirecaoX;

        if (indoParaDireita)
        {
            xLocoAtual = xMin + distanciaNoTrecho;
            escalaDirecaoX = 1.0;
        }
        else
        {
            xLocoAtual = xMax - distanciaNoTrecho;
            escalaDirecaoX = -1.0;
        }

        //# ===================================================================
        //# ETAPA 2: ROTAÇÃO PURA DAS RODAS (MONÓTONA E SEM DESCONTINUIDADES)
        //# ===================================================================
        double distanciaTotalRolada = (indiceMeioCiclo * cursoTotal) + distanciaNoTrecho;
        double theta = (distanciaTotalRolada / RaioRoda) * (180.0 / Math.PI);
        double rad = theta * (Math.PI / 180.0);

        //# ===================================================================
        //# ETAPA 3: COORDENADAS ANALÍTICAS DOS PINOS DE MANIVELA
        //# ===================================================================
        double dxManivela = RaioManivela * Math.Cos(rad);
        double dyManivela = RaioManivela * Math.Sin(rad);

        double pino1X = CentroRoda1X + dxManivela;
        double pino1Y = CentroRodasY + dyManivela;

        double pino2X = CentroRoda2X + dxManivela;
        double pino2Y = CentroRodasY + dyManivela;

        //# ===================================================================
        //# ETAPA 4: BIELA DE ACOPLAMENTO HORIZONTAL (SIDE ROD)
        //# ===================================================================
        double bielaAcoplamentoX = pino1X;
        double bielaAcoplamentoY = pino1Y;

        //# ===================================================================
        //# ETAPA 5: CRUZETA E MECANISMO DO PISTÃO (CROSSHEAD)
        //# ===================================================================
        double catetoVertical = CentroRodasY - pino2Y;
        double termoRadical = Math.Max(0.0, (ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical));
        double catetoHorizontal = Math.Sqrt(termoRadical);
        double xCruzeta = pino2X + catetoHorizontal;

        double cruzetaX = xCruzeta - 9.0;
        double cruzetaY = CentroRodasY - 9.0;

        double pinoCruzetaX = xCruzeta - 4.0;
        double pinoCruzetaY = CentroRodasY - 4.0;

        double hastePistaoX = xCruzeta;
        double hastePistaoY = CentroRodasY - 3.0;

        //# ===================================================================
        //# ETAPA 6: BIELA MOTRIZ ARTICULADA (CONNECTING ROD)
        //# ===================================================================
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
            BielaMotrizAngulo: anguloBielaMotriz,
            EscalaDirecaoX: escalaDirecaoX,
            StatusDescritivo: status
        );
    }
}
