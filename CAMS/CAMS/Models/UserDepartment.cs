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
    
    public partial class UserDepartment
    {
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public string AccessType { get; set; }
    
        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
    }
}