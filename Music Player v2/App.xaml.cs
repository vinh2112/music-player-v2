using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Music_Player_v2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string cls, string win);
        [DllImport("user32")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32")]
        static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32")]
        static extern bool OpenIcon(IntPtr hWnd);


        private static Mutex _mutex = null;

        private static string _fileName = null;

        //public static string FileName
        //{
        //    get { return App._fileName; }
        //    set { App._fileName = value; }
        //}

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            const string appName = "Music Player";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                ActivateOtherWindow();
                //app is already running! Exiting the application  
                Application.Current.Shutdown();
            }
        }
        private static void ActivateOtherWindow()
        {
            var other = FindWindow(null, "Music Player");
            if (other != IntPtr.Zero)
            {
                SetForegroundWindow(other);
                if (IsIconic(other))
                    OpenIcon(other);
            }
        }
    }
}
