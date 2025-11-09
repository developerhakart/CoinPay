using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDemoTokenBalanceSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DemoTokenBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TokenSymbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    TotalIssued = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    TotalInvested = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoTokenBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemoTokenBalances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DemoTokenBalances_IsActive",
                table: "DemoTokenBalances",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DemoTokenBalances_UserId",
                table: "DemoTokenBalances",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DemoTokenBalances_UserId_TokenSymbol",
                table: "DemoTokenBalances",
                columns: new[] { "UserId", "TokenSymbol" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemoTokenBalances");
        }
    }
}
