using System.Windows.Controls;
using PI_T1.Models;

namespace PI_T1.Controls;

/// <summary>
/// Interação lógica para LocomotivaControl.xaml
///
/// Encapsula a árvore visual vetorial da locomotiva 2D e atualiza suas matrizes
/// de transformação afim a partir do LocomotivaFrameState fornecido pelo motor cinemático.
/// </summary>
public partial class LocomotivaControl : UserControl
{
    public LocomotivaControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Aplica o estado cinemático calculado às matrizes de transformação afim internas do trem.
    /// </summary>
    /// <param name="estado">Estado imutável contendo coordenadas e rotações do quadro atual.</param>
    public void AtualizarEstado(in LocomotivaFrameState estado)
    {
        // 1. Rotação das rodas sob o chassi
        RotacaoRoda1.Angle = estado.AnguloRodas;
        RotacaoRoda2.Angle = estado.AnguloRodas;

        // 2. Translação da biela de acoplamento horizontal (side rod)
        TranslacaoBielaAcoplamento.X = estado.BielaAcoplamentoX;
        TranslacaoBielaAcoplamento.Y = estado.BielaAcoplamentoY;

        // 3. Cruzeta deslizante e componentes do pistão
        TranslacaoCruzeta.X = estado.CruzetaX;
        TranslacaoCruzeta.Y = estado.CruzetaY;

        TranslacaoPinoCruzeta.X = estado.PinoCruzetaX;
        TranslacaoPinoCruzeta.Y = estado.PinoCruzetaY;

        TranslacaoHastePistao.X = estado.HastePistaoX;
        TranslacaoHastePistao.Y = estado.HastePistaoY;

        // 4. Biela motriz articulada (translação + rotação analítica)
        TranslacaoBielaMotriz.X = estado.BielaMotrizX;
        TranslacaoBielaMotriz.Y = estado.BielaMotrizY;
        RotacaoBielaMotriz.Angle = estado.BielaMotrizAngulo;

        // 5. Translação global do Canvas da locomotiva através da tela
        TranslacaoLocomotiva.X = estado.LocomotivaX;
    }
}
