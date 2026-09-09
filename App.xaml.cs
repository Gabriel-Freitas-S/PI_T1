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

        if (e.Args.Length > 0 && e.Args[0] == "--record-frames")
        {
            var window = new MainWindow();
            window.Show();
            window.Measure(new Size(1100, 620));
            window.Arrange(new Rect(0, 0, 1100, 620));
            window.UpdateLayout();

            double duration = e.Args.Length > 1 && double.TryParse(e.Args[1], System.Globalization.CultureInfo.InvariantCulture, out double d) ? d : 11.0;
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
}
