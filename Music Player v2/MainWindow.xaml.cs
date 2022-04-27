using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using WpfAnimatedGif;

namespace Music_Player_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static MainWindow _obj;
        private string pathFolder;
        private string pathCurrentPlaylist;
        private List<string> lstSongs = new List<string>();
        private List<string> lstShuffleSong = new List<string>();
        private List<string> lstPlaylistSongs = new List<string>();
        private List<string> lstPlaylistShuffleSongs = new List<string>();
        public static MainWindow Instance
        {
            get
            {
                if (_obj == null)
                {
                    _obj = new MainWindow();
                }
                return _obj;
            }
        }

        public string PathFolder { get => pathFolder; set => pathFolder = value; }
        public List<string> LstSongs { get => lstSongs; set => lstSongs = value; }
        public List<string> LstShuffleSong { get => lstShuffleSong; set => lstShuffleSong = value; }
        public string PathCurrentPlaylist { get => pathCurrentPlaylist; set => pathCurrentPlaylist = value; }
        public List<string> LstPlaylistSongs { get => lstPlaylistSongs; set => lstPlaylistSongs = value; }
        public List<string> LstPlaylistShuffleSongs { get => lstPlaylistShuffleSongs; set => lstPlaylistShuffleSongs = value; }

        public bool mediaPlayerIsPlaying = false;
        public bool isDraggingSlider = false;

        public VariableValue val = new VariableValue();
        public MainWindow()
        {
            SourceInitialized += Window1_SourceInitialized;
            InitializeFileXML();
            InitializeComponent();
            _obj = this;
            ReadDataXML();
            PathFolder = val.PathFolder;

            if (WMPlayer.Source == null)
            {
                NextSong.IsEnabled = false;
                PreviousSong.IsEnabled = false;
            }


            //Timer
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

          
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadSong();
        }

        void loadSong()
        {
            //Load Song Files
            foreach (string item in Directory.GetFiles(PathFolder, "*.mp3"))
            {
                LstSongs.Add(item);
                UCSong song = new UCSong(@item);
                PnlListSongs.Children.Add(song);
            }

            if (lbNotFoundSongs.Visibility == Visibility.Visible && PnlListSongs.Children.Count > 0)
            {
                lbNotFoundSongs.Visibility = Visibility.Collapsed;
            }

            //Trộn danh sách nhạc 
            LstShuffleSong = LstSongs.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((WMPlayer.Source != null) && (WMPlayer.NaturalDuration.HasTimeSpan) && (!isDraggingSlider))
            {
                NextSong.IsEnabled = true;
                PreviousSong.IsEnabled = true;

                sliderProgress.Minimum = 0;
                sliderProgress.Maximum = WMPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliderProgress.Value = WMPlayer.Position.TotalSeconds;

                if(WMPlayer.NaturalDuration.TimeSpan > TimeSpan.FromSeconds(3600))
                {
                    TimeStart.Text = WMPlayer.Position.ToString(@"h\:mm\:ss");
                    TimeEnd.Text = WMPlayer.NaturalDuration.TimeSpan.ToString(@"h\:mm\:ss");
                }
                else
                {
                    TimeStart.Text = WMPlayer.Position.ToString(@"mm\:ss");
                    TimeEnd.Text = WMPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                }

                //Nếu Time hiện tại bằng Time bài hát 
                if (WMPlayer.Position.TotalSeconds == WMPlayer.NaturalDuration.TimeSpan.TotalSeconds)
                {
                    if (!val.IsLoop)
                    {
                        NextSong_Click(null, null);
                    }
                    else
                    {
                        WMPlayer.Position = TimeSpan.FromSeconds(0);
                    }
                }
            }
        }

        #region Resize Window

        private const int WM_SYSCOMMAND = 0x112;
        private HwndSource hwndSource;

        private enum ResizeDirection
        {
            Left = 61441,
            Right = 61442,
            Top = 61443,
            TopLeft = 61444,
            TopRight = 61445,
            Bottom = 61446,
            BottomLeft = 61447,
            BottomRight = 61448,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private void Window1_SourceInitialized(object sender, EventArgs e)
        {
            hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)direction, IntPtr.Zero);
        }

        protected void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        protected void Resize(object sender, MouseButtonEventArgs e)
        {
            var clickedShape = sender as Shape;

            switch (clickedShape.Name)
            {
                case "ResizeN":
                    this.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "ResizeE":
                    this.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "ResizeS":
                    this.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "ResizeW":
                    this.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "ResizeNW":
                    this.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "ResizeNE":
                    this.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "ResizeSE":
                    this.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                case "ResizeSW":
                    this.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                default:
                    break;
            }
        }

        protected void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            var clickedShape = sender as Shape;

            switch (clickedShape.Name)
            {
                case "ResizeN":
                case "ResizeS":
                    this.Cursor = Cursors.SizeNS;
                    break;
                case "ResizeE":
                case "ResizeW":
                    this.Cursor = Cursors.SizeWE;
                    break;
                case "ResizeNW":
                case "ResizeSE":
                    this.Cursor = Cursors.SizeNWSE;
                    break;
                case "ResizeNE":
                case "ResizeSW":
                    this.Cursor = Cursors.SizeNESW;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Action Phụ
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnYourMusic_Click(object sender, RoutedEventArgs e)
        {
            PnlListSongs.Visibility = Visibility.Visible;
            PnlPlaylistSongs.Visibility = Visibility.Collapsed;
            ScrollView1.Visibility = Visibility.Visible;
            ScrollView2.Visibility = Visibility.Collapsed;
            if(PnlListSongs.Children.Count > 0)
            {
                lbNotFoundSongs.Visibility = Visibility.Collapsed;
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

        private void ChildAddtoPlaylist_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ChildNewPlaylist_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ChildFolderRequest_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region Action Media
        public void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                var controller = ImageBehavior.GetAnimationController(SoundWave);
                controller.Play();
            }
            
            if (PnlListSongs.Visibility == Visibility.Visible)
            {
                if (LstSongs.Count > 0 && LstShuffleSong.Count > 0)
                {
                    btnPlay.Visibility = Visibility.Collapsed;
                    btnPause.Visibility = Visibility.Visible;
                    mediaPlayerIsPlaying = true;

                    if (WMPlayer.Source == null)
                    {
                        if (val.IsShuffle)
                        {
                            WMPlayer.Source = new Uri(LstShuffleSong[0]);
                            loadDataSong();
                        }
                        else
                        {
                            WMPlayer.Source = new Uri(LstSongs[0]);
                            loadDataSong();
                        }
                    }
                }
            }
            else
            {
                if(LstPlaylistSongs.Count > 0 && LstPlaylistShuffleSongs.Count > 0)
                {
                    btnPlay.Visibility = Visibility.Collapsed;
                    btnPause.Visibility = Visibility.Visible;
                    mediaPlayerIsPlaying = true;

                    if (WMPlayer.Source == null)
                    {
                        if (val.IsShuffle)
                        {
                            WMPlayer.Source = new Uri(LstPlaylistShuffleSongs[0]);
                            loadDataSong();
                        }
                        else
                        {
                            WMPlayer.Source = new Uri(LstPlaylistSongs[0]);
                            loadDataSong();
                        }
                    }
                }    
            }

            WMPlayer.Play();
        }

        public void NextSong_Click(object sender, RoutedEventArgs e)
        {
            if (PnlListSongs.Visibility == Visibility.Visible)
            {
                if (val.IsShuffle)
                {
                    int indexSong = LstShuffleSong.IndexOf(WMPlayer.Source.OriginalString);
                    if (indexSong >= 0)
                    {
                        if (indexSong < LstShuffleSong.Count - 1)
                        {
                            WMPlayer.Source = new Uri(LstShuffleSong[++indexSong]);
                            loadDataSong();
                        }
                        else
                        {
                            WMPlayer.Source = new Uri(LstShuffleSong[0]);
                            loadDataSong();
                        }
                    }
                    else
                    {
                        WMPlayer.Source = new Uri(LstShuffleSong[0]);
                        loadDataSong();
                    }
                }
                else
                {
                    int indexSong = LstSongs.IndexOf(WMPlayer.Source.OriginalString);

                    if (indexSong < LstSongs.Count - 1)
                    {
                        WMPlayer.Source = new Uri(LstSongs[++indexSong]);
                        loadDataSong();
                    }
                    else
                    {
                        WMPlayer.Source = new Uri(LstSongs[0]);
                        loadDataSong();
                    }
                }
            }
            else
            {
                if (val.IsShuffle)
                {
                    int indexSong = LstPlaylistShuffleSongs.IndexOf(WMPlayer.Source.OriginalString);
                    if (indexSong >= 0)
                    {
                        if (indexSong < LstPlaylistShuffleSongs.Count - 1)
                        {
                            WMPlayer.Source = new Uri(LstPlaylistShuffleSongs[++indexSong]);
                            loadDataSong();
                        }
                        else
                        {
                            WMPlayer.Source = new Uri(LstPlaylistShuffleSongs[0]);
                            loadDataSong();
                        }
                    }
                    else
                    {
                        WMPlayer.Source = new Uri(LstPlaylistShuffleSongs[0]);
                        loadDataSong();
                    }
                }
                else
                {
                    int indexSong = LstPlaylistSongs.IndexOf(WMPlayer.Source.OriginalString);

                    if (indexSong < LstPlaylistSongs.Count - 1)
                    {
                        WMPlayer.Source = new Uri(LstPlaylistSongs[++indexSong]);
                        loadDataSong();
                    }
                    else
                    {
                        WMPlayer.Source = new Uri(LstPlaylistSongs[0]);
                        loadDataSong();
                    }
                }
            }

            btnPlay_Click(null, null);
        }

        public void PreviousSong_Click(object sender, RoutedEventArgs e)
        {
            if (PnlListSongs.Visibility == Visibility.Visible)
            {
                if (val.IsShuffle)
                {
                    int indexSong = LstShuffleSong.IndexOf(WMPlayer.Source.OriginalString);
                    if (indexSong <= LstShuffleSong.Count - 1)
                    {
                        if (indexSong > 0)
                        {
                            WMPlayer.Source = new Uri(LstShuffleSong[--indexSong]);
                            loadDataSong();
                        }
                        else
                        {
                            WMPlayer.Source = new Uri(LstShuffleSong[LstShuffleSong.Count - 1]);
                            loadDataSong();
                        }
                    }
                    else
                    {
                        WMPlayer.Source = new Uri(LstShuffleSong[LstShuffleSong.Count - 1]);
                        loadDataSong();
                    }
                }
                else
                {
                    int indexSong = LstSongs.IndexOf(WMPlayer.Source.OriginalString);

                    if (indexSong > 0)
                    {
                        WMPlayer.Source = new Uri(LstSongs[--indexSong]);
                        loadDataSong();
                    }
                    else
                    {
                        WMPlayer.Source = new Uri(LstSongs[LstSongs.Count - 1]);
                        loadDataSong();
                    }
                }
            }
            else
            {
                if (val.IsShuffle)
                {
                    int indexSong = LstPlaylistShuffleSongs.IndexOf(WMPlayer.Source.OriginalString);
                    if (indexSong <= LstPlaylistShuffleSongs.Count - 1)
                    {
                        if (indexSong > 0)
                        {
                            WMPlayer.Source = new Uri(LstPlaylistShuffleSongs[--indexSong]);
                            loadDataSong();
                        }
                        else
                        {
                            WMPlayer.Source = new Uri(LstPlaylistShuffleSongs[LstPlaylistShuffleSongs.Count - 1]);
                            loadDataSong();
                        }
                    }
                    else
                    {
                        WMPlayer.Source = new Uri(LstPlaylistShuffleSongs[LstPlaylistShuffleSongs.Count - 1]);
                        loadDataSong();
                    }
                }
                else
                {
                    int indexSong = LstPlaylistSongs.IndexOf(WMPlayer.Source.OriginalString);

                    if (indexSong > 0)
                    {
                        WMPlayer.Source = new Uri(LstPlaylistSongs[--indexSong]);
                        loadDataSong();
                    }
                    else
                    {
                        WMPlayer.Source = new Uri(LstPlaylistSongs[LstPlaylistSongs.Count - 1]);
                        loadDataSong();
                    }
                }
            }

            btnPlay_Click(null, null);
        }

        public void Loop_Click(object sender, RoutedEventArgs e)
        {
            if (val.IsLoop)
            {
                Loop.Foreground = new SolidColorBrush(Color.FromRgb(234, 234, 234));
                val.IsLoop = false;
            }
            else
            {
                Loop.Foreground = new SolidColorBrush(Color.FromRgb(29, 185, 84));
                val.IsLoop = true;
            }
        }

        public void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (val.IsShuffle)
            {
                Shuffle.Foreground = new SolidColorBrush(Color.FromRgb(234, 234, 234));
                val.IsShuffle = false;
            }
            else
            {
                Shuffle.Foreground = new SolidColorBrush(Color.FromRgb(29, 185, 84));
                val.IsShuffle = true;
            }
        }

        public void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if(this.Visibility == Visibility.Visible)
            {
                var controller = ImageBehavior.GetAnimationController(SoundWave);
                controller.Pause();
            }
            btnPlay.Visibility = Visibility.Visible;
            btnPause.Visibility = Visibility.Collapsed;
            mediaPlayerIsPlaying = false;
            WMPlayer.Pause();
        }

        private void SilderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
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
            WMPlayer.Volume = SliderVolume.Value;
            val.VolumeValue = SliderVolume.Value;
        }

        private void SliderVolume_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (SliderVolume.Value == 0)
            {
                volumeTemp = 0;
            }
        }

        double volumeTemp = 0;
        public void Volume_Click(object sender, RoutedEventArgs e)
        {
            if (VolumeIcon.Kind == MaterialDesignThemes.Wpf.PackIconKind.VolumeVariantOff)
            {
                val.VolumeValue = volumeTemp;
                if (val.VolumeValue > 0.4)
                {
                    VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh;

                    val.VolumeValue = volumeTemp;
                    SliderVolume.Value = val.VolumeValue;
                }
                else
                {
                    VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeMedium;

                    if (val.VolumeValue > 0)
                    {
                        SliderVolume.Value = val.VolumeValue;
                    }
                    else
                    {
                        SliderVolume.Value = 0.05;
                    }
                }
            }
            else
            {
                VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeVariantOff;
                volumeTemp = SliderVolume.Value;
                SliderVolume.Value = 0;
            }
        }

        public void sliderProgress_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDraggingSlider = true;
        }

        public void sliderProgress_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDraggingSlider = false;
            WMPlayer.Position = TimeSpan.FromSeconds(sliderProgress.Value);
        }

        public void loadDataSong()
        {
            try
            {
                TagLib.File tagFile = TagLib.File.Create(WMPlayer.Source.OriginalString);
                NameSong.Text = tagFile.Tag.Title;
                LyricNameSong.Text = NameSong.Text;
                NameArtist.Text = tagFile.Tag.FirstAlbumArtist;
                LyricNameArtist.Text = NameArtist.Text;

                string lyrics = tagFile.Tag.Lyrics;
                ScrollLyric.ScrollToVerticalOffset(0);

                if (lyrics != null && lyrics.Length > 50 && !lyrics.Contains("______________"))
                {
                    lyrics = lyrics.Replace("&#039,", "'").Replace("?", "'");
                    lyrics = Regex.Replace(lyrics, "\\[t1].*", "");
                    lyrics = Regex.Replace(lyrics, "\n{2,}", "\n");
                    Karaoke.IsEnabled = true;
                }
                else
                {
                    Karaoke.IsEnabled = false;
                    if(PnlLyric.Height != 0)
                    {
                        HideLyric();
                    }
                }
                Lyrics.Text = lyrics;
                try
                {
                    var bin = (byte[])(tagFile.Tag.Pictures[0].Data.Data);
                    ImageSourceConverter con = new ImageSourceConverter();
                    ThumbnailSong.ImageSource = (ImageSource)con.ConvertFrom(bin);
                }
                catch
                {
                    var bin = File.ReadAllBytes(@"..\..\..\Resources\no-cover.png");
                    ImageSourceConverter con = new ImageSourceConverter();
                    ThumbnailSong.ImageSource = (ImageSource)con.ConvertFrom(bin);
                }
            }
            catch { }
        }
        public void mixPlaylistShuffleSongs()
        {
            LstPlaylistShuffleSongs = LstPlaylistSongs.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(!tbSearch.IsFocused && !tbPlaylistName.IsFocused)
            {
                if (e.Key == Key.Space)
                {
                    if (btnPlay.Visibility == Visibility.Visible)
                    {
                        btnPlay_Click(null, null);
                    }
                    else
                    {
                        btnPause_Click(null, null);
                    }
                }

                if (e.Key == Key.M)
                {
                    Volume_Click(null, null);
                }

                if (e.Key == Key.S)
                { Shuffle_Click(null, null); }

                if (e.Key == Key.L)
                { Loop_Click(null, null); }

                if (e.Key == Key.LeftCtrl)
                {
                    PreviousSong_Click(null, null);
                }

                if (e.Key == Key.RightCtrl)
                {
                    NextSong_Click(null, null);
                }


                if (e.Key == Key.Left)
                {
                    if (WMPlayer.Source != null)
                    {
                        WMPlayer.Position -= TimeSpan.FromSeconds(5);
                    }
                }

                if (e.Key == Key.Right)
                {
                    if (WMPlayer.Source != null)
                    {
                        WMPlayer.Position += TimeSpan.FromSeconds(5);
                    }
                }

                if (e.Key == Key.Up)
                {
                    SliderVolume.Value += 0.1;
                }

                if (e.Key == Key.Down)
                {
                    SliderVolume.Value -= 0.1;
                }
            }
        }
        #endregion

        #region Request Folder Form
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            lbPathFolder.Text = val.PathFolder;
            FolderRequestForm.Visibility = Visibility.Visible;
        }

        private void btnChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folder = new VistaFolderBrowserDialog();
            if (folder.ShowDialog().GetValueOrDefault())
            {
                val.PathFolder = folder.SelectedPath;
                val.reloadPathSong();
                LstShuffleSong.Clear();
                LstShuffleSong = LstSongs.OrderBy(x => Guid.NewGuid()).ToList();

                FolderRequestForm.Visibility = Visibility.Collapsed;
                lbPathFolder.Text = val.PathFolder;
            }
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderRequestForm.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Change Mode
        private void btnMP3Mode_Click(object sender, RoutedEventArgs e)
        {
            var controller = ImageBehavior.GetAnimationController(SoundWave);
            controller.Pause();
            this.Hide();
            MP3Mode mp3 = new MP3Mode();
            mp3.ShowDialog();
            try
            {
                this.Show();
                if (btnPause.Visibility == Visibility.Visible)
                {
                    controller.Play();
                }
            }
            catch { }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            var controller = ImageBehavior.GetAnimationController(SoundWave);
            controller.Pause();
            this.Hide();
            Minimize min = new Minimize();
            min.ShowDialog();
            try
            {
                this.Show();
                if(btnPause.Visibility == Visibility.Visible)
                {
                    controller.Play();
                }
            }
            catch { }
        }
        #endregion

        #region XML File 

        public void InitializeFileXML()
        {
            bool isExisted = File.Exists(System.Windows.Forms.Application.StartupPath + @"\Data.xml");
            if (!isExisted)
            {
                XmlDocument doc = new XmlDocument();

                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);

                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlElement mainElement = doc.CreateElement(string.Empty, "MusicPlayer", string.Empty);
                doc.AppendChild(mainElement);

                XmlElement Data = doc.CreateElement(string.Empty, "Data", string.Empty);
                mainElement.AppendChild(Data);
                XmlElement Playlist = doc.CreateElement(string.Empty, "Playlists", string.Empty);
                mainElement.AppendChild(Playlist);

                XmlElement Path = doc.CreateElement(string.Empty, "Path", string.Empty);
                XmlText path = doc.CreateTextNode(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
                XmlElement LastSong = doc.CreateElement(string.Empty, "LastSong", string.Empty);
                XmlElement Volume = doc.CreateElement(string.Empty, "Volume", string.Empty);
                XmlText volume = doc.CreateTextNode("0.4");
                XmlElement Loop = doc.CreateElement(string.Empty, "Loop", string.Empty);
                XmlText loop = doc.CreateTextNode("false");
                XmlElement Shuffle = doc.CreateElement(string.Empty, "Shuffle", string.Empty);
                XmlText shuffle = doc.CreateTextNode("false");

                Path.AppendChild(path);
                Volume.AppendChild(volume);
                Loop.AppendChild(loop);
                Shuffle.AppendChild(shuffle);

                Data.AppendChild(Path);
                Data.AppendChild(LastSong);
                Data.AppendChild(Volume);
                Data.AppendChild(Loop);
                Data.AppendChild(Shuffle);

                doc.Save(System.Windows.Forms.Application.StartupPath + @"\Data.xml");
            }
        }

        public void ReadDataXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + @"\Data.xml");
            XmlElement root = doc.DocumentElement;
            XmlNodeList data = root.SelectNodes("Data");

            foreach (XmlNode item in data)
            {
                val.PathFolder = item.SelectSingleNode("Path").InnerText;
                try
                {
                    WMPlayer.Source = new Uri(item.SelectSingleNode("LastSong").InnerText);
                }
                catch { }
                SliderVolume.Value = Convert.ToDouble(item.SelectSingleNode("Volume").InnerText);
                val.IsLoop = Convert.ToBoolean(item.SelectSingleNode("Loop").InnerText);
                val.IsShuffle = Convert.ToBoolean(item.SelectSingleNode("Shuffle").InnerText);
            }

            LoadDataPlaylist();

            if (WMPlayer.Source != null)
            {
                loadDataSong();
            }
            if (val.IsLoop)
            {
                Loop.Foreground = new SolidColorBrush(Color.FromRgb(29, 185, 84));
            }
            if (val.IsShuffle)
            {
                Shuffle.Foreground = new SolidColorBrush(Color.FromRgb(29, 185, 84));
            }
        }

        void LoadDataPlaylist()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + @"\Data.xml");
            
            XmlElement root = doc.DocumentElement;
            XmlNode Playlists = root.SelectSingleNode("Playlists");
            XmlNodeList Playlist = Playlists.SelectNodes("Playlist");

            foreach (XmlNode item in Playlist)
            {
                string namePlaylist = item.SelectSingleNode("Name").InnerText;
                string pathPlaylist = item.SelectSingleNode("Path").InnerText;

                if (!Directory.Exists(item.SelectSingleNode("Path").InnerText))
                {
                    Playlists.RemoveChild(item);
                    doc.Save(System.Windows.Forms.Application.StartupPath + @"\Data.xml");
                }
                else
                {
                    UCPlaylist uc = new UCPlaylist(namePlaylist, pathPlaylist);
                    ListPlaylist.Children.Add(uc);
                }
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + @"\Data.xml");
            XmlElement root = doc.DocumentElement;

            XmlNode data = root.SelectSingleNode("Data");

            data.SelectSingleNode("Path").InnerText = val.PathFolder;
            if (WMPlayer.Source != null)
            {
                data.SelectSingleNode("LastSong").InnerText = WMPlayer.Source.OriginalString;
            }
            data.SelectSingleNode("Volume").InnerText = SliderVolume.Value.ToString();
            data.SelectSingleNode("Loop").InnerText = val.IsLoop.ToString();
            data.SelectSingleNode("Shuffle").InnerText = val.IsShuffle.ToString();

            doc.Save(System.Windows.Forms.Application.StartupPath + @"\Data.xml");
        }
        #endregion

        #region Playlist
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NewPlaylistForm.Visibility = Visibility.Visible;
            TitlePlaylist.Text = "Create Playlist";
            tbPlaylistName.Text = "";

            CardCreatePlaylist.Visibility = Visibility.Visible;
            CardRenamePlaylist.Visibility = Visibility.Collapsed;
        }
        private void AddtoPlaylistForm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddtoPlaylistForm.Visibility = Visibility.Collapsed;
        }

        private void btnAddNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (tbPlaylistName.Text != "")
            {
                if(!Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\Playlists"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + @"\Playlists");
                }
                string newPlaylistPath = System.Windows.Forms.Application.StartupPath + "Playlists\\" + tbPlaylistName.Text;
                if (!Directory.Exists(newPlaylistPath))
                {
                    Directory.CreateDirectory(newPlaylistPath);

                    XmlDocument doc = new XmlDocument();
                    doc.Load(System.Windows.Forms.Application.StartupPath + @"\Data.xml");
                    XmlElement root = doc.DocumentElement;
                    XmlNode Playlists = root.SelectSingleNode("Playlists");

                    XmlElement Playlist = doc.CreateElement(string.Empty, "Playlist", string.Empty);

                    XmlElement Name = doc.CreateElement(string.Empty, "Name", string.Empty);
                    Name.InnerText = tbPlaylistName.Text;

                    XmlElement Path = doc.CreateElement(string.Empty, "Path", string.Empty);
                    Path.InnerText = newPlaylistPath;

                    Playlist.AppendChild(Name);
                    Playlist.AppendChild(Path);

                    Playlists.AppendChild(Playlist);
                    doc.Save(System.Windows.Forms.Application.StartupPath + @"\Data.xml");

                    this.NewPlaylistForm.Visibility = Visibility.Collapsed;

                    UCPlaylist uc = new UCPlaylist(tbPlaylistName.Text,newPlaylistPath);
                    ListPlaylist.Children.Add(uc);

                    tbPlaylistName.Text = "";
                }
            }
        }

        private void btnRenamePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (tbPlaylistName.Text != "")
            {
                string folder = System.Windows.Forms.Application.StartupPath + @"Playlists\" + tbPlaylistName.Text;
                if (!Directory.Exists(folder))
                {
                    Directory.Move(PathCurrentPlaylist, folder);

                    XmlDocument doc = new XmlDocument();
                    doc.Load(System.Windows.Forms.Application.StartupPath + @"Data.xml");

                    XmlElement root = doc.DocumentElement;
                    XmlNode playlists = root.SelectSingleNode("Playlists");
                    XmlNode playlist = playlists.SelectSingleNode("Playlist[Path = '" + PathCurrentPlaylist + "']");

                    playlist["Name"].InnerText = tbPlaylistName.Text;
                    playlist["Path"].InnerText = folder;

                    doc.Save(System.Windows.Forms.Application.StartupPath + @"Data.xml");

                    ListPlaylist.Children.Clear();
                    LoadDataPlaylist();

                    NewPlaylistForm.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void tbPlaylistName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(CardCreatePlaylist.Visibility == Visibility.Visible)
                {
                    btnAddNewPlaylist_Click(null, null);
                }
                else
                {
                    btnRenamePlaylist_Click(null, null);
                }
            }
        }

        private void NewPlaylistForm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NewPlaylistForm.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Search

        bool isSearchArtist = false;
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(tbSearch.Text.Length != 0)
            {
                if(!isSearchArtist)
                {
                    btnClearSearch.Visibility = Visibility.Visible;
                    if (tbSearch.Text.Length > 1)
                    {
                        if (PnlListSongs.Children.Count > 0)
                        {
                            PnlSearchSongs.Children.Clear();
                            foreach (UCSong item in PnlListSongs.Children)
                            {
                                string nameSong = ConvertVietnamese(item.lbNameSong.Text);
                                string nameArtist = ConvertVietnamese(item.lbNameArtist.Text);
                                string textSearch = ConvertVietnamese(tbSearch.Text).Trim();
                                if (nameSong.Contains(textSearch) || nameArtist.Contains(textSearch))
                                {
                                    PnlSearchSongs.Children.Add(new UCSong(item.sPath));
                                }
                            }

                            if (PnlSearchSongs.Children.Count == 0)
                            {
                                lbNotFoundSongs.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ScrollView3.ScrollToVerticalOffset(0);
                                lbNotFoundSongs.Visibility = Visibility.Collapsed;
                            }

                            if (ScrollView2.Visibility == Visibility.Visible)
                            {
                                ScrollView2.Visibility = Visibility.Hidden;
                            }
                            ScrollView1.Visibility = Visibility.Collapsed;
                            ScrollView3.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    isSearchArtist = false;
                    btnClearSearch.Visibility = Visibility.Visible;
                    if (tbSearch.Text.Length > 1)
                    {
                        if (PnlListSongs.Children.Count > 0)
                        {
                            PnlSearchSongs.Children.Clear();
                            List<string> lstNameArtist = SplitArtist(tbSearch.Text.Trim());
                            foreach (UCSong item in PnlListSongs.Children)
                            {
                                string nameArtist = ConvertVietnamese(item.lbNameArtist.Text);
                                foreach(string sArtist in lstNameArtist)
                                {
                                    if (nameArtist.Contains(sArtist))
                                    {
                                        PnlSearchSongs.Children.Add(new UCSong(item.sPath));
                                        break;
                                    }
                                }   
                            }

                            if (PnlSearchSongs.Children.Count == 0)
                            {
                                lbNotFoundSongs.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ScrollView3.ScrollToVerticalOffset(0);
                                lbNotFoundSongs.Visibility = Visibility.Collapsed;
                            }

                            if (ScrollView2.Visibility == Visibility.Visible)
                            {
                                ScrollView2.Visibility = Visibility.Hidden;
                            }
                            ScrollView1.Visibility = Visibility.Collapsed;
                            ScrollView3.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            else
            {
                if (ScrollView2.Visibility == Visibility.Hidden)
                {
                    ScrollView2.Visibility = Visibility.Visible;
                }
                else
                {
                    ScrollView1.Visibility = Visibility.Visible;
                }
                ScrollView3.Visibility = Visibility.Collapsed;

                lbNotFoundSongs.Visibility = Visibility.Collapsed;
                btnClearSearch.Visibility = Visibility.Collapsed;
            }
        }

        string ConvertVietnamese(string s)
        {
            s = s.ToLower();
            s = Regex.Replace(s, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            s = Regex.Replace(s, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            s = Regex.Replace(s, "ì|í|ị|ỉ|ĩ|/g", "i");
            s = Regex.Replace(s, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            s = Regex.Replace(s, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            s = Regex.Replace(s, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            s = Regex.Replace(s, "đ", "d");
            return s;
        }

        List<string> SplitArtist(string s)
        {
            List<string> lstArtist = new List<string>();
            s = s.ToLower();
            s = Regex.Replace(s, ",|ft.|/|;|-| x|x | x |&|/g", " ");
            s = Regex.Replace(s, @"\s{2,}", "  ");
            foreach(string item in s.Split("  "))
            {
                lstArtist.Add(ConvertVietnamese(item));
            }

            return lstArtist;
        }

        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "";
            btnPlay.Focus();
        }

        private void NameSong_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbSearch.Text = NameSong.Text;
        }
        private void NameArtist_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isSearchArtist = true;
            tbSearch.Text = NameArtist.Text;
        }
        #endregion

        #region Lyric
        private void Karaoke_Click(object sender, RoutedEventArgs e)
        {
            if (PnlLyric.Height == 0)
            {
                ShowLyric();
            }
            else
            {
                HideLyric();
            }
        }
        public void ShowLyric()
        {
            Storyboard storyboard = new Storyboard();

            Duration duration = new Duration(TimeSpan.FromMilliseconds(300));
            CubicEase ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            DoubleAnimation animation = new DoubleAnimation();
            animation.EasingFunction = ease;
            animation.Duration = duration;
            storyboard.Children.Add(animation);
            animation.From = 0;
            animation.To = this.Height - 90;

            Storyboard.SetTarget(animation, PnlLyric);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));

            storyboard.Begin();
        }

        void HideLyric()
        {
            Storyboard storyboard = new Storyboard();

            Duration duration = new Duration(TimeSpan.FromMilliseconds(300));
            CubicEase ease = new CubicEase { EasingMode = EasingMode.EaseIn };

            DoubleAnimation animation = new DoubleAnimation();
            animation.EasingFunction = ease;
            animation.Duration = duration;
            storyboard.Children.Add(animation);
            animation.From = this.Height - 90;
            animation.To = 0;

            Storyboard.SetTarget(animation, PnlLyric);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));

            storyboard.Begin();
        }

        private void btnCloseLyrics_Click(object sender, RoutedEventArgs e)
        {
            if(PnlLyric.Height != 0)
            {
                HideLyric();
            }
        }
        #endregion
    }
}
