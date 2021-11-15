﻿// <auto-generated />
using System;
using Flow.Infrastructure.Storage.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Flow.Infrastructure.Storage.Migrations.Migrations
{
    [DbContext(typeof(FlowDbContext))]
    [Migration("20211114181433_rates")]
    partial class rates
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Flow.Domain.ExchangeRates.ExchangeRate", b =>
                {
                    b.Property<string>("From")
                        .HasColumnType("text");

                    b.Property<string>("To")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("Rate")
                        .HasColumnType("numeric");

                    b.HasKey("From", "To", "Date");

                    b.ToTable("ExchangeRates");
                });

            modelBuilder.Entity("Flow.Infrastructure.Storage.Model.DbAccount", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Bank")
                        .HasColumnType("text");

                    b.HasKey("Name", "Bank");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Flow.Infrastructure.Storage.Model.DbTransaction", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DbAccountBank")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DbAccountName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Key");

                    b.HasIndex("DbAccountName", "DbAccountBank");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Flow.Infrastructure.Storage.Model.DbTransferKey", b =>
                {
                    b.Property<long>("Source")
                        .HasColumnType("bigint");

                    b.Property<long>("Sink")
                        .HasColumnType("bigint");

                    b.HasKey("Source", "Sink");

                    b.HasIndex("Sink")
                        .IsUnique();

                    b.HasIndex("Source")
                        .IsUnique();

                    b.ToTable("EnforcedTransfers");
                });

            modelBuilder.Entity("Flow.Infrastructure.Storage.Model.DbTransaction", b =>
                {
                    b.HasOne("Flow.Infrastructure.Storage.Model.DbAccount", "DbAccount")
                        .WithMany("Transactions")
                        .HasForeignKey("DbAccountName", "DbAccountBank")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Flow.Domain.Transactions.Overrides", "Overrides", b1 =>
                        {
                            b1.Property<long>("DbTransactionKey")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("bigint")
                                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                            b1.Property<string>("Category")
                                .HasColumnType("text");

                            b1.Property<string>("Comment")
                                .HasColumnType("text");

                            b1.Property<string>("Title")
                                .HasColumnType("text");

                            b1.HasKey("DbTransactionKey");

                            b1.ToTable("Transactions");

                            b1.WithOwner()
                                .HasForeignKey("DbTransactionKey");
                        });

                    b.Navigation("DbAccount");

                    b.Navigation("Overrides");
                });

            modelBuilder.Entity("Flow.Infrastructure.Storage.Model.DbTransferKey", b =>
                {
                    b.HasOne("Flow.Infrastructure.Storage.Model.DbTransaction", "SinkTransaction")
                        .WithOne("SinkOf")
                        .HasForeignKey("Flow.Infrastructure.Storage.Model.DbTransferKey", "Sink")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Flow.Infrastructure.Storage.Model.DbTransaction", "SourceTransaction")
                        .WithOne("SourceOf")
                        .HasForeignKey("Flow.Infrastructure.Storage.Model.DbTransferKey", "Source")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SinkTransaction");

                    b.Navigation("SourceTransaction");
                });

            modelBuilder.Entity("Flow.Infrastructure.Storage.Model.DbAccount", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("Flow.Infrastructure.Storage.Model.DbTransaction", b =>
                {
                    b.Navigation("SinkOf");

                    b.Navigation("SourceOf");
                });
#pragma warning restore 612, 618
        }
    }
}
