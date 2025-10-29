using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWebhookSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebhookRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Secret = table.Column<string>(type: "text", nullable: false),
                    Events = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookRegistrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebhookDeliveryLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WebhookId = table.Column<int>(type: "integer", nullable: false),
                    EventName = table.Column<string>(type: "text", nullable: false),
                    TransactionId = table.Column<int>(type: "integer", nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookDeliveryLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookDeliveryLogs_BlockchainTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "BlockchainTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WebhookDeliveryLogs_WebhookRegistrations_WebhookId",
                        column: x => x.WebhookId,
                        principalTable: "WebhookRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveryLogs_Timestamp",
                table: "WebhookDeliveryLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveryLogs_TransactionId",
                table: "WebhookDeliveryLogs",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveryLogs_WebhookId",
                table: "WebhookDeliveryLogs",
                column: "WebhookId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookRegistrations_IsActive",
                table: "WebhookRegistrations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookRegistrations_UserId",
                table: "WebhookRegistrations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebhookDeliveryLogs");

            migrationBuilder.DropTable(
                name: "WebhookRegistrations");
        }
    }
}
