using IPdevices.Commands;
using IPdevices.Errors;
using IPdevices.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IPdevices.Exceptions;


namespace IPdevices.View_Models
{

    public class LoginViewModel : ViewModelBase
    {


  

        private string _username;
        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;

            }
        }




        /// <summary>
        /// This is the callback method to fire upon login. It returns the client instance and the type of authentication that was made.
        /// </summary>
        public Action<client> LoginSuccess
        {
            get; set;
        }

        private SecureString _password;

        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {

                _password = value;
                OnPropertyChanged("Password");
 
            }
        }
 

        private string _loginMessage;

        public string LoginMessage
        {
            get
            {
                return _loginMessage;
            }
            set
            {
                _loginMessage = value;
                OnPropertyChanged("LoginMessage");
            }
        }



        public LoginViewModel()
        {
            Password = new SecureString();
        }

        public ICommand SignIn
        {
            get
            {
                return new RelayCommand<object>(signin, o => canLogin());
            }
        }



        private bool canLogin()
        {
            return true;
        }

        public ClientLogin ClientLogin { get; set; }


        private async  void signin(object o)
        {

            if (UserName == "" || Password.Length == 0)
            {
                LoginMessage = "Please enter a username or password";
            }
            else
            {
                LoginMessage = "Logging in...please wait";

                if (ClientLogin == null)
                    throw new InvalidClientLoginException("No ClientLogin assigned.");

                ClientLogin.Password = PasswordHelper.ConvertToUnsecureString(Password);
                ClientLogin.Username = UserName;


                // this might take a while so we need a task or else ui wont update
                Task<bool> logging  = Task.Run(()=> ClientLogin.login() );

                bool isloggedin  = await logging;


                /*
                Thread t = new Thread(new ThreadStart(delegate
                {
                    ClientLogin.login();
                }));*/


                if (isloggedin)
                {

                    Console.WriteLine("Client found!");
                    resetForm();
                    LoginSuccess(ClientLogin.Client);   
                }
                else
                {
                    LoginMessage = "Username or password incorrect!";
                }
            }
        }

        private void resetForm()
        {
            LoginMessage = "";
            Password = new SecureString();
            UserName = "";
        }
    }


}
