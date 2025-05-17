using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryAppSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDriverModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Users_DriverID",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Users_DriverUserId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Users_DriverId",
                table: "RideRequests");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_DriverUserId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FixedDeliveryPrice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PricePerKm",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PricingType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DriverUserId",
                table: "Ratings");

            // Rename DriverID -> DriverId (column already exists)
            migrationBuilder.RenameColumn(
                name: "DriverID",
                table: "Ratings",
                newName: "DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_DriverID",
                table: "Ratings",
                newName: "IX_Ratings_DriverId");

            // Make DriverId nullable
            migrationBuilder.AlterColumn<int>(
                name: "DriverId",
                table: "Ratings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Create Drivers table
            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    DriverId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingHours = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FixedDeliveryPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerKm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.DriverId);
                    table.ForeignKey(
                        name: "FK_Drivers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_UserId",
                table: "Drivers",
                column: "UserId");

            // Add FK only once using the correct DriverId
            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Drivers_DriverId",
                table: "Ratings",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Drivers_DriverId",
                table: "RideRequests",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Drivers_DriverId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Drivers_DriverId",
                table: "RideRequests");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                table: "Ratings",
                newName: "DriverID");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_DriverId",
                table: "Ratings",
                newName: "IX_Ratings_DriverID");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FixedDeliveryPrice",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerKm",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PricingType",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingHours",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DriverID",
                table: "Ratings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverUserId",
                table: "Ratings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_DriverUserId",
                table: "Ratings",
                column: "DriverUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Users_DriverID",
                table: "Ratings",
                column: "DriverID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Users_DriverUserId",
                table: "Ratings",
                column: "DriverUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Users_DriverId",
                table: "RideRequests",
                column: "DriverId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

    }
}
