//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IPdevices
{
    using System;
    using System.Collections.Generic;
    
    public partial class ip
    {
        public int id { get; set; }
        public int did { get; set; }
        public string IP { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual device device { get; set; }
    }
}
