using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPdevices.Errors
{
    class CustomError
    {
        public string ValidationMessage
        {
            get; set;
        }

        public CustomError(string msg)
        {
            ValidationMessage = msg;
        }
    }
}
