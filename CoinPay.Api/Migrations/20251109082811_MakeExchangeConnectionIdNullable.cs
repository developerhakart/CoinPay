using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeExchangeConnectionIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPositions_ExchangeConnections_ExchangeConnectionI~",
                table: "InvestmentPositions");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeConnectionId1",
                table: "InvestmentPositions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeConnectionId",
                table: "InvestmentPositions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPositions_ExchangeConnections_ExchangeConnectionI~",
                table: "InvestmentPositions",
                column: "ExchangeConnectionId1",
                principalTable: "ExchangeConnections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPositions_ExchangeConnections_ExchangeConnectionI~",
                table: "InvestmentPositions");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeConnectionId1",
                table: "InvestmentPositions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeConnectionId",
                table: "InvestmentPositions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPositions_ExchangeConnections_ExchangeConnectionI~",
                table: "InvestmentPositions",
                column: "ExchangeConnectionId1",
                principalTable: "ExchangeConnections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
