using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace PI_T1;

/// <summary>
/// Interação para MainWindow.xaml
/// Trabalho 1 de Processamento de Imagens — Locomotiva a Vapor 2D em WPF
///
/// Implementa a cinemática analítica exata da locomotiva a vapor a cada quadro
/// via CompositionTarget.Rendering (conforme Trabalho C1.md:52 e documentação oficial
/// do Microsoft Learn).
/// </summary>
public partial class MainWindow : Window
{
    // # =======================================================================
    // # ESTADO E TEMPORIZAÇÃO DO SISTEMA
    // # =======================================================================
    private readonly Stopwatch _cronometro = new();
    private double _xLocoAnterior = -100;
    private double _anguloRodaAcumulado = 0;

    // # =======================================================================
    // # PARÂMETROS GEOMÉTRICOS E MECÂNICOS DA LOCOMOTIVA
    // # =======================================================================
    // * Roda e Manivela:
    private const double RaioRoda = 40.0;               // * Raio primitivo da roda (diâmetro 80px)
    private const double RaioManivela = 22.0;           // * Raio do pino excêntrico da manivela
    private const double ComprimentoBielaMotriz = 95.0; // * Distância entre centros dos olhais (L = 95px)

    // * Posicionamento no Canvas da Locomotiva:
    private const double CentroRoda1X = 130.0;          // * Centro da Roda 1 (traseira) no Canvas
    private const double CentroRoda2X = 270.0;          // * Centro da Roda 2 (dianteira) no Canvas
    private const double CentroRodasY = 210.0;          // * Altura do eixo das rodas e da cruzeta (Y = 210px)

    // * Ciclo Global e Limites de Janela (Trabalho C1.md:54):
    private const double DuracaoCiclo = 14.0;           // * Duração total do ciclo completo (ida e volta em segundos)
    private const double LimiteEsquerdo = -100.0;       // * Limite esquerdo da janela
    private const double LimiteDireito = 540.0;         // * Limite direito da janela

    public MainWindow()
    {
        InitializeComponent();

        // # ===================================================================
        // # CICLO DE VIDA: ASSOCIAÇÃO DO RENDER TARGET DO WPF
        // # ===================================================================
        // * Conforme Microsoft Learn: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/how-to-render-on-a-per-frame-interval-using-compositiontarget
        Loaded += (_, _) =>
        {
            _xLocoAnterior = LimiteEsquerdo;
            _cronometro.Start();
            CompositionTarget.Rendering += AtualizarQuadroMecanico;
        };

        Unloaded += (_, _) =>
        {
            CompositionTarget.Rendering -= AtualizarQuadroMecanico;
            _cronometro.Stop();
        };
    }

    /// <summary>
    /// Método executado a cada quadro de renderização do subsistema gráfico do WPF.
    /// Calcula a cinemática física analítica sem aproximações lineares ou defasagens.
    /// </summary>
    private void AtualizarQuadroMecanico(object? sender, EventArgs e)
    {
        double segundos = _cronometro.Elapsed.TotalSeconds;

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
        // # ETAPA 3: ATUALIZAÇÃO ANGULAR DAS RODAS
        // # ===================================================================
        // * Slide 2D.md:258 — RotateTransform em torno do centro (CenterX=40, CenterY=40)
        RotacaoRoda1.Angle = theta;
        RotacaoRoda2.Angle = theta;

        // # ===================================================================
        // # ETAPA 4: COORDENADAS ANALÍTICAS DOS PINOS DE MANIVELA
        // # ===================================================================
        // ? Decomposição circular exata a partir dos centros das rodas (130,210) e (270,210)
        double pino1X = CentroRoda1X + RaioManivela * Math.Cos(rad);
        double pino1Y = CentroRodasY + RaioManivela * Math.Sin(rad);

        double pino2X = CentroRoda2X + RaioManivela * Math.Cos(rad);
        double pino2Y = CentroRodasY + RaioManivela * Math.Sin(rad);

        // # ===================================================================
        // # ETAPA 5: BIELA DE ACOPLAMENTO HORIZONTAL (SIDE ROD)
        // # ===================================================================
        // * Conforme Trabalho C1.md:48-52 (Etapa 3 - 8,0 pontos)
        // * A barra possui olhais em (0,0) e (140,0), correspondendo à distância exata entre as rodas.
        // ? Ao transladar a origem da barra para (pino1X, pino1Y), ambos os olhais
        // ? coincidem matematicamente com os pinos das duas rodas em 100% do tempo!
        TranslacaoBielaAcoplamento.X = pino1X;
        TranslacaoBielaAcoplamento.Y = pino1Y;

        // # ===================================================================
        // # ETAPA 6: CRUZETA E MECANISMO DO PISTÃO (CROSSHEAD)
        // # ===================================================================
        // * A cruzeta desliza rigorosamente no eixo horizontal Y = 210 entre as guias de aço.
        // ? Teorema de Pitágoras no triângulo da biela motriz de hipotenusa L = 95px:
        // ? (xCruzeta - pino2X)^2 + (CentroRodasY - pino2Y)^2 = L^2
        // ? xCruzeta = pino2X + sqrt(L^2 - (CentroRodasY - pino2Y)^2)
        double catetoVertical = CentroRodasY - pino2Y; // = -RaioManivela * sin(rad)
        double termoRadical = Math.Max(0.0, (ComprimentoBielaMotriz * ComprimentoBielaMotriz) - (catetoVertical * catetoVertical));
        double catetoHorizontal = Math.Sqrt(termoRadical);
        double xCruzeta = pino2X + catetoHorizontal;

        // * Posicionamento geométrico exato da Cruzeta, Pino e Haste do Pistão
        TranslacaoCruzeta.X = xCruzeta - 9.0;
        TranslacaoCruzeta.Y = CentroRodasY - 9.0;

        TranslacaoPinoCruzeta.X = xCruzeta - 4.0;
        TranslacaoPinoCruzeta.Y = CentroRodasY - 4.0;

        TranslacaoHastePistao.X = xCruzeta + 5.0;
        TranslacaoHastePistao.Y = CentroRodasY - 3.0;

        // # ===================================================================
        // # ETAPA 7: BIELA MOTRIZ ARTICULADA (CONNECTING ROD)
        // # ===================================================================
        // * Olhal traseiro em (0,0) coincide exatamente com o pino da Roda 2 (pino2X, pino2Y).
        // ! ATENÇÃO: No WPF o eixo Y aponta para baixo na tela e RotateTransform.Angle
        // ! em graus gira no sentido horário.
        // ? Com o vetor (xCruzeta - pino2X, CentroRodasY - pino2Y), Math.Atan2(deltaY, deltaX)
        // ? fornece com precisão analítica o ângulo horário perfeito para o olhal dianteiro (L=95px)
        // ? coincidir rigorosamente com a cruzeta em (xCruzeta, CentroRodasY)!
        TranslacaoBielaMotriz.X = pino2X;
        TranslacaoBielaMotriz.Y = pino2Y;
        double anguloBielaMotriz = Math.Atan2(CentroRodasY - pino2Y, xCruzeta - pino2X) * (180.0 / Math.PI);
        RotacaoBielaMotriz.Angle = anguloBielaMotriz;

        // # ===================================================================
        // # ETAPA 8: TRANSLAÇÃO GLOBAL DO CANVAS DA LOCOMOTIVA
        // # ===================================================================
        // * Desloca todo o conjunto hierárquico 2D da locomotiva através da tela
        TranslacaoLocomotiva.X = xLocoAtual;
    }
}
