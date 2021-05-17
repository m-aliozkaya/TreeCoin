using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TreeCoinUI.Entity;

namespace TreeCoinUI.Identity
{
    public class IdentityDataContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityDataContext() : base("dataConnection")
        {
            Database.SetInitializer(new IdentityDataInitializer());
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Quantity> Quantities { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SupplierProduct> SupplierProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<MoneyConfirm> MoneyConfirms { get; set; }
        public DbSet<FinanceHistory> FinanceHistories { get; set; }
        public DbSet<FinanceType> FinanceTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasRequired(t => t.User)
                .WithMany(t => t.Customers)
                .HasForeignKey(d => d.UserId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Supplier>()
                .HasRequired(t => t.User)
                .WithMany(t => t.Suppliers)
                .HasForeignKey(d => d.UserId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Order>()
                .HasRequired(t => t.Customer)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.CustomerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasRequired(t => t.Supplier)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.SupplierId)
                .WillCascadeOnDelete(false);



        }
    }
}