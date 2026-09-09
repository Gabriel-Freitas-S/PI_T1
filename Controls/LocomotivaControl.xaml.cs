using System.Windows.Controls;
using PI_T1.Models;
using PI_T1.ViewModels;

namespace PI_T1.Controls;

/// <summary>
/// Interação lógica para LocomotivaControl.xaml
///
/// Encapsula a árvore visual vetorial da locomotiva 2D e atua como a View no padrão MVVM.
/// Suporta associação de dados reativa (Data Binding) via LocomotivaViewModel
/// e mantém método de compatibilidade direta para renderização offline de quadros.
/// </summary>
public partial class LocomotivaControl : UserControl
{
    public LocomotivaControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Aplica o estado cinemático calculado delegando diretamente ao LocomotivaViewModel via Data Binding (MVVM puro).
    /// </summary>
    /// <param name="estado">Estado imutável contendo coordenadas e rotações do quadro atual.</param>
    public void AtualizarEstado(in LocomotivaFrameState estado)
    {
        if (DataContext is LocomotivaViewModel vm)
        {
            vm.AtualizarEstado(estado);
        }
    }

    /// <summary>
    /// Atualiza analiticamente as partículas de vapor da chaminé para renderização determinística por quadro (GIF/Screenshot).
    /// </summary>
    /// <param name="tempo">Tempo decorrido em segundos.</param>
    public void AtualizarFumaca(double tempo)
    {
        //* Baforada 1 (período 1.5s)
        double p1 = (tempo % 1.5) / 1.5;
        TranslacaoFumaca1.Y = 40.0 - 80.0 * p1;
        TranslacaoFumaca1.X = 395.0 - 75.0 * p1;
        double s1 = 0.5 + 1.7 * p1;
        EscalaFumaca1.ScaleX = s1;
        EscalaFumaca1.ScaleY = s1;
        Fumaca1.Opacity = 0.8 * (1.0 - p1);

        //* Baforada 2 (período 1.8s, defasagem 0.5s)
        double t2 = (tempo + 1.3) % 1.8;
        double p2 = t2 / 1.8;
        TranslacaoFumaca2.Y = 40.0 - 90.0 * p2;
        TranslacaoFumaca2.X = 395.0 - 105.0 * p2;
        double s2 = 0.6 + 2.2 * p2;
        EscalaFumaca2.ScaleX = s2;
        EscalaFumaca2.ScaleY = s2;
        Fumaca2.Opacity = 0.7 * (1.0 - p2);

        //* Baforada 3 (período 2.0s, defasagem 1.0s)
        double t3 = (tempo + 1.0) % 2.0;
        double p3 = t3 / 2.0;
        TranslacaoFumaca3.Y = 40.0 - 100.0 * p3;
        TranslacaoFumaca3.X = 395.0 - 135.0 * p3;
        double s3 = 0.7 + 2.8 * p3;
        EscalaFumaca3.ScaleX = s3;
        EscalaFumaca3.ScaleY = s3;
        Fumaca3.Opacity = 0.6 * (1.0 - p3);
    }
}
