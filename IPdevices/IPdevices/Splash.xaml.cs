using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Deployment.Application;
using IPdevices.Utils;

namespace IPdevices
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();
            DataContext = new SplashScreenViewModel();

            version.Content = "v" + Utility.GetPublishedVersion();

        }



        /// <summary>
        /// Enables to drag and drop splash screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Splash_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

      
    }

}
