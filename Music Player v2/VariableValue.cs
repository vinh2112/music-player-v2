using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Music_Player_v2
{
    public class VariableValue
    {
        private bool isLoop = false;
        private bool isShuffle = false;
        private double volumeValue;
        private string song;
        private string pathSong;

        public bool IsLoop { get => isLoop; set => isLoop = value; }
        public bool IsShuffle { get => isShuffle; set => isShuffle = value; }
        public double VolumeValue { get => volumeValue; set => volumeValue = value; }
        public string Song { get => song; set => song = value; }
        public string PathFolder { get => pathSong; set => pathSong = value; }

        public void reloadPathSong()
        {
            MainWindow.Instance.LstSongs.Clear();
            MainWindow.Instance.PnlListSongs.Children.Clear();

            foreach (string item in Directory.GetFiles(PathFolder, "*.mp3"))
            {
                MainWindow.Instance.LstSongs.Add(item);
                UCSong song = new UCSong(@item);
                MainWindow.Instance.PnlListSongs.Children.Add(song);
            }
            if (MainWindow.Instance.PnlListSongs.Children.Count == 0)
            {
                MainWindow.Instance.lbNotFoundSongs.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
