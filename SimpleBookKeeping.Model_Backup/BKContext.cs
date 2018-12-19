using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleBookKeeping.Model
{
        public class BookKeeperContext : DbContext
        {
     
            public DbSet<ExpenseEntry> ExpenseDBSet { get; set; }
            public DbSet<IncomeEntry> IncomeDBSet { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<ExpenseEntry>()
                    .HasKey(e => e.ExpenseId);
                modelBuilder.Entity<ExpenseEntry>()
                    .Property(e => e.AmountPaid)
                    .HasMaxLength(50);
                modelBuilder.Entity<ExpenseEntry>()
                    .Property(e => e.CheckNumber)
                    .HasMaxLength(20);
                modelBuilder.Entity<ExpenseEntry>()
                    .Property(e => e.EntryNotes)
                    .HasMaxLength(5000);
                modelBuilder.Entity<ExpenseEntry>()
                    .Property(e => e.Month)
                    .HasMaxLength(2);
                modelBuilder.Entity<ExpenseEntry>()
                    .Property(e => e.ExpenseCategory)
                    .HasMaxLength(50);
                modelBuilder.Entity<ExpenseEntry>()
                    .Property(e => e.ExpenseTotal)
                    .HasMaxLength(500);

                modelBuilder.Entity<IncomeEntry>()
                    .HasKey(i => i.IncomeId);
                modelBuilder.Entity<IncomeEntry>()
                    .Property(i => i.CheckNumber)
                    .HasMaxLength(20);
                modelBuilder.Entity<IncomeEntry>()
                    .Property(i => i.EntryNotes)
                    .HasMaxLength(5000);
                modelBuilder.Entity<IncomeEntry>()
                    .Property(i => i.IncomeAmount)
                    .HasMaxLength(50);
                modelBuilder.Entity<IncomeEntry>()
                    .Property(i => i.IncomeCategory)
                    .HasMaxLength(50);
                modelBuilder.Entity<IncomeEntry>()
                    .Property(i => i.Month)
                    .HasMaxLength(2);
                modelBuilder.Entity<IncomeEntry>()
                       .Property(i => i.IncomeTotal)
                       .HasMaxLength(500);
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=SimpleBookKeeping;Trusted_Connection=True;");
            }


        }

}
