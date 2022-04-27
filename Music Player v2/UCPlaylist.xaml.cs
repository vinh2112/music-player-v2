using System;
using System.Collections.Generic;
using System.IO;
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

namespace Music_Player_v2
{
    /// <summary>
    /// Interaction logic for UCPlaylist.xaml
    /// </summary>
    public partial class UCPlaylist : UserControl
    {
        private string path;
        private string pathSong;
        public UCPlaylist(string NamePlaylist, string Path, string PathSong = null)
        {
            InitializeComponent();
            this.Width = MainWindow.Instance.ListPlaylist.Width;
            lbNamePlaylist.Text = NamePlaylist;
            this.sPath = Path;
            if(PathSong != null)
            {
                this.PathSong = PathSong;
            }
        } 
        public string sPath { get => path; set => path = value; }
        public string PathSong { get => pathSong; set => pathSong = value; }

        private void ButtonPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if(this.PathSong != null) // khác Null thì dành cho Add to Playlist
            {
                string fileName = System.IO.Path.GetFileName(PathSong);
                if(!File.Exists(sPath + "\\" + fileName))
                {
                    File.Copy(PathSong, sPath + "\\" + fileName, true);
                    MainWindow.Instance.AddtoPlaylistForm.Visibility = Visibility.Collapsed;
                }
            }
            else // == null thì dành cho hiện Playlist
            {
                MainWindow.Instance.PnlListSongs.Visibility = Visibility.Collapsed;
                MainWindow.Instance.PnlPlaylistSongs.Visibility = Visibility.Visible;
                MainWindow.Instance.ScrollView2.Visibility = Visibility.Visible;
                MainWindow.Instance.ScrollView1.Visibility = Visibility.Collapsed;

                MainWindow.Instance.PnlPlaylistSongs.Children.Clear();

                MainWindow.Instance.LstPlaylistSongs.Clear();
                MainWindow.Instance.LstPlaylistShuffleSongs.Clear();

                foreach (string item in Directory.GetFiles(sPath, "*.mp3"))
                {
                    if (MainWindow.Instance.lbNotFoundSongs.Visibility == Visibility.Visible)
                    {
                        MainWindow.Instance.lbNotFoundSongs.Visibility = Visibility.Collapsed;
                    }
                    MainWindow.Instance.LstPlaylistSongs.Add(item);
                    UCSongPlaylist song = new UCSongPlaylist(@item);
                    MainWindow.Instance.PnlPlaylistSongs.Children.Add(song);
                }

                //Trộn danh sách nhạc 
                if (MainWindow.Instance.LstPlaylistSongs.Count > 0)
                {
                    MainWindow.Instance.mixPlaylistShuffleSongs();
                }
                else
                {
                    MainWindow.Instance.lbNotFoundSongs.Visibility = Visibility.Visible;
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Directory.Delete(sPath, true);
            MainWindow.Instance.ListPlaylist.Children.Remove(this);
            if(MainWindow.Instance.PathCurrentPlaylist == sPath)
            {
                MainWindow.Instance.PnlPlaylistSongs.Children.Clear();
                MainWindow.Instance.lbNotFoundSongs.Visibility = Visibility.Visible;
            }
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.NewPlaylistForm.Visibility = Visibility.Visible;
            MainWindow.Instance.TitlePlaylist.Text = "Rename Playlist";
            MainWindow.Instance.CardCreatePlaylist.Visibility = Visibility.Collapsed;
            MainWindow.Instance.CardRenamePlaylist.Visibility = Visibility.Visible;
            MainWindow.Instance.tbPlaylistName.Text = lbNamePlaylist.Text;
            MainWindow.Instance.PathCurrentPlaylist = sPath;
        }
    }
}
