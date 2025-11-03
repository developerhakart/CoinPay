using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoinPay.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCircleWalletId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL to add column only if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_name = 'Users' AND column_name = 'CircleWalletId'
                    ) THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""CircleWalletId"" TEXT NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CircleWalletId",
                table: "Users");
        }
    }
}
