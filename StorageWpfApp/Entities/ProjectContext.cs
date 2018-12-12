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
        
        public ProjectContext()
        {


            //Database.EnsureCreated();

            //if (true)
            //{
            //    if (!Groups.Any())
            //    {
            //        Groups.AddRange(
            //            new ProductGroup { Name = "Гвозди" },
            //            new ProductGroup { Name = "Краны" },
            //            new ProductGroup { Name = "Споты" }
            //            );
            //    }

            //    var rnd = new Random();
            //    for (int i = 0; i < 10000; i++)
            //    {
            //        var IsPiece = rnd.Next(2) == 0 ? false : true;
            //        var prd = new Product { Code = Guid.NewGuid().ToString().Substring(0, 10), IsPieceProduct = IsPiece, Name = Guid.NewGuid().ToString().Substring(0, 8), GroupId = rnd.Next(1, 4) };
            //        if (IsPiece)
            //        {
            //            prd.PieceQuantity = rnd.Next(10, 150);
            //        }
            //        Products.Add(prd);
            //    }

            //    SaveChanges();

            //    for (int i = 0; i < 10000; i++)
            //    {
            //        var index = rnd.Next(1, 150);
            //        var prd = Products.FirstOrDefault(p => p.Id == index);
            //        var cons = new Consignment { Date = new DateTime(rnd.Next(2015, 2019), rnd.Next(1, 13), rnd.Next(1, 28)), IsPieceAllowed = prd.IsPieceProduct, ProductId = index, PurchasePrice = rnd.Next(10, 150), Quantity = i, SellingPrice = rnd.Next(10, 150) };
            //        if (prd.IsPieceProduct)
            //        {
            //            cons.CurrentPieceQuantity = prd.PieceQuantity.Value * i;
            //            cons.PiecePrice = rnd.Next(10, 150);
            //        }
            //        Consignments.Add(cons);
            //    }


            //    SaveChanges();
            //}


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().Property(c => c.Debit).HasDefaultValue(0);
            modelBuilder.Entity<Product>().Property(p => p.IsPieceProduct).HasDefaultValue(false);
            modelBuilder.Entity<Product>().HasIndex(i => i.Code).IsUnique();
            modelBuilder.Entity<Consignment>().HasOne(p => p.Product).WithMany(c => c.Consignments).HasForeignKey(k => k.ProductId);


            modelBuilder.Entity<Invoice>().HasOne(d => d.Debt).WithOne(d => d.Invoice).HasForeignKey<Debt>(d => d.InvoiceId);  
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