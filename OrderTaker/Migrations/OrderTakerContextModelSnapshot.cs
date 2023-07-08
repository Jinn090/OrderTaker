﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderTaker.Models;

#nullable disable

namespace OrderTaker.Migrations
{
    [DbContext(typeof(OrderTakerContext))]
    partial class OrderTakerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OrderTaker.Models.Customer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("City")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasMaxLength(102)
                        .IsUnicode(false)
                        .HasColumnType("varchar(102)")
                        .HasComputedColumnSql("(([LastName]+', ')+[FirstName])", true);

                    b.Property<bool?>("IsActive")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("((1))");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("MobileNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex(new[] { "FullName" }, "Is_Unique_FullName")
                        .IsUnique();

                    b.HasIndex(new[] { "MobileNumber" }, "Is_Unique_Mobile")
                        .IsUnique();

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("OrderTaker.Models.PurchaseItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("PurchaseOrderID")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("SKUID")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("PurchaseOrderID");

                    b.HasIndex("SKUID");

                    b.ToTable("PurchaseItems");
                });

            modelBuilder.Entity("OrderTaker.Models.PurchaseOrder", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<decimal>("AmountDue")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int>("CustomerID")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime>("DateOfDelivery")
                        .HasColumnType("date");

                    b.Property<bool?>("IsActive")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("((1))");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)")
                        .HasDefaultValueSql("('New')");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.ToTable("PurchaseOrder");
                });

            modelBuilder.Entity("OrderTaker.Models.SKU", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID")
                        .HasName("PK_SKU");

                    b.HasIndex(new[] { "Code" }, "Is_Unique_CODE")
                        .IsUnique();

                    b.HasIndex(new[] { "Name" }, "Is_Unique_NAME")
                        .IsUnique();

                    b.ToTable("SKUs");
                });

            modelBuilder.Entity("OrderTaker.Models.PurchaseItem", b =>
                {
                    b.HasOne("OrderTaker.Models.PurchaseOrder", "PurchaseOrder")
                        .WithMany("PurchaseItems")
                        .HasForeignKey("PurchaseOrderID")
                        .IsRequired()
                        .HasConstraintName("FK_PurchaseItems_PurchaseOrder");

                    b.HasOne("OrderTaker.Models.SKU", "SKU")
                        .WithMany("PurchaseItems")
                        .HasForeignKey("SKUID")
                        .IsRequired()
                        .HasConstraintName("FK_PurchaseItems_SKUs");

                    b.Navigation("PurchaseOrder");

                    b.Navigation("SKU");
                });

            modelBuilder.Entity("OrderTaker.Models.PurchaseOrder", b =>
                {
                    b.HasOne("OrderTaker.Models.Customer", "Customer")
                        .WithMany("PurchaseOrders")
                        .HasForeignKey("CustomerID")
                        .IsRequired()
                        .HasConstraintName("FK_PurchaseOrder_Customers");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("OrderTaker.Models.Customer", b =>
                {
                    b.Navigation("PurchaseOrders");
                });

            modelBuilder.Entity("OrderTaker.Models.PurchaseOrder", b =>
                {
                    b.Navigation("PurchaseItems");
                });

            modelBuilder.Entity("OrderTaker.Models.SKU", b =>
                {
                    b.Navigation("PurchaseItems");
                });
#pragma warning restore 612, 618
        }
    }
}
