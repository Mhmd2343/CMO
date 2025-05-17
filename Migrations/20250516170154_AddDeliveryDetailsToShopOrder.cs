using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryAppSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryDetailsToShopOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstimatedDeliveryTime",
                table: "ShopOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "ShopOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestChange",
                table: "ShopOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountLBP",
                table: "ShopOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountUSD",
                table: "ShopOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryTime",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "RequestChange",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "TotalAmountLBP",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "TotalAmountUSD",
                table: "ShopOrders");
        }
    }
}
