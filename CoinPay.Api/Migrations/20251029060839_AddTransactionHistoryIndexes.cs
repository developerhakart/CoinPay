using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionHistoryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add index on CreatedAt for date range filtering
            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_CreatedAt",
                table: "BlockchainTransactions",
                column: "CreatedAt");

            // Add index on AmountDecimal for amount filtering and sorting
            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_AmountDecimal",
                table: "BlockchainTransactions",
                column: "AmountDecimal");

            // Add composite index for common query pattern (WalletId + CreatedAt)
            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_WalletId_CreatedAt",
                table: "BlockchainTransactions",
                columns: new[] { "WalletId", "CreatedAt" });

            // Add composite index for status queries (WalletId + Status + CreatedAt)
            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_WalletId_Status_CreatedAt",
                table: "BlockchainTransactions",
                columns: new[] { "WalletId", "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlockchainTransactions_CreatedAt",
                table: "BlockchainTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BlockchainTransactions_AmountDecimal",
                table: "BlockchainTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BlockchainTransactions_WalletId_CreatedAt",
                table: "BlockchainTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BlockchainTransactions_WalletId_Status_CreatedAt",
                table: "BlockchainTransactions");
        }
    }
}
