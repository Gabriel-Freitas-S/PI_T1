using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using PI_T1.ViewModels;

namespace PI_T1;

/// <summary>
/// Interação para MainWindow.xaml
/// Trabalho 1 de Processamento de Imagens — Locomotiva a Vapor 2D em WPF
///
/// Atua como a View principal no padrão MVVM:
/// - Associa seu DataContext a uma instância do MainViewModel.
/// - Captura o ciclo de V-Sync da GPU via CompositionTarget.Rendering.
/// - Delega a atualização contínua do estado físico/cinemático ao MainViewModel.
///
/// Documentação oficial de referência:
/// - Microsoft Learn (Visão Geral do Data Binding no WPF):
///   https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/data/data-binding-overview
/// - Microsoft Learn (CompositionTarget.Rendering):
///   https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.compositiontarget.rendering
/// </summary>
public partial class MainWindow : Window
{
    //# =======================================================================
    //# VIEWMODEL E TEMPORIZAÇÃO DA SIMULAÇÃO
    //# =======================================================================

    private readonly Stopwatch _cronometro = new();

    /// <summary>
    /// ViewModel principal associado ao contexto de dados desta janela.
    /// </summary>
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();

        ViewModel = new MainViewModel();
        DataContext = ViewModel;

        //# ===================================================================
        //# CICLO DE VIDA: ASSOCIAÇÃO AO RENDER TARGET DO WPF
        //# ===================================================================
        Loaded += (_, _) =>
        {
            ViewModel.Reset();
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
    /// Delega ao ViewModel a atualização cinemática correspondente ao tempo atual.
    /// </summary>
    private void AtualizarQuadroMecanico(object? sender, EventArgs e)
    {
        double largura = CenarioCanvas.ActualWidth > 0 ? CenarioCanvas.ActualWidth : 1100.0;
        ViewModel.AtualizarQuadro(_cronometro.Elapsed.TotalSeconds, largura);
    }
}
