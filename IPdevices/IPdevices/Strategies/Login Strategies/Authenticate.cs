using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPdevices
{
    class Authenticate : ILogin
    {


        public client Login(string username, string password)
        {
            client c = null;
            try
            {
                var clientid = Properties.Settings.Default.clientid;
                using (var context = new ipdevicesEntities())
                {
                    // Are we just authenticating?
            
                    c = context.clients.Where(b =>(b.Username == username || b.Email == username) &&  b.uid == clientid).DefaultIfEmpty(null).FirstOrDefault();
                    if (c == null)
                        return null;

                    if (Utils.PasswordHelper.BCrypt.CheckPassword(password, c.Password))
                    {
                        Console.WriteLine("Logged in via authentication");
                        return c;
                    }

                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

       
        }
    }
}
