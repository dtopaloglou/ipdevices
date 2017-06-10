using IPdevices.Commands;
using IPdevices.View_Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;

namespace IPdevices
{
    public class MainViewModel : ViewModelBase
    {
        private int _Uid; 
        public int Uid
        {
            get { return _Uid; }
            set
            {
                _Uid = value;
                OnPropertyChanged("Uid");
            }
        }

        private string _ClientEmail;



        public string ClientEmail
        {
            get { return _ClientEmail; }
            set
            {
                _ClientEmail = value;
                OnPropertyChanged("ClientEmail");
            }
        }

        private client _client;

        public client Client
        {
            get { return _client; }
            set
            {
                _client = value;
                if(_client != null)
                {

                    Uid = _client.uid;
                    ClientEmail = _client.Email;
                    CImageSource = "/Resources/check.png";
                }
                else
                {
                    CImageSource = "/Resources/x.png";
                }

                //OnPropertyChanged("Uid");
            }
        }

        private device _device;

        public device Device
        {
            get { return _device; }
            set
            {
                _device = value;
                if (_device == null)
                {
                    enableRegistration(true);
                    enableSignIn(true);
              
                }
                else
                {
                    Console.WriteLine(" deviced id " + Device.did);
                    DeviceName = _device.Name;
                    DeviceLocation = _device.Location;
                    enableRegistration(false);
                    enableSignIn(false);
                }
            }
        }

        public string ADD
        {
            get { return _add; }
            set
            {
                _add = value;
                OnPropertyChanged("ADD");
            }
        }

        private string _OldAdd
        {
            get { return (string) Properties.Settings.Default["lastip"]; }
            set
            {
                Properties.Settings.Default.lastip = value;
                Properties.Settings.Default.Save();
            }
        }

        public string TimeFound
        {
            get { return _timeFound; }
            set
            {
                _timeFound = value;
                OnPropertyChanged("TimeFound");
            }
        }

        public string Timer
        {
            get { return _timer; }
            set
            {
                _timer = value;
                OnPropertyChanged("Timer");
            }
        }

        private string _DeviceLocation;

        public string DeviceLocation
        {
            get
            {
                return _DeviceLocation;
                ;
            }
            set
            {
                _DeviceLocation = value;
                OnPropertyChanged("DeviceLocation");
            }
        }

        private string _DeviceName;

        public string DeviceName
        {
            get { return _DeviceName; }
            set
            {
                _DeviceName = value;
                OnPropertyChanged("DeviceName");
            }
        }

        private string _DImageSource;

        public string DImageSource
        {
            get { return _DImageSource; }
            set
            {
                _DImageSource = value;
                OnPropertyChanged("DImageSource");
            }
        }

        private string _CImageSource;

        public string CImageSource
        {
            get { return _CImageSource;  }
            set
            {
                _CImageSource = value;
                OnPropertyChanged("CImageSource");
            }
        }


        private bool _IsUnregisterEnable;

        public bool IsUnregisterEnable
        {
            get { return _IsUnregisterEnable; }
            set
            {
                _IsUnregisterEnable = value;
                OnPropertyChanged("IsUnregisterEnable");
            }
        }

        private bool _IsRegisterEnable;


        public bool IsRegisterEnable
        {
            get { return _IsRegisterEnable; }
            set
            {
                _IsRegisterEnable = value;
                OnPropertyChanged("IsRegisterEnable");
            }
        }

        private bool _isRefreshEnable = true;

        public bool isRefreshEnable
        {
            get { return _isRefreshEnable; }
            set
            {
                _isRefreshEnable = value;
                OnPropertyChanged("isRefreshEnable");
            }
        }

        private bool _IsSigninEnable;

        public bool IsSigninEnable
        {
            get
            {
                return _isRefreshEnable;
                
            }
            set
            {
                _isRefreshEnable = value;
                OnPropertyChanged("IsSigninEnable");
            }
        }

        private string _add = "";

        private string _timer;

        private string _timeFound;

        private readonly DispatcherTimer _countDownTimer;

        private readonly IPret _iprep;

        private DateTime _now;

        private DateTime _next;

        private int _numUpdates = 0;


        public MainViewModel(client cl, device d)
        {
            Client = cl;
            Device = d;

            _countDownTimer = new DispatcherTimer();
            _countDownTimer.Tick += async (o, e) => await countdownTimer();
            _countDownTimer.Interval = new TimeSpan(0,0,0,1);
            _countDownTimer.IsEnabled = true;
            _countDownTimer.Start();
            _iprep = new IPret();
            restartCountdown();
        }

