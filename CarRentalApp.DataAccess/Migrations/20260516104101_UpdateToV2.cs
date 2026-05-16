using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "Rentals",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SellerAmount",
                table: "Rentals",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "SellerAmount",
                table: "Rentals");
        }
    }
}
