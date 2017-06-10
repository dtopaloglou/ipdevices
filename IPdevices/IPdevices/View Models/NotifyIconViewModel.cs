using IPdevices.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IPdevices.View_Models
{
    class NotifyIconViewModel
    {

        MainViewModel _MainViewModel;

        public MainViewModel MainViewModel
        {
            get
            {
                return _MainViewModel;
            }
        }

        public NotifyIconViewModel(MainViewModel mainViewModel)
        {
            _MainViewModel = mainViewModel;

        }

        public ICommand ShowApp
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    var window = App.Current.MainWindow;
                    
                        window.Show();
                        
                    

                });
            }
        }

        public ICommand ExitApplication
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    Application.Current.Shutdown();
                });
            }
        }
    }
}
