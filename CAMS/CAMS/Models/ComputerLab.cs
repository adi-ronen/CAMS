//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CAMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ComputerLab
    {
        public int LabId { get; set; }
        public int ComputerId { get; set; }
        public System.DateTime Entrance { get; set; }
        public Nullable<System.DateTime> Exit { get; set; }
    
        public virtual Computer Computer { get; set; }
        public virtual Lab Lab { get; set; }
    }
}