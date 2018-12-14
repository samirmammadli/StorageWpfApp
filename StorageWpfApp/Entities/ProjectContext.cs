using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StorageWpfApp.Entities
{
    public class ProjectContext : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().Property(c => c.Debit).HasDefaultValue(0);
            modelBuilder.Entity<Product>().Property(p => p.IsPieceProduct).HasDefaultValue(false);
            modelBuilder.Entity<Product>().HasIndex(i => i.Code).IsUnique();
            modelBuilder.Entity<Consignment>().HasOne(p => p.Product).WithMany(c => c.Consignments).HasForeignKey(k => k.ProductId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
           
            optionbuilder.UseSqlite(@"Data Source=Storage.db");
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductGroup> Groups { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Consignment> Consignments { get; set; }

        public DbSet<SingleOrder> SingleOrders { get; set; }

        public DbSet<PieceOrder> PieceOrders { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Debt> Debts { get; set; }

        public DbSet<DebtPayment> DebtPayments { get; set; }
    }
}