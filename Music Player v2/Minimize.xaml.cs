using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Music_Player_v2
{
    /// <summary>
    /// Interaction logic for Minimize.xaml
    /// </summary>
    public partial class Minimize : Window
    {
        private bool isJustOpen = true;

        public Minimize()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = 0;

            NextSong.IsEnabled = MainWindow.Instance.NextSong.IsEnabled;
            PreviousSong.IsEnabled = MainWindow.Instance.PreviousSong.IsEnabled;
            btnPlay.Visibility = MainWindow.Instance.btnPlay.Visibility;
            btnPause.Visibility = MainWindow.Instance.btnPause.Visibility;
            NameSong.Text = MainWindow.Instance.NameSong.Text;
            NameArtist.Text = MainWindow.Instance.NameArtist.Text;
            sliderProgress.Maximum = MainWindow.Instance.sliderProgress.Maximum;

            SliderVolume.Value = MainWindow.Instance.SliderVolume.Value;
            VolumeIcon.Kind = MainWindow.Instance.VolumeIcon.Kind;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            NextSong.IsEnabled = MainWindow.Instance.NextSong.IsEnabled;
            PreviousSong.IsEnabled = MainWindow.Instance.PreviousSong.IsEnabled;

            NameSong.Text = MainWindow.Instance.NameSong.Text;
            NameArtist.Text = MainWindow.Instance.NameArtist.Text;


            if (!MainWindow.Instance.isDraggingSlider)
            {
                sliderProgress.Value = MainWindow.Instance.sliderProgress.Value;
            }
            sliderProgress.Maximum = MainWindow.Instance.sliderProgress.Maximum;

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
                if (this.Left > (SystemParameters.WorkArea.Width - this.Width))
                {
                    this.Left = SystemParameters.WorkArea.Width - this.Width;
                }
                if (this.Left < 0)
                {
                    this.Left = 0;
                }
                if (this.Top < 0)
                {
                    this.Top = 0;
                }
                if (this.Top > SystemParameters.WorkArea.Height - this.Height)
                {
                    this.Top = SystemParameters.WorkArea.Height - this.Height;
                }
            }
            catch { }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.btnPlay_Click(null, null);
            btnPlay.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Visible;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.btnPause_Click(null, null);
            btnPlay.Visibility = Visibility.Visible;
            btnPause.Visibility = Visibility.Collapsed;
        }

        private void PreviousSong_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.PreviousSong_Click(null, null);
            btnPlay.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Visible;
        }

        private void NextSong_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.NextSong_Click(null, null);
            btnPlay.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Visible;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void sliderProgress_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            MainWindow.Instance.isDraggingSlider = true;
        }

        private void sliderProgress_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            MainWindow.Instance.isDraggingSlider = false;
            MainWindow.Instance.WMPlayer.Position = TimeSpan.FromSeconds(sliderProgress.Value);
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(isJustOpen)
            {
                isJustOpen = false;
            }
            else
            {
                MainWindow.Instance.SliderVolume.Value = SliderVolume.Value;

                if (SliderVolume.Value < 0.4)
                {
                    VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeMedium;
                    if (SliderVolume.Value == 0)
                    {
                        VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeVariantOff;
                    }
                }
                else
                {
                    VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh;
                }
            }
           
        }

        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Volume_Click(null, null);
            SliderVolume.Value = MainWindow.Instance.SliderVolume.Value;
        }
    }
}
