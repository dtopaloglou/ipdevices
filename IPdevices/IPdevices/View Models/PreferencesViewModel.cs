using IPdevices.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace IPdevices
{
    class PreferencesViewModel : ViewModelBase
    {
        public ObservableCollection<Interval> Intervals { get; private set; }

        public bool AutoStartCheckBoxIsChecked
        {
            get; set;
        }

        private int _selectedIntervalValue;
        public int SelectedIntervalValue
        {
            get
            {
                return _selectedIntervalValue;
            }
            set
            {
                _selectedIntervalValue = value;
            }
        }

        public PreferencesViewModel()
        {
            Intervals = new ObservableCollection<Interval>();

            AutoStartCheckBoxIsChecked = Properties.Settings.Default.aStart;

            Intervals.Add(new Interval("5 minutes", 5 * 60));
            Intervals.Add(new Interval("15 minutes", 15 * 60));
            Intervals.Add(new Interval("1 hour", 60 * 60));
            Intervals.Add(new Interval("6 hours", 60 * 60 * 6));
        }

        #region Commands
        public ICommand SaveInterval
        {
            get
            {
                return new RelayCommand<Object>(saveInterval);
            }
        }

        private void saveInterval(object o)
        {
            Properties.Settings.Default.refreshInterval = _selectedIntervalValue;
            Properties.Settings.Default.Save();
            Console.WriteLine("Saved!");

        }

        public ICommand EnableSystemService
        {
            get; set;
        }

        public ICommand AutoStart
        {
            get
            {
                return new RelayCommand<bool>(registerStartup);
            }
        }

        private void registerStartup(bool isChecked)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (isChecked)
            {
                registryKey.SetValue("IPDevices", Application.ExecutablePath);
                Properties.Settings.Default.aStart = true;

            }
            else
            {
                if(registryKey.GetValue("IPDevices") != null)
                {
                    registryKey.DeleteValue("IPDevices");
                }
                
                Properties.Settings.Default.aStart = false;
            }

            Properties.Settings.Default.Save();
        }

        #endregion


    }
}
