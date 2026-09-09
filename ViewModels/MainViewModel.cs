using PI_T1.Models;

namespace PI_T1.ViewModels;

/// <summary>
/// ViewModel principal da aplicação (MainWindow).
/// Centraliza os dados de apresentação, títulos informativos e orquestra a comunicação
/// entre o motor de cinemática analítica (Model) e os componentes visuais (Views).
///
/// Documentação oficial de referência:
/// - Microsoft Learn (Padrão Model-View-ViewModel no WPF):
///   https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/data/data-binding-overview
/// </summary>
public class MainViewModel : ViewModelBase
{
    //# =======================================================================
    //# DEPENDÊNCIAS DE DOMÍNIO E SUB-VIEWMODELS
    //# =======================================================================

    private readonly LocomotivaKinematics _kinematics = new();

    /// <summary>
    /// Instância do motor de cinemática analítica (Model puro).
    /// </summary>
    public LocomotivaKinematics Kinematics => _kinematics;

    /// <summary>
    /// ViewModel filha responsável pelo estado geométrico da locomotiva.
    /// </summary>
    public LocomotivaViewModel Locomotiva { get; } = new();

    //# =======================================================================
    //# PROPRIEDADES DE APRESENTAÇÃO DA INTERFACE PRINCIPAL
    //# =======================================================================

    private string _titulo = "PROCESSO DE IMAGENS — TRABALHO C1: LOCOMOTIVA A VAPOR 2D";
    private string _subtitulo = " | Cinemática Analítica & Mecanismo Biela-Manivela";
    private string _statusTrajetoria = "Trajetória contínua nos limites da janela (vai-e-volta com inversão e física analítica biela-manivela).";
    private string _autor = "Gabriel Freitas Souza";

    /// <summary>
    /// Título informativo exibido no cabeçalho.
    /// </summary>
    public string Titulo
    {
        get => _titulo;
        set => SetProperty(ref _titulo, value);
    }

    /// <summary>
    /// Subtítulo descritivo do tema da disciplina.
    /// </summary>
    public string Subtitulo
    {
        get => _subtitulo;
        set => SetProperty(ref _subtitulo, value);
    }

    /// <summary>
    /// Mensagem descritiva de status da simulação no rodapé.
    /// </summary>
    public string StatusTrajetoria
    {
        get => _statusTrajetoria;
        set => SetProperty(ref _statusTrajetoria, value);
    }

    /// <summary>
    /// Identificação do autor do projeto.
    /// </summary>
    public string Autor
    {
        get => _autor;
        set => SetProperty(ref _autor, value);
    }

    //# =======================================================================
    //# CICLO DE SIMULAÇÃO E ORQUESTRAÇÃO
    //# =======================================================================

    /// <summary>
    /// Executa o cálculo cinemático determinístico para o instante fornecido e atualiza o ViewModel da locomotiva.
    /// </summary>
    /// <param name="tempoSegundos">Tempo decorrido medido pelo cronômetro de alta precisão.</param>
    /// <param name="larguraCenario">Largura atual do contêiner visual de percurso.</param>
    public void AtualizarQuadro(double tempoSegundos, double larguraCenario)
    {
        LocomotivaFrameState estado = LocomotivaKinematics.CalcularQuadro(tempoSegundos, larguraCenario);
        Locomotiva.AtualizarEstado(estado);

        if (!string.IsNullOrEmpty(estado.StatusDescritivo) && _statusTrajetoria != estado.StatusDescritivo)
        {
            StatusTrajetoria = estado.StatusDescritivo;
        }
    }

    /// <summary>
    /// Reinicia o estado do motor cinemático para as condições iniciais.
    /// </summary>
    public void Reset()
    {
        _kinematics.Reset();
    }
}
