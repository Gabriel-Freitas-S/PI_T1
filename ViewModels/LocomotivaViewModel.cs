using System.ComponentModel;
using PI_T1.Models;

namespace PI_T1.ViewModels;

/// <summary>
/// ViewModel da locomotiva a vapor 2D.
/// Encapsula as propriedades observáveis correspondentes às 9 transformações afins da locomotiva
/// e fornece notificação de alteração para o Data Binding declarativo do WPF.
///
/// Documentação oficial de referência:
/// - Microsoft Learn (Visão Geral do Data Binding no WPF):
///   https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/data/data-binding-overview
/// - Microsoft Learn (Visão Geral de Transformações):
///   https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/
/// </summary>
public class LocomotivaViewModel : ViewModelBase
{
    //# =======================================================================
    //# CACHE ESTÁTICO DE ARGUMENTOS DE NOTIFICAÇÃO (ZERO HEAP ALLOCATIONS NO GC)
    //# =======================================================================
    private static readonly PropertyChangedEventArgs LocomotivaXArgs = new(nameof(LocomotivaX));
    private static readonly PropertyChangedEventArgs EscalaDirecaoXArgs = new(nameof(EscalaDirecaoX));
    private static readonly PropertyChangedEventArgs AnguloRodasArgs = new(nameof(AnguloRodas));
    private static readonly PropertyChangedEventArgs BielaAcoplamentoXArgs = new(nameof(BielaAcoplamentoX));
    private static readonly PropertyChangedEventArgs BielaAcoplamentoYArgs = new(nameof(BielaAcoplamentoY));
    private static readonly PropertyChangedEventArgs CruzetaXArgs = new(nameof(CruzetaX));
    private static readonly PropertyChangedEventArgs CruzetaYArgs = new(nameof(CruzetaY));
    private static readonly PropertyChangedEventArgs PinoCruzetaXArgs = new(nameof(PinoCruzetaX));
    private static readonly PropertyChangedEventArgs PinoCruzetaYArgs = new(nameof(PinoCruzetaY));
    private static readonly PropertyChangedEventArgs HastePistaoXArgs = new(nameof(HastePistaoX));
    private static readonly PropertyChangedEventArgs HastePistaoYArgs = new(nameof(HastePistaoY));
    private static readonly PropertyChangedEventArgs BielaMotrizXArgs = new(nameof(BielaMotrizX));
    private static readonly PropertyChangedEventArgs BielaMotrizYArgs = new(nameof(BielaMotrizY));
    private static readonly PropertyChangedEventArgs BielaMotrizAnguloArgs = new(nameof(BielaMotrizAngulo));

    //# =======================================================================
    //# CAMPOS PRIVADOS DE ESTADO DE APRESENTAÇÃO
    //# =======================================================================

    private double _locomotivaX = 20.0;
    private double _escalaDirecaoX = 1.0;
    private double _anguloRodas;
    private double _bielaAcoplamentoX = 152.0;
    private double _bielaAcoplamentoY = 210.0;
    private double _cruzetaX = 321.0;
    private double _cruzetaY = 201.0;
    private double _pinoCruzetaX = 326.0;
    private double _pinoCruzetaY = 206.0;
    private double _hastePistaoX = 330.0;
    private double _hastePistaoY = 207.0;
    private double _bielaMotrizX = 292.0;
    private double _bielaMotrizY = 210.0;
    private double _bielaMotrizAngulo;

    //# =======================================================================
    //# PROPRIEDADES OBSERVÁVEIS VINCULADAS AO XAML
    //# =======================================================================

    /// <summary>
    /// Posição X da locomotiva no cenário ferroviário.
    /// </summary>
    public double LocomotivaX
    {
        get => _locomotivaX;
        set => SetProperty(ref _locomotivaX, value, LocomotivaXArgs);
    }

    /// <summary>
    /// Fator de escala horizontal direcional (+1.0 para a direita, -1.0 para a esquerda).
    /// </summary>
    public double EscalaDirecaoX
    {
        get => _escalaDirecaoX;
        set => SetProperty(ref _escalaDirecaoX, value, EscalaDirecaoXArgs);
    }

    /// <summary>
    /// Ângulo de rotação acumulado dos rodeiros acoplados em graus.
    /// </summary>
    public double AnguloRodas
    {
        get => _anguloRodas;
        set => SetProperty(ref _anguloRodas, value, AnguloRodasArgs);
    }

