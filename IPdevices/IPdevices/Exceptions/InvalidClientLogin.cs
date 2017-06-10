using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPdevices.Exceptions
{
    public class InvalidClientLoginException : System.Exception
    {
        // The default constructor needs to be defined
        // explicitly now since it would be gone otherwise.

        public InvalidClientLoginException()
        {
           
        }

        public InvalidClientLoginException(string message) : base(message)
        {
        }

        public InvalidClientLoginException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
