using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Music_Player_v2
{
    /// <summary>
    /// Interaction logic for MP3Mode.xaml
    /// </summary>
    public partial class MP3Mode : Window
    {
        private bool isJustOpen = true;
        public MP3Mode()
        {
            InitializeComponent();

            Shuffle.Foreground = MainWindow.Instance.Shuffle.Foreground;
            Loop.Foreground = MainWindow.Instance.Loop.Foreground;
            NextSong.IsEnabled = MainWindow.Instance.NextSong.IsEnabled;
            PreviousSong.IsEnabled = MainWindow.Instance.PreviousSong.IsEnabled;
            btnPlay.Visibility = MainWindow.Instance.btnPlay.Visibility;
            btnPause.Visibility = MainWindow.Instance.btnPause.Visibility;
            NameSong.Text = MainWindow.Instance.NameSong.Text;
            NameArtist.Text = MainWindow.Instance.NameArtist.Text;
            sliderProgress.Maximum = MainWindow.Instance.sliderProgress.Maximum;
            SliderVolume.Value = MainWindow.Instance.SliderVolume.Value;
            TimeStart.Text = MainWindow.Instance.TimeStart.Text;
            TimeEnd.Text = MainWindow.Instance.TimeEnd.Text;

            if(MainWindow.Instance.btnPlay.Visibility == Visibility.Visible)
            {
                ThumbnailSong.IsEnabled = false;
            }
            else
            {
                ThumbnailSong.IsEnabled = true;
            }

            if (MainWindow.Instance.ThumbnailSong.ImageSource != null)
            {
                ThumbnailSong.Source = MainWindow.Instance.ThumbnailSong.ImageSource;
            }

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

            if(!MainWindow.Instance.isDraggingSlider)
            {
                sliderProgress.Value = MainWindow.Instance.sliderProgress.Value;
            }
            sliderProgress.Maximum = MainWindow.Instance.sliderProgress.Maximum;
            TimeStart.Text = MainWindow.Instance.TimeStart.Text;
            TimeEnd.Text = MainWindow.Instance.TimeEnd.Text;

            if (MainWindow.Instance.ThumbnailSong.ImageSource != null)
            {
                ThumbnailSong.Source = MainWindow.Instance.ThumbnailSong.ImageSource;
            }
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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnMP3Mode_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Minimize min = new Minimize();
            min.ShowDialog();
            this.ShowDialog();
        }

        #region Media 
        private void PreviousSong_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.PreviousSong_Click(null, null);
            btnPlay.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Visible;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.btnPlay_Click(null, null);
            ThumbnailSong.IsEnabled = true;
            btnPlay.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Visible;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.btnPause_Click(null, null);
            ThumbnailSong.IsEnabled = false;
            btnPlay.Visibility = Visibility.Visible;
            btnPause.Visibility = Visibility.Collapsed;
        }

        private void NextSong_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.NextSong_Click(null, null);
            btnPlay.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Visible;
        }

        private void Loop_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Loop_Click(null, null);
            if (MainWindow.Instance.val.IsLoop)
            {
                Loop.Foreground = new SolidColorBrush(Color.FromRgb(29, 185, 84));
            }
            else
            {
                Loop.Foreground = new SolidColorBrush(Color.FromRgb(234, 234, 234));
            }
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Shuffle_Click(null, null);
            if (MainWindow.Instance.val.IsShuffle)
            {
                Shuffle.Foreground = new SolidColorBrush(Color.FromRgb(29, 185, 84));
            }
            else
            {
                Shuffle.Foreground = new SolidColorBrush(Color.FromRgb(234, 234, 234));
            }
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
        #endregion

        #region Volume
        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Volume_Click(null, null);
            VolumeIcon.Kind = MainWindow.Instance.VolumeIcon.Kind;
            SliderVolume.Value = MainWindow.Instance.SliderVolume.Value;
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
                VolumeIcon.Kind = MainWindow.Instance.VolumeIcon.Kind;
            }

        }

        #endregion
    }
}
