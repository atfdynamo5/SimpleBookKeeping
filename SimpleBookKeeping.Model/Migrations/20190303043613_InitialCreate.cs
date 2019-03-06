using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleBookKeeping.Model.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseDBSet",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpenseCategory = table.Column<string>(maxLength: 50, nullable: true),
                    CheckNumber = table.Column<int>(maxLength: 20, nullable: false),
                    EntryNotes = table.Column<string>(maxLength: 5000, nullable: true),
                    AmountPaid = table.Column<decimal>(maxLength: 50, nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    DatePaid = table.Column<DateTime>(nullable: false),
                    Month = table.Column<int>(maxLength: 2, nullable: false),
                    ExpenseTotal = table.Column<decimal>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseDBSet", x => x.ExpenseId);
                });

            migrationBuilder.CreateTable(
                name: "IncomeDBSet",
                columns: table => new
                {
                    IncomeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Month = table.Column<int>(maxLength: 2, nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    DatePaid = table.Column<DateTime>(nullable: false),
                    IncomeAmount = table.Column<decimal>(maxLength: 50, nullable: false),
                    IncomeCategory = table.Column<string>(maxLength: 50, nullable: true),
                    Percentage = table.Column<decimal>(maxLength: 10, nullable: false),
                    EntryNotes = table.Column<string>(maxLength: 5000, nullable: true),
                    IncomeTotal = table.Column<decimal>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeDBSet", x => x.IncomeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseDBSet");

            migrationBuilder.DropTable(
                name: "IncomeDBSet");
        }
    }
}
