using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IPdevices
{
    public class ClientLogin
    {

        private string _username;

        private string _password;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set { _password = value; }
        }

        private client _client;

        public client Client
        {
            get { return _client; }
        }

        private device _device;

        public device Device
        {
            get
            {
                return _device;
            }
        }

        /// <summary>
        /// This is the authentication strategy
        /// </summary>
        public ILogin AuthenticationMethod { get; set; }

        public ClientLogin()
        {
         
            _device = null;
            _client = null;
        }

        /// <summary>
        /// Authenticate client
        /// </summary>
        /// <returns></returns>
        public  bool login()
        {

            var client = AuthenticationMethod.Login(Username, Password);

            if (client != null)
            {
                _client = client;
                Properties.Settings.Default.clientid = _client.uid;
                Properties.Settings.Default.Save();

                return true;
            }
            return false;
   
        }


        /// <summary>
        /// Checks whether the client actually exist in the database. Used to check prior to ip update.
        /// </summary>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public static bool IsClientAlive(int clientid)
        {
            try
            {
                using (var context = new ipdevicesEntities())
                {
                    return context.clients.Any(c => c.uid == clientid);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error during client alive check" + ex.Message);
            }
            return false;

        }
    }
}
