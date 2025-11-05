using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSwapTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SwapTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletAddress = table.Column<string>(type: "text", nullable: false),
                    FromToken = table.Column<string>(type: "text", nullable: false),
                    ToToken = table.Column<string>(type: "text", nullable: false),
                    FromTokenSymbol = table.Column<string>(type: "text", nullable: false),
                    ToTokenSymbol = table.Column<string>(type: "text", nullable: false),
                    FromAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ToAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "numeric", nullable: false),
                    PlatformFee = table.Column<decimal>(type: "numeric", nullable: false),
                    PlatformFeePercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    GasUsed = table.Column<string>(type: "text", nullable: true),
                    GasCost = table.Column<decimal>(type: "numeric", nullable: true),
                    SlippageTolerance = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceImpact = table.Column<decimal>(type: "numeric", nullable: true),
                    MinimumReceived = table.Column<decimal>(type: "numeric", nullable: false),
                    DexProvider = table.Column<string>(type: "text", nullable: false),
                    TransactionHash = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwapTransactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SwapTransactions_CreatedAt",
                table: "SwapTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SwapTransactions_Status",
                table: "SwapTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SwapTransactions_TransactionHash",
                table: "SwapTransactions",
                column: "TransactionHash");

            migrationBuilder.CreateIndex(
                name: "IX_SwapTransactions_UserId",
                table: "SwapTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SwapTransactions_WalletAddress",
                table: "SwapTransactions",
                column: "WalletAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwapTransactions");
        }
    }
}
