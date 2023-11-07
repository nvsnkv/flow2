﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flow.Infrastructure.Storage.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class revsions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Revision",
                table: "Transactions",
                type: "text",
                nullable: true);
            migrationBuilder.Sql("update \"Transactions\" set \"Revision\" = md5(random()::text)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Revision",
                table: "Transactions");
        }
    }
}
