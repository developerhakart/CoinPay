using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SenderName = table.Column<string>(type: "text", nullable: true),
                    ReceiverName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "CompletedAt", "CreatedAt", "Currency", "Description", "ReceiverName", "SenderName", "Status", "TransactionId", "Type" },
                values: new object[,]
                {
                    { 1, 100.50m, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "USD", "Payment for services", "Jane Smith", "John Doe", "Completed", "TXN001", "Payment" },
                    { 2, 250.00m, new DateTime(2025, 10, 26, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 26, 0, 0, 0, 0, DateTimeKind.Utc), "USD", "Money transfer", "Bob Wilson", "Alice Johnson", "Completed", "TXN002", "Transfer" },
                    { 3, 75.25m, null, new DateTime(2025, 10, 27, 0, 0, 0, 0, DateTimeKind.Utc), "USD", "Pending payment", "David Lee", "Charlie Brown", "Pending", "TXN003", "Payment" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
