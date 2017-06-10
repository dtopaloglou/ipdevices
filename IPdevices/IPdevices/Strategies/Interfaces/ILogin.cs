using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPdevices
{
    public interface ILogin
    {
        client Login(string username, string password);
    }
}
