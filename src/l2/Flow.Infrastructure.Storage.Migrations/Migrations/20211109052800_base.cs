using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Flow.Infrastructure.Storage.Migrations.Migrations
{
    public partial class @base : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Bank = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => new { x.Name, x.Bank });
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Key = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DbAccountName = table.Column<string>(type: "text", nullable: false),
                    DbAccountBank = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Overrides_Comment = table.Column<string>(type: "text", nullable: true),
                    Overrides_Title = table.Column<string>(type: "text", nullable: true),
                    Overrides_Category = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_DbAccountName_DbAccountBank",
                        columns: x => new { x.DbAccountName, x.DbAccountBank },
                        principalTable: "Accounts",
                        principalColumns: new[] { "Name", "Bank" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DbAccountName_DbAccountBank",
                table: "Transactions",
                columns: new[] { "DbAccountName", "DbAccountBank" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
