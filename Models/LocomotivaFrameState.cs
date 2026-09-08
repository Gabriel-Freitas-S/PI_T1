namespace PI_T1.Models;

/// <summary>
/// Representa o estado cinemático e geométrico calculado para um quadro específico da locomotiva.
/// Estrutura imutável de dados desacoplada do subsistema de renderização do WPF.
/// </summary>
/// <param name="LocomotivaX">Posição horizontal do Canvas da locomotiva no cenário.</param>
/// <param name="AnguloRodas">Ângulo acumulado de rotação das rodas em graus.</param>
/// <param name="BielaAcoplamentoX">Coordenada X de translação da biela de acoplamento (side rod).</param>
/// <param name="BielaAcoplamentoY">Coordenada Y de translação da biela de acoplamento (side rod).</param>
/// <param name="CruzetaX">Coordenada X de translação da cruzeta deslizante (crosshead).</param>
/// <param name="CruzetaY">Coordenada Y de translação da cruzeta deslizante (crosshead).</param>
/// <param name="PinoCruzetaX">Coordenada X de translação do pino articulador da cruzeta.</param>
/// <param name="PinoCruzetaY">Coordenada Y de translação do pino articulador da cruzeta.</param>
/// <param name="HastePistaoX">Coordenada X de translação da haste do pistão.</param>
/// <param name="HastePistaoY">Coordenada Y de translação da haste do pistão.</param>
/// <param name="BielaMotrizX">Coordenada X de ancoragem da biela motriz no pino da roda dianteira.</param>
/// <param name="BielaMotrizY">Coordenada Y de ancoragem da biela motriz no pino da roda dianteira.</param>
/// <param name="BielaMotrizAngulo">Ângulo de inclinação da biela motriz em graus (calculado via Math.Atan2).</param>
public readonly record struct LocomotivaFrameState(
    double LocomotivaX,
    double AnguloRodas,
    double BielaAcoplamentoX,
    double BielaAcoplamentoY,
    double CruzetaX,
    double CruzetaY,
    double PinoCruzetaX,
    double PinoCruzetaY,
    double HastePistaoX,
    double HastePistaoY,
    double BielaMotrizX,
    double BielaMotrizY,
    double BielaMotrizAngulo
);
