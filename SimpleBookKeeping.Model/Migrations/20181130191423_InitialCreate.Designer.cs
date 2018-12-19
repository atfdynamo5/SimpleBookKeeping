﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleBookKeeping.Model;

namespace SimpleBookKeeping.Model.Migrations
{
    [DbContext(typeof(BookKeepingContext))]
    [Migration("20181130191423_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SimpleBookKeeping.Model.ExpenseEntry", b =>
                {
                    b.Property<int>("ExpenseId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AmountPaid")
                        .HasMaxLength(50);

                    b.Property<int>("CheckNumber")
                        .HasMaxLength(20);

                    b.Property<DateTime>("DatePaid");

                    b.Property<string>("EntryNotes")
                        .HasMaxLength(5000);

                    b.Property<string>("ExpenseCategory")
                        .HasMaxLength(50);

                    b.Property<decimal>("ExpenseTotal")
                        .HasMaxLength(500);

                    b.Property<int>("Month")
                        .HasMaxLength(2);

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("ExpenseId");

                    b.ToTable("ExpenseDBSet");
                });

            modelBuilder.Entity("SimpleBookKeeping.Model.IncomeEntry", b =>
                {
                    b.Property<int>("IncomeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CheckNumber")
                        .HasMaxLength(20);

                    b.Property<DateTime>("DatePaid");

                    b.Property<string>("EntryNotes")
                        .HasMaxLength(5000);

                    b.Property<decimal>("IncomeAmount")
                        .HasMaxLength(50);

                    b.Property<string>("IncomeCategory")
                        .HasMaxLength(50);

                    b.Property<decimal>("IncomeTotal")
                        .HasMaxLength(500);

                    b.Property<int>("Month")
                        .HasMaxLength(2);

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("IncomeId");

                    b.ToTable("IncomeDBSet");
                });
#pragma warning restore 612, 618
        }
    }
}