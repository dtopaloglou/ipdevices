using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace IPdevices
{
    /// <summary>
    /// This class checks for an IP address
    /// </summary>
    class IPret
    {

        private string _external = "";

        private static string IP_PATTERN = @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";

        private bool _found = false;
        public IPret()
        {
     
        }
 
        private void retrieve()
        {
            try
            {

                Console.WriteLine("...getting ip from server");
                _external = (new WebClient()).DownloadString("http://ipdevices.icubicksolutions.com/ip.php");  // this url shouldb't be hardcoded either
                _external = (new Regex(IP_PATTERN)).Matches(_external)[0].ToString();

                _found = true;
            }
            catch
            {
                Console.WriteLine("An error occured while fetching IP");
                _found = false;
            }
        }

        public string getExternalIp()
        {
            return _external;
        }

        public bool isFound()
        {
            return _found;
        }

        public void refresh()
        {
            _found = false;
            retrieve();
        }
    }
}
