using Hardcodet.Wpf.TaskbarNotification;
using IPdevices.Utils;
using IPdevices.View_Models;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace IPdevices
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    /// 
 
    public partial class App : Application
    {

        private TaskbarIcon notifyIcon;

        private Main main;

        private DispatcherTimer _retryCountDownTimer;

        private static int RETRY_INTERVAL = 10;

        private int _retryInterval = 5;

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            // TODO: Don't display splash screen during startup if chosen by user
            _retryInterval = RETRY_INTERVAL;

            ManualResetEvent mre = new ManualResetEvent(false);
            // start splash screen thread
            Thread thread = new Thread(
            new ThreadStart(
                delegate ()
                { 

                    SplashScreenHelper.SplashScreen = new Splash();

                    
                    SplashScreenHelper.Show();

                    mre.Set();

                    System.Windows.Threading.Dispatcher.Run();


                }
            ));


            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            // wait until thread initializes everything and then check
            mre.WaitOne();
          
            check();




        }


        private void check()
        {
            
            var clientid = (int) IPdevices.Properties.Settings.Default["clientid"];
            var deviceid = (int)IPdevices.Properties.Settings.Default["deviceid"];

            if (clientid > 0)
            {
                try
                {
                    SplashScreenHelper.ShowText("Looking for profile...");
                    using (ipdevicesEntities ipde = new ipdevicesEntities())
                    {
                        // does the client actually exist?
                        if (ipde.clients.Any(o => o.uid == clientid))
                        {

                            Console.WriteLine("Client id " + clientid + " was found");

                            SplashScreenHelper.ShowText("Profile found!");
                            var cl = new client();
                            cl = ipde.clients.Find(clientid);

                            SplashScreenHelper.ShowText("Looking for registered device...");

                            // check for registered devices
                            device dev = null;

                            if (cl.devices.Where(d => d.did == deviceid).DefaultIfEmpty(null).FirstOrDefault() != null)
                            {
                                SplashScreenHelper.ShowText("Device found!");
                                dev = ipde.devices.Find(deviceid);
                            }

                            // give client information and device information
                            main = new Main(cl, dev);
                            // apply the proper datacontet to the notify icon
                            notifyIcon = (TaskbarIcon) FindResource("MyNotifyIcon");
                            notifyIcon.DataContext = new NotifyIconViewModel((MainViewModel) main.DataContext);

                            SplashScreenHelper.Close();

                            main.Show();

                        }
                        else
                        {
                            // well clientid was found in settings, but it doesn't exist in the db. Then just make sure that we reset settings too!
                            Console.WriteLine("Client id " + clientid + " not found in db, resetting settings to default 0");

                            IPdevices.Properties.Settings.Default.clientid = 0;
                            IPdevices.Properties.Settings.Default.deviceId = 0;
                            IPdevices.Properties.Settings.Default.Save();

                            SplashScreenHelper.Close();

                            openLoginWindow();
                        }
                    }
                    ;
                }
                catch (Exception ex)
                {
                    retry();
                    Console.WriteLine("Error logging in: " + ex.Message);
                }
            }
            else
            {
                SplashScreenHelper.Close();
                openLoginWindow();
            }
            
        }

        private void retry()
        {
            _retryCountDownTimer = new DispatcherTimer();
            _retryCountDownTimer.Tick += (_, a) =>
            {
                // countdown reached zero, let's try to log back in 
                if (_retryInterval-- == 0)
                {
                    _retryCountDownTimer.Stop();
                    _retryInterval = RETRY_INTERVAL;
                    check();
                }
                else
                {
                    SplashScreenHelper.ShowText("There was a problem accessing your profile. Trying again in " + _retryInterval + " seconds");
                }
            };
            _retryCountDownTimer.Interval = new TimeSpan(0, 0, 0, 1);
            _retryCountDownTimer.IsEnabled = true;
            _retryCountDownTimer.Start();
        }

        private void openLoginWindow()
        {
            // since the clientid doesnt exist on this computer it means that there's no client and therefore no device
            LoginViewModel vm = new LoginViewModel();
            LoginWindow lw = new LoginWindow(vm);
            //  type of login
            vm.ClientLogin = new ClientLogin
            {
                AuthenticationMethod = new SignIn()
            };
            vm.LoginSuccess += (client) =>
            {
                Main m = new Main(client, null);
                m.Show();
                lw.Close();
            };

            lw.Show();
        }



        protected override void OnExit(ExitEventArgs e)
        {
            if(notifyIcon!=null)
            {
                notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            }
            Console.WriteLine("Closing application!");
            base.OnExit(e);
        }

    }
}
