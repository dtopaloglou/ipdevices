using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using IPdevices.Commands;

namespace IPdevices.View_Models
{
    public class RegisterViewModel : ViewModelBase, INotifyDataErrorInfo
    {

        private string _DeviceName = "";

        /// <summary>
        /// Message to display in UI during registration process
        /// </summary>

        private string _RegisterMsg;

        public string RegisterMsg
        {
            get
            {
                return _RegisterMsg;
            }
            set
            {
                _RegisterMsg = value;
                OnPropertyChanged("RegisterMsg");
            }
        }



        public string DeviceName
        {
            get { return _DeviceName; }
            set
            {
                _DeviceName = value;
                OnPropertyChanged("DeviceName");
                validateDeviceName(_DeviceName);
            }
        }

        /// <summary>
        /// Device location
        /// </summary>
        private string _Location = "";

        public string Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
                OnPropertyChanged("Location");
            }
        }


        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private readonly Dictionary<string, ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();

        private void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }

        /// <summary>
        /// This example was taken from here: https://blog.magnusmontin.net/2013/08/26/data-validation-in-wpf/
        /// </summary>
        /// <param name="name"></param>
        public async void validateDeviceName(string name)
        {
            const string propertyKey = "DeviceName";
            ICollection<string> validationErrors = null;
            /* Call service asynchronously */
            bool isValid = await Task.Run(() =>
            {

                DeviceService deviceService = new DeviceService();
                return deviceService.ValidateDeviceName(DeviceName, out validationErrors);

            })
            .ConfigureAwait(false);

            if (!isValid)
            {
                /* Update the collection in the dictionary returned by the GetErrors method */
                _validationErrors[propertyKey] = validationErrors;
                /* Raise event to tell WPF to execute the GetErrors method */
                RaiseErrorsChanged(propertyKey);
            }
            else if (_validationErrors.ContainsKey(propertyKey))
            {
                /* Remove all errors for this property */
                _validationErrors.Remove(propertyKey);
                /* Raise event to tell WPF to execute the GetErrors method */
                RaiseErrorsChanged(propertyKey);
            }
        }

        /// <summary>
        /// Checks if errors exist
        /// </summary>
        public bool HasErrors
        {
            get { return _validationErrors.Count > 0; }
        }

        public RegisterViewModel()
        {
            // give it this computer's name
            DeviceName = System.Environment.MachineName;
        }

        /// <summary>
        /// Register device command
        /// </summary>
        public ICommand Register
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    int clientid = (int)Properties.Settings.Default["clientId"];

                    try
                    {
                        RegisterMsg = "Registering device...please wait";
                        using (var context = new ipdevicesEntities())
                        {
                       
                            // save as new device
                            device newDevice = new device
                            {
                                Name = DeviceName,
                                uid = clientid,
                                Location = this.Location
                            };

                            context.devices.Add(newDevice);
                            context.SaveChanges();

                            // save device id in user properties for later us
                            Properties.Settings.Default.deviceId = newDevice.did;
                            Properties.Settings.Default.Save();

                            // handle the regisration success and pass new device model
                            RegisterSuccess(newDevice);


                            Console.WriteLine("New device id " + IPdevices.Properties.Settings.Default["deviceId"] );
                        }

                        RegisterMsg = "";


                    }
                    catch (EntityException ex)
                    {

                        MessageBox.Show("Error: " + ex.Message, "Error");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error");
                    }

                
                }, (o) => allow());
            }
        }


        /// <summary>
        /// Determines if registration button is enabled
        /// </summary>
        /// <returns></returns>
        private bool allow()
        {
            return !HasErrors;
        }

        /// <summary>
        /// Callback used when device registration successful
        /// </summary>
        public event Action<device> RegisterSuccess;
    }
}
