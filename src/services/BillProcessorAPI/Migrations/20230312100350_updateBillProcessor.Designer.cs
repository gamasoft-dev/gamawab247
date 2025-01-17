﻿// <auto-generated />
using System;
using BillProcessorAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    [DbContext(typeof(BillProcessorDbContext))]
    [Migration("20230312100350_updateBillProcessor")]
    partial class updateBillProcessor
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BillProcessorAPI.Entities.BillCharge", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BusinessId")
                        .HasColumnType("uuid");

                    b.Property<string>("ChannelModel")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<decimal>("MaxChargeAmount")
                        .HasColumnType("numeric");

                    b.Property<decimal>("MinChargeAmount")
                        .HasColumnType("numeric");

                    b.Property<double>("PercentageCharge")
                        .HasColumnType("double precision");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Charges");
                });

            modelBuilder.Entity("BillProcessorAPI.Entities.BillPayerInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AccountInfoRequestData")
                        .HasColumnType("text");

                    b.Property<string>("AccountInfoResponseData")
                        .HasColumnType("text");

                    b.Property<string>("AcctCloseDate")
                        .HasColumnType("text");

                    b.Property<string>("AgencyCode")
                        .HasColumnType("text");

                    b.Property<string>("AgencyName")
                        .HasColumnType("text");

                    b.Property<decimal>("AmountDue")
                        .HasColumnType("numeric");

                    b.Property<string>("CbnCode")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<string>("CreditAccount")
                        .HasColumnType("text");

                    b.Property<string>("Currency")
                        .HasColumnType("text");

                    b.Property<decimal>("MaxAmount")
                        .HasColumnType("numeric");

                    b.Property<decimal>("MinAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("OraAgencyRev")
                        .HasColumnType("text");

                    b.Property<string>("PayerName")
                        .HasColumnType("text");

                    b.Property<string>("PaymentFlag")
                        .HasColumnType("text");

                    b.Property<string>("Pid")
                        .HasColumnType("text");

                    b.Property<string>("ReadOnly")
                        .HasColumnType("text");

                    b.Property<string>("RevName")
                        .HasColumnType("text");

                    b.Property<string>("RevenueCode")
                        .HasColumnType("text");

                    b.Property<string>("State")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("StatusMessage")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("BillPayers");
                });

            modelBuilder.Entity("BillProcessorAPI.Entities.BillTransaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("AmountPaid")
                        .HasColumnType("numeric");

                    b.Property<decimal>("BillAmount")
                        .HasColumnType("numeric");

                    b.Property<int>("Channel")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<decimal>("GatewayTransactionCharge")
                        .HasColumnType("numeric");

                    b.Property<string>("GatewayTransactionReference")
                        .HasColumnType("text");

                    b.Property<int>("GatewayType")
                        .HasColumnType("integer");

                    b.Property<string>("Narration")
                        .HasColumnType("text");

                    b.Property<string>("PaymentInfoRequestData")
                        .HasColumnType("text");

                    b.Property<string>("PaymentInfoResponseData")
                        .HasColumnType("text");

                    b.Property<decimal>("PrinciPalAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("Receipt")
                        .HasColumnType("text");

                    b.Property<string>("ReceiptUrl")
                        .HasColumnType("text");

                    b.Property<string>("ResourcePIN")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("StatusMessage")
                        .HasColumnType("text");

                    b.Property<string>("SystemReference")
                        .HasColumnType("text");

                    b.Property<decimal>("TransactionCharge")
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("BillTransactions");
                });

            modelBuilder.Entity("BillProcessorAPI.Entities.BillTransaction", b =>
                {
                    b.HasOne("BillProcessorAPI.Entities.BillPayerInfo", "User")
                        .WithMany("BillTransactions")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BillProcessorAPI.Entities.BillPayerInfo", b =>
                {
                    b.Navigation("BillTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}
