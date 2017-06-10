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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Reflection;
using System.Timers;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using IPdevices.Utils;

namespace IPdevices
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {

        private int _numOfNotifs = 0;

        public Main(client cl, device d)
        {
            InitializeComponent();
            DataContext = new MainViewModel(cl, d);
            this.Title = this.Title + " v" + Utility.GetPublishedVersion();
        }


 
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
        }

        // minimize to system tray when applicaiton is closed
        protected override void OnClosing(CancelEventArgs e)
        {
            // setting cancel to true will cancel the close request
            // so the application is not closed

            e.Cancel = true;
            this.Hide();

            base.OnClosing(e);

            if(_numOfNotifs++ == 0)
            {
                var tb = (TaskbarIcon)FindResource("MyNotifyIcon");

                tb.ShowBalloonTip("IPDevices Notification", "Application is still running in the background", BalloonIcon.Info);
            }

            // one problem is that when you click on "CLose All Windows" from the Taskbar, it works, however it doesn't work for windows that are dialogs.
            // Try opening the preferences window and then try to Close All Windows: there preferences window will close, but not the main window 
        }


    }
}
