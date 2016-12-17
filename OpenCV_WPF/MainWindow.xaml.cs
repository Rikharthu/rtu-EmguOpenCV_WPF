using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using System.Runtime.InteropServices;
using System.Timers;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Windows.Threading;

namespace OpenCV_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Capture capture = new Capture();
        DispatcherTimer timer = new DispatcherTimer();

        const bool GAUSSIAN_BLUR_ENABLED=true;
        const bool COLOR_ENABLED = true;
        const bool ADAPTIVE_TRESHOLD_ENABLED = true;

        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromMilliseconds( 20);
            timer.Tick += TimerElapsed;

        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            // 1) iegut attelu
            // matrica lai sagalbat attelu
            Mat image = capture.QueryFrame();

            // 2) Veikt transformacijas ar attelu

            // pievienot tekstu
            CvInvoke.PutText(
                image,
                "RTU",
                new System.Drawing.Point(10, 80),
                FontFace.HersheyComplex,
                2.0,
                new Bgr(0, 255, 0).MCvScalar);

            if(GAUSSIAN_BLUR_ENABLED)
            CvInvoke.GaussianBlur(
                image,                              // input
                image,                              // output
                new System.Drawing.Size(5, 5),      // Gaussian blur parametri (frekvence)
                1.0                                 // ?
                );

            if(COLOR_ENABLED)
            CvInvoke.CvtColor(image, image, ColorConversion.Rgba2Gray);

            if(ADAPTIVE_TRESHOLD_ENABLED)
            CvInvoke.AdaptiveThreshold(
                image,
                image,
                255,
                AdaptiveThresholdType.GaussianC,    //
                ThresholdType.Binary,               //
                5,                                  //
                0.2                                 //
                );

            // 3) paradit attelu
            img_MyImage.Source = ToBitmapSource(image);
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop
                  .Imaging.CreateBitmapSourceFromHBitmap(
                  ptr,
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        private void btn_MyButton_Click(object sender, RoutedEventArgs e)
        {
            timer.IsEnabled = !timer.IsEnabled;
            btn_MyButton.Content = timer.IsEnabled ? "Stop" : "Start";

        }

       
    }
}
