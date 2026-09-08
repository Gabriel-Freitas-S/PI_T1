using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using PI_T1.Models;

namespace PI_T1;

/// <summary>
/// Interação para MainWindow.xaml
/// Trabalho 1 de Processamento de Imagens — Locomotiva a Vapor 2D em WPF
///
/// Atua como View orquestradora: captura o evento CompositionTarget.Rendering
/// do WPF, consulta o motor de cinemática analítica pura (LocomotivaKinematics)
/// e delega a atualização visual diretamente ao controle autônomo LocomotivaControl.
///
/// Documentação de referência:
/// - Microsoft Learn (CompositionTarget.Rendering):
///   https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering
/// - Microsoft Learn (Visão Geral de Transformações):
///   https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/
/// - Microsoft Learn (Classe UserControl):
///   https://learn.microsoft.com/pt-br/dotnet/api/system.windows.controls.usercontrol
/// </summary>
public partial class MainWindow : Window
{
    //# =======================================================================
    //# ESTADO, TEMPORIZAÇÃO E MOTOR CINEMÁTICO
    //# =======================================================================
    private readonly Stopwatch _cronometro = new();
    private readonly LocomotivaKinematics _kinematics = new();

    public MainWindow()
    {
        InitializeComponent();

        //# ===================================================================
        //# CICLO DE VIDA: ASSOCIAÇÃO AO RENDER TARGET DO WPF
        //# ===================================================================
        Loaded += (_, _) =>
        {
            _kinematics.Reset();
            _cronometro.Restart();
            CompositionTarget.Rendering += AtualizarQuadroMecanico;
        };

        Unloaded += (_, _) =>
        {
            CompositionTarget.Rendering -= AtualizarQuadroMecanico;
            _cronometro.Stop();
        };
    }

    /// <summary>
    /// Manipulador executado a cada quadro de renderização do subsistema gráfico do WPF.
    /// Obtém o estado instantâneo do motor de cinemática e delega ao componente da locomotiva.
    /// </summary>
    private void AtualizarQuadroMecanico(object? sender, EventArgs e)
    {
        double largura = CenarioCanvas.ActualWidth > 0 ? CenarioCanvas.ActualWidth : 1100.0;
        LocomotivaFrameState estado = _kinematics.CalcularQuadro(_cronometro.Elapsed.TotalSeconds, largura);
        Locomotiva.AtualizarEstado(estado);
    }
}
