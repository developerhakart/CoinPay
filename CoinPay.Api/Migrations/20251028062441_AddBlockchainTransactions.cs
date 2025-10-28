using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockchainTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    CircleWalletId = table.Column<string>(type: "text", nullable: false),
                    Blockchain = table.Column<string>(type: "text", nullable: false),
                    WalletType = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    BalanceCurrency = table.Column<string>(type: "text", nullable: false),
                    BalanceUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockchainTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WalletId = table.Column<int>(type: "integer", nullable: false),
                    UserOpHash = table.Column<string>(type: "text", nullable: false),
                    TransactionHash = table.Column<string>(type: "text", nullable: true),
                    FromAddress = table.Column<string>(type: "text", nullable: false),
                    ToAddress = table.Column<string>(type: "text", nullable: false),
                    TokenAddress = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<string>(type: "text", nullable: false),
                    AmountDecimal = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ChainId = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    GasUsed = table.Column<decimal>(type: "numeric", nullable: false),
                    IsGasless = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    BlockNumber = table.Column<long>(type: "bigint", nullable: true),
                    Confirmations = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockchainTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockchainTransactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_Status",
                table: "BlockchainTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_TransactionHash",
                table: "BlockchainTransactions",
                column: "TransactionHash");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_UserOpHash",
                table: "BlockchainTransactions",
                column: "UserOpHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_WalletId",
                table: "BlockchainTransactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Address",
                table: "Wallets",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockchainTransactions");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
