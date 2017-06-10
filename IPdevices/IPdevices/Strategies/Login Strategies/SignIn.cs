using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPdevices
{
    public class SignIn : ILogin
    {
     
        public client Login(string username, string password)
        {
            client c = null;
            try
            {
                using (var context = new ipdevicesEntities())
                {

                    c = context.clients.Where(b => b.Username == username || b.Email == username).DefaultIfEmpty(null).FirstOrDefault();
                    if (c == null)
                        return null;

                    if (Utils.PasswordHelper.BCrypt.CheckPassword(password, c.Password))
                    {
                        Console.WriteLine("Logged in via sign in");
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
