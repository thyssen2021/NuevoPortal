﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal_2_0.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Portal_2_0Entities : DbContext
    {
        public Portal_2_0Entities()
            : base("name=Portal_2_0Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<plantas> plantas { get; set; }
        public virtual DbSet<empleados> empleados { get; set; }
        public virtual DbSet<puesto> puesto { get; set; }
    }
}
