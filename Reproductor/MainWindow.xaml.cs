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

using Microsoft.Win32;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using System.Windows.Threading;

namespace Reproductor
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer timer;

        //Lector de archivos
        AudioFileReader reader;
        //comunicacion con la tarjeta de audio
        //exclusivo para salida
        WaveOut output;

        bool dragging = false;
        

        public MainWindow()
        {
            InitializeComponent();
            ListarDispositivosSalida();

            cbDispositivoSalida.Visibility = Visibility.Collapsed;
            pbCancion.Visibility = Visibility.Hidden;
            txt1.Visibility = Visibility.Hidden;
            txt2.Visibility = Visibility.Hidden;
            txt3.Visibility = Visibility.Hidden;
            txt4.Visibility = Visibility.Hidden;
            txt5.Visibility = Visibility.Hidden;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;

            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(!dragging)
            {
                pbCancion.Value = reader.CurrentTime.TotalSeconds;
                if (pbCancion.Value >= 4.5 && pbCancion.Value <= 19)
                {
                    txt1.Visibility = Visibility.Visible;
                }
                else if (pbCancion.Value >= 20 && pbCancion.Value <= 28)
                {
                    txt1.Visibility = Visibility.Hidden;
                    txt2.Visibility = Visibility.Visible;
                }
                else if (pbCancion.Value >= 29 && pbCancion.Value <= 44)
                {
                    txt2.Visibility = Visibility.Hidden;
                    txt3.Visibility = Visibility.Visible;
                }
                else if(pbCancion.Value >= 46 && pbCancion.Value <=55)
                {
                    txt3.Visibility = Visibility.Hidden;
                    txt4.Visibility = Visibility.Visible;
                }
                else if(pbCancion.Value >= 56)
                {
                    txt4.Visibility = Visibility.Hidden;
                    txt5.Visibility = Visibility.Visible;
                }
            }

        }

        void ListarDispositivosSalida()
        {
            cbDispositivoSalida.Items.Clear();
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities capacidades = WaveOut.GetCapabilities(i);
                cbDispositivoSalida.Items.Add(capacidades.ProductName);
            }
            cbDispositivoSalida.SelectedIndex = 0;
            
        }

        private void BtnReproducir_Click(object sender, RoutedEventArgs e)
        {
            btnReproducir.Visibility = Visibility.Collapsed;
            pbCancion.Visibility = Visibility.Visible;
            reader = new AudioFileReader("../../cancion/Himno.mp3");
            output = new WaveOut();
            output.DeviceNumber = cbDispositivoSalida.SelectedIndex;
            output.PlaybackStopped += Output_PlaybackStopped;
            output.Init(reader);
            output.Play();

            pbCancion.Maximum = reader.TotalTime.TotalSeconds;
            pbCancion.Value = reader.CurrentTime.TotalSeconds;

            timer.Start();
        }

            private void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            reader.Dispose();
            output.Dispose();
            timer.Stop();
            btnReproducir.Visibility = Visibility.Visible;
            pbCancion.Visibility = Visibility.Hidden;
        }

       

        private void SldTiempo_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragging = true;
        }

        private void SldTiempo_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragging = false;
            if (reader != null && output != null && output.PlaybackState != PlaybackState.Stopped)
            {
                reader.CurrentTime = TimeSpan.FromSeconds(sldTiempo.Value);
            }
        }

    }
}
