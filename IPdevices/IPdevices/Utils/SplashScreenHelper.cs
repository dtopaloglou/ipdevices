using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IPdevices.Utils
{
    class SplashScreenHelper
    {

        public static Splash SplashScreen { get; set; }

        public static void Show()
        {
            if (SplashScreen != null)
            {
                SplashScreen.Show();
            }
                
        }

        public static void Hide()
        {
            if (SplashScreen == null) return;

            if (!SplashScreen.Dispatcher.CheckAccess())
            {
                Thread thread = new Thread(
                    new ThreadStart(
                        delegate ()
                        {
                            SplashScreen.Dispatcher.Invoke(
                                DispatcherPriority.Normal,
                                new Action(delegate ()
                                {
                                    Thread.Sleep(2000);
                                    SplashScreen.Hide();
                                }
                            ));
                        }
                ));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            else
                SplashScreen.Hide();
        }

        public static void Close()
        {
            if (SplashScreen == null) return;
            if (!SplashScreen.Dispatcher.CheckAccess())
            {
                SplashScreen.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Action(delegate()
                    {
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
                        SplashScreen.Close();
                    }));
            }
            else
            {
                SplashScreen.Close();
            }
                

            Console.WriteLine("Splashscreen closed");
        }

        /// <summary>
        /// We use the dispatcher’s CheckAccess() method to determine if the current thread has access to the dispatcher, 
        /// which means we can just issue a call directly upon the View. If we don’t have access, we must use the dispatcher to invoke the necessary command.
        /// </summary>
        /// <param name="text"></param>
        public static void ShowText(string text)
        {
            if (SplashScreen == null) return;

            if (!SplashScreen.Dispatcher.CheckAccess())
            {

                SplashScreen.Dispatcher.Invoke(
                    DispatcherPriority.Normal,

                    new Action(delegate ()
                    {
                        ((SplashScreenViewModel)SplashScreen.DataContext).SplashScreenText = text;
                    }
                ));
   
            }
            else
            {
                ((SplashScreenViewModel)SplashScreen.DataContext).SplashScreenText = text;
            }
                
        }
    }

    
}
