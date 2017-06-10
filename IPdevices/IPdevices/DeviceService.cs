using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.ClientServices;

namespace IPdevices
{
    /// <summary>
    /// DeviceService class for device
    /// </summary>
    class DeviceService
    {

        /// <summary>
        /// Checks whether the given device name is already taken
        /// </summary>
        /// <param name="name"></param>
        /// <param name="validationErrors"></param>
        /// <returns></returns>
        public bool ValidateDeviceName(string name, out ICollection<string> validationErrors)
        {
            validationErrors = new List<string>();


            // verify that a device name is given
            if (string.IsNullOrWhiteSpace(name))
                validationErrors.Add("Device name is required");

            using (var context = new ipdevicesEntities())
            {
                int clientid = (int)Properties.Settings.Default["clientid"];
                if (DeviceExists(name, clientid))
                {
                    validationErrors.Add("The device name already exists.");
                }
            }

            return validationErrors.Count == 0;
        }

        /// <summary>
        /// Returns true of false if the device name exists for particular client
        /// </summary>
        /// <param name="name">Device name</param>
        /// <param name="clientid">Client id</param>
        /// <returns></returns>
        public static bool DeviceExists(string name,  int clientid)
        {
            bool exists = false;
            try
            {
                using (var context = new ipdevicesEntities())
                {

                    if (context.devices.Any(u => u.uid == clientid && u.Name == name))
                    {
                        exists = true;
                    }
                }
            }
            catch (Exception)
            {

                exists = false;
            }

            return exists;

        }
    }
}