    /// <summary>
    /// Coordenada X da translação da biela de acoplamento horizontal.
    /// </summary>
    public double BielaAcoplamentoX
    {
        get => _bielaAcoplamentoX;
        set => SetProperty(ref _bielaAcoplamentoX, value, BielaAcoplamentoXArgs);
    }

    /// <summary>
    /// Coordenada Y da translação da biela de acoplamento horizontal.
    /// </summary>
    public double BielaAcoplamentoY
    {
        get => _bielaAcoplamentoY;
        set => SetProperty(ref _bielaAcoplamentoY, value, BielaAcoplamentoYArgs);
    }

    /// <summary>
    /// Coordenada X da cruzeta deslizante.
    /// </summary>
    public double CruzetaX
    {
        get => _cruzetaX;
        set => SetProperty(ref _cruzetaX, value, CruzetaXArgs);
    }

    /// <summary>
    /// Coordenada Y da cruzeta deslizante.
    /// </summary>
    public double CruzetaY
    {
        get => _cruzetaY;
        set => SetProperty(ref _cruzetaY, value, CruzetaYArgs);
    }

    /// <summary>
    /// Coordenada X do pino de articulação da cruzeta.
    /// </summary>
    public double PinoCruzetaX
    {
        get => _pinoCruzetaX;
        set => SetProperty(ref _pinoCruzetaX, value, PinoCruzetaXArgs);
    }

    /// <summary>
    /// Coordenada Y do pino de articulação da cruzeta.
    /// </summary>
    public double PinoCruzetaY
    {
        get => _pinoCruzetaY;
        set => SetProperty(ref _pinoCruzetaY, value, PinoCruzetaYArgs);
    }

    /// <summary>
    /// Coordenada X da haste do pistão acoplada à cruzeta.
    /// </summary>
    public double HastePistaoX
    {
        get => _hastePistaoX;
        set => SetProperty(ref _hastePistaoX, value, HastePistaoXArgs);
    }

    /// <summary>
    /// Coordenada Y da haste do pistão.
    /// </summary>
    public double HastePistaoY
    {
        get => _hastePistaoY;
        set => SetProperty(ref _hastePistaoY, value, HastePistaoYArgs);
    }

    /// <summary>
    /// Coordenada X da biela motriz no pino da roda motriz dianteira.
    /// </summary>
    public double BielaMotrizX
    {
        get => _bielaMotrizX;
        set => SetProperty(ref _bielaMotrizX, value, BielaMotrizXArgs);
    }

    /// <summary>
    /// Coordenada Y da biela motriz no pino da roda motriz dianteira.
    /// </summary>
    public double BielaMotrizY
    {
        get => _bielaMotrizY;
        set => SetProperty(ref _bielaMotrizY, value, BielaMotrizYArgs);
    }

    /// <summary>
    /// Ângulo de inclinação da biela motriz em graus.
    /// </summary>
    public double BielaMotrizAngulo
    {
        get => _bielaMotrizAngulo;
        set => SetProperty(ref _bielaMotrizAngulo, value, BielaMotrizAnguloArgs);
    }

    //# =======================================================================
    //# MÉTODOS DE ATUALIZAÇÃO DO ESTADO
    //# =======================================================================

    /// <summary>
    /// Atualiza em bloco as propriedades observáveis a partir do estado cinemático imutável fornecido pelo Model.
    /// Utiliza instâncias estáticas cacheadas de PropertyChangedEventArgs, garantindo Zero-Alloc no GC.
    /// </summary>
    /// <param name="estado">Estado calculado pelo motor de cinemática analítica.</param>
    public void AtualizarEstado(in LocomotivaFrameState estado)
    {
        LocomotivaX = estado.LocomotivaX;
        EscalaDirecaoX = estado.EscalaDirecaoX;
        AnguloRodas = estado.AnguloRodas;
        BielaAcoplamentoX = estado.BielaAcoplamentoX;
        BielaAcoplamentoY = estado.BielaAcoplamentoY;
        CruzetaX = estado.CruzetaX;
        CruzetaY = estado.CruzetaY;
        PinoCruzetaX = estado.PinoCruzetaX;
        PinoCruzetaY = estado.PinoCruzetaY;
        HastePistaoX = estado.HastePistaoX;
        HastePistaoY = estado.HastePistaoY;
        BielaMotrizX = estado.BielaMotrizX;
        BielaMotrizY = estado.BielaMotrizY;
        BielaMotrizAngulo = estado.BielaMotrizAngulo;
    }
}
