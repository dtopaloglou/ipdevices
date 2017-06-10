using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPdevices
{
    class Interval
    {

        public string Name { get; set;}

        public int Value { get; set; }

        public Interval()
        {

        }

        public Interval(string name, int value)
        {
            Name = name;
            Value = value;
        }


    }
}
