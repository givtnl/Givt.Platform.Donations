﻿// <auto-generated />
using System;
using Givt.Donations.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Givt.Donations.Persistence.Migrations
{
    [DbContext(typeof(DonationsContext))]
    [Migration("20220622095013_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Givt.Donations.Domain.Entities.Donation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<Guid>("CampaignId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Currency")
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<DateTime>("DonationDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("DonorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Fingerprint")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Last4")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<Guid>("MediumId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("PayinId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RecipientId")
                        .HasColumnType("uuid");

                    b.Property<string>("TransactionReference")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("PayinId");

                    b.HasIndex("TransactionReference");

                    b.ToTable("Donations");
                });

            modelBuilder.Entity("Givt.Donations.Domain.Entities.DonationHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<Guid>("CampaignId")
                        .HasColumnType("uuid");

                    b.Property<string>("Currency")
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<DateTime>("DonationDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("DonorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Fingerprint")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Last4")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<Guid>("MediumId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PayinId")
                        .HasColumnType("uuid");

                    b.Property<int>("Reason")
                        .HasColumnType("integer");

                    b.Property<Guid>("RecipientId")
                        .HasColumnType("uuid");

                    b.Property<string>("TransactionReference")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id", "Modified");

                    b.HasIndex("TransactionReference");

                    b.ToTable("DonationHistory");
                });

            modelBuilder.Entity("Givt.Donations.Domain.Entities.PayIn", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<string>("Currency")
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExecutedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("PayInMethodId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("PaymentProviderExecutionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("TotalPaid")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("PayIns");
                });

            modelBuilder.Entity("Givt.Donations.Domain.Entities.PayOut", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<Guid>("CampaignId")
                        .HasColumnType("uuid");

                    b.Property<string>("Currency")
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExecutedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("GivtServiceFee")
                        .HasColumnType("numeric");

                    b.Property<decimal>("GivtServiceFeeTaxes")
                        .HasColumnType("numeric");

                    b.Property<decimal>("MandateCost")
                        .HasColumnType("numeric");

                    b.Property<int>("MandateCostCount")
                        .HasColumnType("integer");

                    b.Property<decimal>("MandateTaxes")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PaymentCost")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PaymentCostTaxes")
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("PaymentProviderExecutionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PaymentProviderId")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<decimal>("RTransactionAmount")
                        .HasColumnType("numeric");

                    b.Property<decimal>("RTransactionT1Cost")
                        .HasColumnType("numeric");

                    b.Property<decimal>("RTransactionT1Count")
                        .HasColumnType("numeric");

                    b.Property<decimal>("RTransactionT2Cost")
                        .HasColumnType("numeric");

                    b.Property<decimal>("RTransactionT2Count")
                        .HasColumnType("numeric");

                    b.Property<decimal>("RTransactionTaxes")
                        .HasColumnType("numeric");

                    b.Property<Guid>("RecipientId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("TotalPaid")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TransactionCost")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TransactionCount")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TransactionTaxes")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("PayOuts");
                });

            modelBuilder.Entity("Givt.Donations.Domain.Entities.Donation", b =>
                {
                    b.HasOne("Givt.Donations.Domain.Entities.PayIn", "Payin")
                        .WithMany("Donations")
                        .HasForeignKey("PayinId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Payin");
                });

            modelBuilder.Entity("Givt.Donations.Domain.Entities.PayIn", b =>
                {
                    b.Navigation("Donations");
                });
#pragma warning restore 612, 618
        }
    }
}