using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShadowForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove InvestmentPositionId1 from InvestmentTransactions if it exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.columns
                              WHERE table_name='InvestmentTransactions'
                              AND column_name='InvestmentPositionId1') THEN
                        ALTER TABLE ""InvestmentTransactions""
                        DROP CONSTRAINT IF EXISTS ""FK_InvestmentTransactions_InvestmentPositions_InvestmentPosit~1"";

                        DROP INDEX IF EXISTS ""IX_InvestmentTransactions_InvestmentPositionId1"";

                        ALTER TABLE ""InvestmentTransactions"" DROP COLUMN ""InvestmentPositionId1"";
                    END IF;
                END $$;
            ");

            // Remove ExchangeConnectionId1 from InvestmentPositions
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.columns
                              WHERE table_name='InvestmentPositions'
                              AND column_name='ExchangeConnectionId1') THEN
                        ALTER TABLE ""InvestmentPositions""
                        DROP CONSTRAINT IF EXISTS ""FK_InvestmentPositions_ExchangeConnections_ExchangeConnectionI~"";

                        DROP INDEX IF EXISTS ""IX_InvestmentPositions_ExchangeConnectionId1"";

                        ALTER TABLE ""InvestmentPositions"" DROP COLUMN ""ExchangeConnectionId1"";
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate shadow columns if needed for rollback
            migrationBuilder.AddColumn<Guid>(
                name: "InvestmentPositionId1",
                table: "InvestmentTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ExchangeConnectionId1",
                table: "InvestmentPositions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_InvestmentPositionId1",
                table: "InvestmentTransactions",
                column: "InvestmentPositionId1");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPositions_ExchangeConnectionId1",
                table: "InvestmentPositions",
                column: "ExchangeConnectionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentTransactions_InvestmentPositions_InvestmentPosit~1",
                table: "InvestmentTransactions",
                column: "InvestmentPositionId1",
                principalTable: "InvestmentPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPositions_ExchangeConnections_ExchangeConnectionI~",
                table: "InvestmentPositions",
                column: "ExchangeConnectionId1",
                principalTable: "ExchangeConnections",
                principalColumn: "Id");
        }
    }
}
