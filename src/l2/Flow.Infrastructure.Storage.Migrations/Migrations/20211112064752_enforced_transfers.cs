using Microsoft.EntityFrameworkCore.Migrations;

namespace Flow.Infrastructure.Storage.Migrations.Migrations
{
    public partial class enforced_transfers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnforcedTransfers",
                columns: table => new
                {
                    Source = table.Column<long>(type: "bigint", nullable: false),
                    Sink = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnforcedTransfers", x => new { x.Source, x.Sink });
                    table.ForeignKey(
                        name: "FK_EnforcedTransfers_Transactions_Sink",
                        column: x => x.Sink,
                        principalTable: "Transactions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnforcedTransfers_Transactions_Source",
                        column: x => x.Source,
                        principalTable: "Transactions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnforcedTransfers_Sink",
                table: "EnforcedTransfers",
                column: "Sink",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnforcedTransfers_Source",
                table: "EnforcedTransfers",
                column: "Source",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnforcedTransfers");
        }
    }
}
