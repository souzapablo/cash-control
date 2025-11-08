using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CashControl.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "currency",
                table: "accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "currency", table: "accounts");
        }
    }
}
