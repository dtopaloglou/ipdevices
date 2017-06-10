using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IPdevices.Utils
{
    /// <summary>
    /// Basic uitily class with some helper methods
    /// </summary>
    class Utility
    {
        /// <summary>
        /// Checks internet connection with a given URL
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static bool CheckConnection(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 5000;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK) return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether server is online
        /// </summary>
        /// <returns></returns>
        public static bool CheckConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://ipdevices.icubicksolutions.com"))  // this url shouldn't really be hard-coded
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the application's published deployed version
        /// </summary>
        /// <returns></returns>
        public static string GetPublishedVersion()
        {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(3);
            }
            else
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            }
      
        }

        
    }
}
