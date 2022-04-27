using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using WpfAnimatedGif;
using System.Xml;
using System.Text.RegularExpressions;

namespace Music_Player_v2
{
    /// <summary>
    /// Interaction logic for UCSong.xaml
    /// </summary>
    public partial class UCSong : UserControl
    {
        private string path;
        private string nameSong;
        private string nameArtist;
        private ImageSource imageSource;

        public string sPath { get => path; set => path = value; }
        public string NameSong { get => nameSong; set => nameSong = value; }
        public string NameArtist { get => nameArtist; set => nameArtist = value; }
        public ImageSource ImageSource { get => imageSource; set => imageSource = value; }

        public UCSong(string Path)
        {
            InitializeComponent();
            this.sPath = Path;
            //loadData();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (MainWindow.Instance.WMPlayer.Source != null)
            {
                if (MainWindow.Instance.WMPlayer.Source.OriginalString == sPath && PlayingImage.Source == null)
                {
                    string filePath = System.IO.Path.Combine(System.IO.Path.GetFullPath(@"..\..\..\"), "Resources");

                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(filePath + "\\PlayingImageGreen.gif");
                    image.EndInit();
                    ImageBehavior.SetAnimatedSource(PlayingImage, image);
                    BackgroundPlayingImage.Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
                    Container.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#333333");
                }
                else
                {
                    if (MainWindow.Instance.WMPlayer.Source.OriginalString != sPath)
                    {
                        ImageBehavior.SetAnimatedSource(PlayingImage, null);
                        BackgroundPlayingImage.Background = Brushes.Transparent;
                        Container.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#181818");
                    }
                }

                var controller = ImageBehavior.GetAnimationController(PlayingImage);

                if (MainWindow.Instance.btnPlay.Visibility == Visibility.Visible && PlayingImage.Source != null)
                {
                    controller.Pause();
                }
                else
                {
                    if (MainWindow.Instance.btnPause.Visibility == Visibility.Visible && PlayingImage.Source != null)
                    {
                        controller.Play();
                    }
                }
            }
        }
        private void loadData()
        {

            TagLib.File tagFile = TagLib.File.Create(@sPath);
            lbNameSong.Text = tagFile.Tag.Title;
            lbNameArtist.Text = tagFile.Tag.FirstAlbumArtist;

            string lyrics = tagFile.Tag.Lyrics;

            if (lyrics != null && lyrics.Length > 50 && !lyrics.Contains("______________"))
            {
                Karaoke.Visibility = Visibility.Visible;
            }

            if (tagFile.Properties.Duration.TotalSeconds > 3600)
            {
                DurationTime.Text = TimeSpan.FromSeconds(tagFile.Properties.Duration.TotalSeconds).ToString(@"h\:mm\:ss");
            }
            else
            {
                DurationTime.Text = TimeSpan.FromSeconds(tagFile.Properties.Duration.TotalSeconds).ToString(@"mm\:ss");
            }

            NameSong = tagFile.Tag.Title;
            NameArtist = tagFile.Tag.FirstAlbumArtist;

            try
            {
                var bin = (byte[])(tagFile.Tag.Pictures[0].Data.Data);

                ImageSourceConverter con = new ImageSourceConverter();
                ThumbnailSong.ImageSource = (ImageSource)con.ConvertFrom(bin);
                ImageSource = (ImageSource)con.ConvertFrom(bin);
            }
            catch
            {

            }

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (MainWindow.Instance.WMPlayer.Source != new Uri(sPath))
                {
                    if (e.ClickCount == 2)
                    {
                        MainWindow.Instance.WMPlayer.Source = new Uri(@sPath);

                        MainWindow.Instance.loadDataSong();

                        MainWindow.Instance.btnPlay_Click(null, null);
                    }

                }
                else
                {
                    MainWindow.Instance.btnPlay_Click(null, null);
                }

            }

        }

        private void btnAddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.AddtoPlaylistForm.Visibility = Visibility.Visible;
            MainWindow.Instance.PnlAddToPlaylist.Children.Clear();

            XmlDocument doc = new XmlDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + @"\Data.xml");
            XmlElement root = doc.DocumentElement;

            XmlNode Playlists = root.SelectSingleNode("Playlists");
            XmlNodeList Playlist = Playlists.SelectNodes("Playlist");

            foreach (XmlNode item in Playlist)
            {
                string namePlaylist = item.SelectSingleNode("Name").InnerText;
                string pathPlaylist = item.SelectSingleNode("Path").InnerText;
                UCPlaylist uc = new UCPlaylist(namePlaylist, pathPlaylist, sPath);
                MainWindow.Instance.PnlAddToPlaylist.Children.Add(uc);
            }
        }

        private void Karaoke_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Instance.WMPlayer.Source != new Uri(sPath))
            {
                MainWindow.Instance.WMPlayer.Source = new Uri(@sPath);
                MainWindow.Instance.btnPlay_Click(null, null);
                MainWindow.Instance.loadDataSong();
                MainWindow.Instance.ShowLyric();

            }
            else
            {
                if (MainWindow.Instance.PnlLyric.Height == 0)
                {
                    MainWindow.Instance.ShowLyric();
                }
            }
        }

        private void btnAddToPlaylist_MouseEnter(object sender, MouseEventArgs e)
        {
            btnAddToPlaylist.Foreground = Brushes.Red;
        }

        private void btnAddToPlaylist_MouseLeave(object sender, MouseEventArgs e)
        {
            btnAddToPlaylist.Foreground = Brushes.White;
        }

        private void Container_MouseEnter(object sender, MouseEventArgs e)
        {
            btnAddToPlaylist.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FAFAFA");
            Karaoke.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FAFAFA");
        }

        private void Container_MouseLeave(object sender, MouseEventArgs e)
        {
            btnAddToPlaylist.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#999999");
            Karaoke.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#999999");
        }

        private void BackgroundPlayingImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.Instance.WMPlayer.Source != new Uri(sPath))
            {
                MainWindow.Instance.WMPlayer.Source = new Uri(@sPath);
                MainWindow.Instance.btnPlay_Click(null, null);
                MainWindow.Instance.loadDataSong();
            }
            else
            {
                MainWindow.Instance.btnPlay_Click(null, null);
            }
        }
    }
}
