using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flow.Infrastructure.Storage.Migrations.Migrations
{
    public partial class transfer_keys_upgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnforcedTransfers_Transactions_Sink",
                table: "EnforcedTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_EnforcedTransfers_Transactions_Source",
                table: "EnforcedTransfers");

            migrationBuilder.RenameColumn(
                name: "Sink",
                table: "EnforcedTransfers",
                newName: "SinkKey");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "EnforcedTransfers",
                newName: "SourceKey");

            migrationBuilder.RenameIndex(
                name: "IX_EnforcedTransfers_Source",
                table: "EnforcedTransfers",
                newName: "IX_EnforcedTransfers_SourceKey");

            migrationBuilder.RenameIndex(
                name: "IX_EnforcedTransfers_Sink",
                table: "EnforcedTransfers",
                newName: "IX_EnforcedTransfers_SinkKey");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "ExchangeRates",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddForeignKey(
                name: "FK_EnforcedTransfers_Transactions_SinkKey",
                table: "EnforcedTransfers",
                column: "SinkKey",
                principalTable: "Transactions",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnforcedTransfers_Transactions_SourceKey",
                table: "EnforcedTransfers",
                column: "SourceKey",
                principalTable: "Transactions",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnforcedTransfers_Transactions_SinkKey",
                table: "EnforcedTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_EnforcedTransfers_Transactions_SourceKey",
                table: "EnforcedTransfers");

            migrationBuilder.RenameColumn(
                name: "SinkKey",
                table: "EnforcedTransfers",
                newName: "Sink");

            migrationBuilder.RenameColumn(
                name: "SourceKey",
                table: "EnforcedTransfers",
                newName: "Source");

            migrationBuilder.RenameIndex(
                name: "IX_EnforcedTransfers_SourceKey",
                table: "EnforcedTransfers",
                newName: "IX_EnforcedTransfers_Source");

            migrationBuilder.RenameIndex(
                name: "IX_EnforcedTransfers_SinkKey",
                table: "EnforcedTransfers",
                newName: "IX_EnforcedTransfers_Sink");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "Transactions",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "ExchangeRates",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddForeignKey(
                name: "FK_EnforcedTransfers_Transactions_Sink",
                table: "EnforcedTransfers",
                column: "Sink",
                principalTable: "Transactions",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnforcedTransfers_Transactions_Source",
                table: "EnforcedTransfers",
                column: "Source",
                principalTable: "Transactions",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
