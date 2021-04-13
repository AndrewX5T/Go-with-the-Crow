using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Go_with_the_Crow.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Go_with_the_Crow.Models.Database
{
    public class Context : DbContext
    {
        public Context() : base("name=Go_with_the_Crow") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<IdentityUserRole>();
            modelBuilder.Ignore<IdentityUserLogin>();
        }

        public DbSet<Store.Bird> Birds { get; set; }

        public DbSet<Store.Adoption> Adoptions { get; set; }

    }
}