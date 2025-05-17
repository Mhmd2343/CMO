using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryAppSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverAndClientToShopOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_Users_UserId",
                table: "ShopOrders");

            migrationBuilder.DropIndex(
                name: "IX_ShopOrders_UserId",
                table: "ShopOrders");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ShopOrders",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "AssignedDriverId",
                table: "ShopOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "ShopOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress",
                table: "ShopOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "ShopOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_AssignedDriverId",
                table: "ShopOrders",
                column: "AssignedDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_ClientId",
                table: "ShopOrders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_ShopId",
                table: "ShopOrders",
                column: "ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_Drivers_AssignedDriverId",
                table: "ShopOrders",
                column: "AssignedDriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_Shops_ShopId",
                table: "ShopOrders",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "ShopId",
                onDelete: ReferentialAction.Restrict); // <- changed from Cascade

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_Users_ClientId",
                table: "ShopOrders",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_Drivers_AssignedDriverId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_Shops_ShopId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_Users_ClientId",
                table: "ShopOrders");

            migrationBuilder.DropIndex(
                name: "IX_ShopOrders_AssignedDriverId",
                table: "ShopOrders");

            migrationBuilder.DropIndex(
                name: "IX_ShopOrders_ClientId",
                table: "ShopOrders");

            migrationBuilder.DropIndex(
                name: "IX_ShopOrders_ShopId",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "AssignedDriverId",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "ShopOrders");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ShopOrders",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_UserId",
                table: "ShopOrders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_Users_UserId",
                table: "ShopOrders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
