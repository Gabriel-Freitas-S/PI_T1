using System.Windows;

namespace PI_T1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    //# =======================================================================
    //# PONTO DE ENTRADA DO APLICATIVO
    //# =======================================================================
    //* Gerenciado pelo ciclo de vida padrão do Application do WPF (.NET 10)

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (e.Args.Length > 0 && e.Args[0] == "--screenshot")
        {
            var window = new MainWindow();
            window.Show();
            window.Measure(new Size(1100, 620));
            window.Arrange(new Rect(0, 0, 1100, 620));
            window.UpdateLayout();

            double t = e.Args.Length > 1 && double.TryParse(e.Args[1], System.Globalization.CultureInfo.InvariantCulture, out double customT) ? customT : 3.5;
            window.ViewModel.AtualizarQuadro(t, 1100.0);
            window.Locomotiva.AtualizarFumaca(t);
            window.UpdateLayout();

            var rtb = new System.Windows.Media.Imaging.RenderTargetBitmap(1100, 620, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            rtb.Render(window);

            string outPath = e.Args.Length > 2 ? e.Args[2] : "frame_check.png";
            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));
            using (var fs = System.IO.File.Open(outPath, System.IO.FileMode.Create))
            {
                encoder.Save(fs);
            }

            Shutdown();
            return;
        }

        if (e.Args.Length > 0 && e.Args[0] == "--generate-icon")
        {
            GenerateIconFiles();
            Shutdown();
            return;
        }

        if (e.Args.Length > 0 && e.Args[0] == "--record-frames")
        {
            var window = new MainWindow();
            window.Show();
            window.Measure(new Size(1100, 620));
            window.Arrange(new Rect(0, 0, 1100, 620));
            window.UpdateLayout();

            double duration = e.Args.Length > 1 && double.TryParse(e.Args[1], System.Globalization.CultureInfo.InvariantCulture, out double d) ? d : 15.0;
            int fps = e.Args.Length > 2 && int.TryParse(e.Args[2], out int f) ? f : 25;
            string outDir = e.Args.Length > 3 ? e.Args[3] : "frames";

            System.IO.Directory.CreateDirectory(outDir);
            int totalFrames = (int)(duration * fps);

            for (int i = 0; i < totalFrames; i++)
            {
                double t = (double)i / fps;
                window.ViewModel.AtualizarQuadro(t, 1100.0);
                window.Locomotiva.AtualizarFumaca(t);
                window.UpdateLayout();

                var rtb = new System.Windows.Media.Imaging.RenderTargetBitmap(1100, 620, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
                rtb.Render(window);

                string framePath = System.IO.Path.Combine(outDir, $"frame_{i + 1:D4}.png");
                var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));
                using (var fs = System.IO.File.Open(framePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }

            Shutdown();
            return;
        }
    }

    private static void GenerateIconFiles()
    {
        const int size = 256;
        var dv = new System.Windows.Media.DrawingVisual();
        using (var dc = dv.RenderOpen())
        {
            // Escala 4x para converter coordenadas da ViewBox 64x64 em 256x256
            dc.PushTransform(new System.Windows.Media.ScaleTransform(4, 4));

            // Fundo escuro com cantos arredondados
            var bgBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(9, 13, 22));
            var borderPen = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 41, 59)), 2);
            dc.DrawRoundedRectangle(bgBrush, null, new Rect(0, 0, 64, 64), 14, 14);
            dc.DrawRoundedRectangle(null, borderPen, new Rect(1, 1, 62, 62), 13, 13);

            // Trilhos
            var railPen = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(71, 85, 105)), 2)
            {
                StartLineCap = System.Windows.Media.PenLineCap.Round,
                EndLineCap = System.Windows.Media.PenLineCap.Round
            };
            dc.DrawLine(railPen, new Point(6, 52), new Point(58, 52));

            var tiePen = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 116, 139)), 2);
            dc.DrawLine(tiePen, new Point(12, 50), new Point(12, 54));
            dc.DrawLine(tiePen, new Point(24, 50), new Point(24, 54));
            dc.DrawLine(tiePen, new Point(36, 50), new Point(36, 54));
            dc.DrawLine(tiePen, new Point(48, 50), new Point(48, 54));

            // Pincéis Ciano e Âmbar
            var cyanBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 210, 255));
            var cyanPen2 = new System.Windows.Media.Pen(cyanBrush, 2);
            var cyanPen15 = new System.Windows.Media.Pen(cyanBrush, 1.5);
            var amberBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 158, 11));
            var amberPen2 = new System.Windows.Media.Pen(amberBrush, 2);
            var amberPen25 = new System.Windows.Media.Pen(amberBrush, 2.5)
            {
                StartLineCap = System.Windows.Media.PenLineCap.Round,
                EndLineCap = System.Windows.Media.PenLineCap.Round
            };

            // Cabine
            var cabinBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 41, 59));
            var cabinGeom = System.Windows.Media.Geometry.Parse("M12 24 H26 V44 H12 Z");
            dc.DrawGeometry(cabinBrush, cyanPen2, cabinGeom);

            // Janela da Cabine
            var winBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(77, 0, 210, 255));
            dc.DrawRoundedRectangle(winBrush, cyanPen15, new Rect(16, 28, 6, 6), 1, 1);

            // Caldeira
            var boilerBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(15, 23, 42));
            var boilerGeom = System.Windows.Media.Geometry.Parse("M26 28 H50 V44 H26 Z");
            dc.DrawGeometry(boilerBrush, cyanPen2, boilerGeom);

            // Chaminé
            var chimneyGeom = System.Windows.Media.Geometry.Parse("M42 28 V18 H46 V28");
            dc.DrawGeometry(cabinBrush, amberPen2, chimneyGeom);
            dc.DrawRoundedRectangle(amberBrush, null, new Rect(40, 16, 8, 3), 1, 1);

            // Limpa-trilhos (Cowcatcher)
            var pilotBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 65, 85));
            var pilotGeom = System.Windows.Media.Geometry.Parse("M50 38 L56 44 H50 Z");
            dc.DrawGeometry(pilotBrush, cyanPen15, pilotGeom);

            // Fumaça de vapor
            dc.DrawEllipse(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(102, 0, 210, 255)), null, new Point(44, 11), 2.5, 2.5);
            dc.DrawEllipse(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(51, 0, 210, 255)), null, new Point(48, 7), 3, 3);

            // Rodas motrizes
            var wheelBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(11, 17, 30));
            dc.DrawEllipse(wheelBrush, cyanPen2, new Point(22, 44), 6, 6);
            dc.DrawEllipse(cyanBrush, null, new Point(22, 44), 2, 2);
            dc.DrawEllipse(wheelBrush, cyanPen2, new Point(36, 44), 6, 6);
            dc.DrawEllipse(cyanBrush, null, new Point(36, 44), 2, 2);

            // Biela de acoplamento (Side Rod)
            dc.DrawLine(amberPen25, new Point(22, 44), new Point(36, 44));

            dc.Pop();
        }

        var rtb = new System.Windows.Media.Imaging.RenderTargetBitmap(size, size, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
        rtb.Render(dv);

        System.IO.Directory.CreateDirectory("Resources");
        string pngPath = System.IO.Path.Combine("Resources", "icon.png");
        byte[] pngBytes;
        using (var ms = new System.IO.MemoryStream())
        {
            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));
            encoder.Save(ms);
            pngBytes = ms.ToArray();
        }
        System.IO.File.WriteAllBytes(pngPath, pngBytes);

        string icoPath = System.IO.Path.Combine("Resources", "icon.ico");
        using (var fs = System.IO.File.Create(icoPath))
        using (var bw = new System.IO.BinaryWriter(fs))
        {
            bw.Write((short)0);
            bw.Write((short)1);
            bw.Write((short)1);

            bw.Write((byte)0); // 256px
            bw.Write((byte)0); // 256px
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write((short)1);
            bw.Write((short)32);
            bw.Write((int)pngBytes.Length);
            bw.Write((int)22);

            bw.Write(pngBytes);
        }
    }
}