        #region Commands

        public ICommand RegisterDevice
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    var vm = new RegisterViewModel();
                    var reg = new Register(vm);
                    // was registration successful?
                    vm.RegisterSuccess += (device) =>
                    {
                        Device = device; // assign new device
                        refresIp(new object());
                        reg.Close();
                    };
                    
                    reg.ShowDialog();
                });
            }
        }

        public ICommand CopyToClipboard
        {
            get { return new RelayCommand<Object>(copyToClipboard); }
        }

        private void copyToClipboard(object o)
        {
            System.Windows.Clipboard.SetText(ADD);
        }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand<Object>((o) =>
                {
                    Console.WriteLine("Closing application...");
                    App.Current.Shutdown();
                });
            }
        }


        public ICommand SignIn
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    LoginViewModel vm = new LoginViewModel();
                    LoginWindow lw = new LoginWindow(vm);

                    // type of login
                    vm.ClientLogin = new ClientLogin
                    {
                        AuthenticationMethod = new SignIn()
                    };

                    vm.LoginSuccess += (client) =>
                    {
                        try
                        {


                            // remove device IN CASE it does exist. This is pretty redundant
                            if (Device != null)
                            {
                                using (var db = new ipdevicesEntities())
                                {
                                    db.devices.Attach(Device);
                                    db.devices.Remove(Device);
                                    db.SaveChanges();
                                }
                            }



                            // since new user is coming in, we need to stop tracking current device and then give new user

                            var deviceid = (int)Properties.Settings.Default["deviceid"];
                            // get client device
                            using (var context = new ipdevicesEntities())
                            {
                                var currentDevice = context.clients.Find(_client.uid).devices.Where(d => d.did == deviceid).DefaultIfEmpty(null).FirstOrDefault();
                                // well it doest exist, remove it from db
                                if (currentDevice != null)
                                {
                                    context.devices.Remove(currentDevice);
                                    context.SaveChanges();
                                }
                                // remove it from settings
                                Properties.Settings.Default.deviceId = 0;
                                Properties.Settings.Default.Save();
                                // delete device pointer
                                Device = null;
                            }

                            Client = client;

                        }
                        catch(Exception)
                        {
                            MessageBox.Show("An error occurred while loggin in. Please try again. ", "Error");
                        }
                        finally
                        {
                            
                            lw.Close();
                        }
                        
                    };


                    // we want it to be a modalwindow so the user can't click on any other window
        
                    lw.ShowDialog();
                });
            }
        }


        public ICommand RefreshIP
        {
            get { return new RelayCommand<object>(refresIp); }
        }

        public async void refresIp(object o)
        {
            int clientid = Properties.Settings.Default.clientid;

            if (Utils.Utility.CheckConnection())
            {

                if (ClientLogin.IsClientAlive(Properties.Settings.Default.clientid))
                {
                    Console.WriteLine("Checking ip...");

                    // let's do this task async since it might take some time
                    await Task.Run(() => { _iprep.refresh(); });
                    assignAndUpdate(_iprep.getExternalIp());
                    Console.WriteLine("...finished checking");
                }
                else
                {
                    // client no longet exists. stop everything.
                    Client = null;
                    Device = null;
                    Properties.Settings.Default.deviceId = 0;
                    Properties.Settings.Default.clientid = 0;
                    Properties.Settings.Default.lastip = "";
                    Properties.Settings.Default.Save();

                    Console.WriteLine("Client no longer exists in db. Stop timer and reset settings");

                }
            }
            else
            {
                assignAndUpdate("Connection Error");
            }
        }

        private async Task countdownTimer()
        {
            if (Client == null)
            {
                this.Timer = "";
                _countDownTimer.Stop();
                _countDownTimer.IsEnabled = false;
            }

            var timeRemaining = _next - DateTime.Now;

            var t = timeRemaining.Hours + "h " + timeRemaining.Minutes + "m " + timeRemaining.Seconds + "s";
            this.Timer = t;
            var secondsRemaining = (int) timeRemaining.TotalSeconds;

            if (_numUpdates == 0 || secondsRemaining == 0)
            {
                _countDownTimer.Stop();
                isRefreshEnable = false;
                this.Timer = "executing...";

                await Task.Run(() => { refresIp(new object()); });

                _countDownTimer.Start();
                isRefreshEnable = true;

            }
        }

        public ICommand ShowPreferences
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    var p = new Preferences();
                    p.Owner = App.Current.MainWindow;
                    p.ShowDialog();
                });
            }
        }


        private void assignAndUpdate(string ip)
        {
            DateTime now = DateTime.Now;
            string suffix = now.ToString("tt");

            ADD = _iprep.getExternalIp(); // new ip

            TimeFound = DateTime.Now.ToString("h:mm:ss") + " " + suffix;

            Console.WriteLine(" Old ip : " + _OldAdd);

            // only update if the ip address is different
            if (!_OldAdd.Equals(ADD))
            {
                SaveIp(ip);
            }

            // do not remove this or the execute will happen over and over.
            _numUpdates++;
            restartCountdown();
        }

        private void restartCountdown()
        {
            _now = DateTime.Now;
            int interval = (int) Properties.Settings.Default["refreshInterval"];
            _next = _now.AddSeconds(interval);
        }


        /// <summary>
        /// Updates IP Address in database. It inserts a new row into the database for the device in question. Done asynchornously no to freeze UI
        /// </summary>
        /// <param name="ip"></param>
        public async void SaveIp(string ip)
        {
           
            int deviceid = Properties.Settings.Default.deviceId;
            // check if client or device exists
            if (deviceid <= 0)
                return;

            await Task.Run(() =>
            {
                try
                {
                    using (var context = new ipdevicesEntities())
                    {
                        device d = context.devices.Find(deviceid);
                        /// does the device really exist in db?
                        if (d != null)
                        {
                            d.ips.Add(new ip
                            {
                                IP = ip,
                                Date = DateTime.Now
                            });
                            context.SaveChanges();
                        }
                        else
                        {
                            // if it doesn't, remove device from application registration
                            Console.WriteLine("Device no longer in database. Removed.");
                            Device = null;
                        }


                        _OldAdd = ip; // save ip in settings for later comparisoons
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("An unknown error occurred while updating ip address");
                }
            });
        }


        /// <summary>
        /// Unregisters current device and re-enables device registration. It removes the device id and last ip from the settings as well.
        /// </summary>
        public ICommand UnregisterDevice
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {


                    LoginViewModel vm = new LoginViewModel();
                    LoginWindow lw = new LoginWindow(vm);
                    vm.ClientLogin = new ClientLogin {AuthenticationMethod = new Authenticate()};

                 
                    vm.LoginSuccess += (client) =>
                    {
                        try
                        {

                            /// THIS STUFF BELOW SHOULD BE REFACTORED SINCE THE EditLogin method also stops tracking device!!!

                            var deviceid = (int)Properties.Settings.Default["deviceId"];
                            using (var context = new ipdevicesEntities())
                            {
                                // if the deviced id exists in the db, erase it (stop tracking it)
                                if (context.devices.Any(d=> d.did == deviceid))
                                {
                                    device device = new device
                                    {
                                        did = deviceid
                                    };
                                    context.devices.Attach(device);
                                    context.devices.Remove(device);
                                    context.SaveChanges();
                                }
                                // remove device from this computer
                                Properties.Settings.Default.deviceId = 0;
                                Properties.Settings.Default.Save();
                                Device = null;
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("There was an error while trying to removing device. Please try again.", "Error");
                        }
                        finally
                        {
                            // close the login window
                            lw.Close(); 
                        }
                    };

                    lw.ShowDialog();


                    
                });
            }
        }

        public ICommand OpenMyDevices
        {
            get
            {
                return new RelayCommand<object>((o0) =>
                {
                    var vm = new LoginViewModel();
                    var lw = new LoginWindow(vm);
                    vm.ClientLogin = new ClientLogin { AuthenticationMethod = new Authenticate() };
                    lw.Show();
                    vm.LoginSuccess += (client) =>
                    {
                        lw.Close();
                        var m = new MyDevices();
                        m.Owner = App.Current.MainWindow;
                        m.ShowDialog();
                    };
                });
            }
        }



        #endregion

        /// <summary>
        /// Determines whether the UI should enable or disable device registration button
        /// </summary>
        /// <param name="e"></param>
        private void enableRegistration(bool e)
        {
            if (e)
            {
                DImageSource = "/Resources/x.png";
                IsRegisterEnable = true;
                IsUnregisterEnable = false;
                DeviceName = "NO DEVICE";
                DeviceLocation = "-";

                // if no device, then there shouldn't be a device id in the settings
                Properties.Settings.Default.deviceId = 0;
                Properties.Settings.Default.lastip = "";
                Properties.Settings.Default.Save();
            }
            else
            {
                DImageSource = "/Resources/check.png";
                IsRegisterEnable = false;
                IsUnregisterEnable = true;
            }
        }

        private void enableSignIn(bool e)
        {
            IsSigninEnable = e;
        }
    }
}