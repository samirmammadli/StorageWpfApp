﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StorageWpfApp.Entities;

namespace StorageWpfApp.Migrations
{
    [DbContext(typeof(ProjectContext))]
    partial class ProjectContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("StorageWpfApp.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Debit")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0.0);

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("Surname");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.Consignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CurrentPieceQuantity");

                    b.Property<DateTime?>("Date");

                    b.Property<bool>("IsPieceAllowed");

                    b.Property<double?>("PiecePrice");

                    b.Property<long?>("ProductId");

                    b.Property<double>("PurchasePrice");

                    b.Property<int>("Quantity");

                    b.Property<double>("SellingPrice");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Consignments");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.Debt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<int>("ClientId");

                    b.Property<int>("InvoiceId");

                    b.Property<double>("Payed");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("InvoiceId")
                        .IsUnique();

                    b.ToTable("Debts");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.DebtPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int>("DebtId");

                    b.HasKey("Id");

                    b.HasIndex("DebtId");

                    b.ToTable("DebtPayments");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.Invoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId");

                    b.Property<int?>("ConsignmentId");

                    b.Property<DateTime>("Date");

                    b.Property<double>("TotalAmount");

                    b.Property<double>("TotalDiscount");

                    b.Property<double>("TotalPayed");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ConsignmentId");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.PieceOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ConsignmentId");

                    b.Property<int>("Count");

                    b.Property<double>("Discount");

                    b.Property<int>("InvoiceId");

                    b.HasKey("Id");

                    b.HasIndex("ConsignmentId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("PieceOrders");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.Product", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<int?>("GroupId");

                    b.Property<bool>("IsPieceProduct")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<string>("Name");

                    b.Property<int?>("PieceQuantity");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("GroupId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.ProductGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.SingleOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ConsignmentId");

                    b.Property<int>("Count");

                    b.Property<double>("Discount");

                    b.Property<int>("InvoiceId");

                    b.HasKey("Id");

                    b.HasIndex("ConsignmentId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("SingleOrders");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.Consignment", b =>
                {
                    b.HasOne("StorageWpfApp.Entities.Product", "Product")
                        .WithMany("Consignments")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.Debt", b =>
                {
                    b.HasOne("StorageWpfApp.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StorageWpfApp.Entities.Invoice", "Invoice")
                        .WithOne("Debt")
                        .HasForeignKey("StorageWpfApp.Entities.Debt", "InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StorageWpfApp.Entities.DebtPayment", b =>
                {
                    b.HasOne("StorageWpfApp.Entities.Debt", "Debt")
                        .WithMany("DebtPayments")
                        .HasForeignKey("DebtId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StorageWpfApp.Entities.Invoice", b =>
                {
                    b.HasOne("StorageWpfApp.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");

                    b.HasOne("StorageWpfApp.Entities.Consignment")
                        .WithMany("Invoices")
                        .HasForeignKey("ConsignmentId");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.PieceOrder", b =>
                {
                    b.HasOne("StorageWpfApp.Entities.Consignment", "Consignment")
                        .WithMany()
                        .HasForeignKey("ConsignmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StorageWpfApp.Entities.Invoice", "Invoice")
                        .WithMany("PieceOrders")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StorageWpfApp.Entities.Product", b =>
                {
                    b.HasOne("StorageWpfApp.Entities.ProductGroup", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("StorageWpfApp.Entities.SingleOrder", b =>
                {
                    b.HasOne("StorageWpfApp.Entities.Consignment", "Consignment")
                        .WithMany()
                        .HasForeignKey("ConsignmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StorageWpfApp.Entities.Invoice", "Invoice")
                        .WithMany("SingleOrders")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
