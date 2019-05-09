using AccountCenter.AppCode;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountCenter.Models.Data
{
    public class ContextString : DbContext
    {
        public ContextString(DbContextOptions<ContextString> options) : base(options)
        {
        }
  
        public DbSet<Account> Account { get; set; }
 
 

   
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>().ToTable("Account");
           
            modelBuilder.Entity<Menu>().ToTable("Menu");
            modelBuilder.Entity<Permission>().ToTable("Permission");
            modelBuilder.Entity<RolePermissions>().ToTable("RolePermissions");
            modelBuilder.Entity<Roles>().ToTable("Roles");
            modelBuilder.Entity<UserRoles>().ToTable("UserRoles");
          






        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Method.ContextStr);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
