using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInvestmentInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExchangeName = table.Column<string>(type: "text", nullable: false),
                    ApiKeyEncrypted = table.Column<string>(type: "text", nullable: false),
                    ApiSecretEncrypted = table.Column<string>(type: "text", nullable: false),
                    EncryptionKeyId = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastValidatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId1 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeConnections_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExchangeConnectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExchangeName = table.Column<string>(type: "text", nullable: false),
                    ExternalPositionId = table.Column<string>(type: "text", nullable: true),
                    PlanId = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<string>(type: "text", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentValue = table.Column<decimal>(type: "numeric", nullable: false),
                    AccruedRewards = table.Column<decimal>(type: "numeric", nullable: false),
                    Apy = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId1 = table.Column<int>(type: "integer", nullable: false),
                    ExchangeConnectionId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentPositions_ExchangeConnections_ExchangeConnectionId",
                        column: x => x.ExchangeConnectionId,
                        principalTable: "ExchangeConnections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestmentPositions_ExchangeConnections_ExchangeConnectionI~",
                        column: x => x.ExchangeConnectionId1,
                        principalTable: "ExchangeConnections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvestmentPositions_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvestmentPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Asset = table.Column<string>(type: "text", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InvestmentPositionId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId1 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentTransactions_InvestmentPositions_InvestmentPositi~",
                        column: x => x.InvestmentPositionId,
                        principalTable: "InvestmentPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvestmentTransactions_InvestmentPositions_InvestmentPosit~1",
                        column: x => x.InvestmentPositionId1,
                        principalTable: "InvestmentPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvestmentTransactions_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeConnections_IsActive",
                table: "ExchangeConnections",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeConnections_UserId",
                table: "ExchangeConnections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeConnections_UserId_ExchangeName",
                table: "ExchangeConnections",
                columns: new[] { "UserId", "ExchangeName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeConnections_UserId1",
                table: "ExchangeConnections",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPositions_CreatedAt",
                table: "InvestmentPositions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPositions_ExchangeConnectionId",
                table: "InvestmentPositions",
                column: "ExchangeConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPositions_ExchangeConnectionId1",
                table: "InvestmentPositions",
                column: "ExchangeConnectionId1");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPositions_Status",
                table: "InvestmentPositions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPositions_UserId",
                table: "InvestmentPositions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPositions_UserId1",
                table: "InvestmentPositions",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_CreatedAt",
                table: "InvestmentTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_InvestmentPositionId",
                table: "InvestmentTransactions",
                column: "InvestmentPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_InvestmentPositionId1",
                table: "InvestmentTransactions",
                column: "InvestmentPositionId1");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_UserId",
                table: "InvestmentTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_UserId1",
                table: "InvestmentTransactions",
                column: "UserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvestmentTransactions");

            migrationBuilder.DropTable(
                name: "InvestmentPositions");

            migrationBuilder.DropTable(
                name: "ExchangeConnections");
        }
    }
}
