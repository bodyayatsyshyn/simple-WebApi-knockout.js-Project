using System;
using System.Collections.Generic;
using System.Text;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DBLayer
{
    public class WebAppDbContext : DbContext
    {
        public WebAppDbContext(DbContextOptions options) : base(options)
        {
        }
        public WebAppDbContext()
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Details> Details { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Invoices)
                .WithOne(i => i.Employee)
                .HasForeignKey(e => e.EmployeeId);

            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Details)
                .WithOne(s => s.Invoice)
                .HasForeignKey(s => s.InvoiceId);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Sum)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.ExecutionDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Details>()
                .Property(s => s.Sum)
                .HasPrecision(19, 4);
        }
    }
}
