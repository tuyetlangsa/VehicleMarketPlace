using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteVehicleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Vehicles_VehicleId",
                schema: "public",
                table: "Listings");

            migrationBuilder.DropTable(
                name: "VehicleImages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Vehicles",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Listings_VehicleId",
                schema: "public",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                schema: "public",
                table: "Listings");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                schema: "public",
                table: "Listings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisible",
                schema: "public",
                table: "Listings");

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                schema: "public",
                table: "Listings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vehicles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyType = table.Column<byte>(type: "smallint", nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    Energy = table.Column<byte>(type: "smallint", nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TransmissionType = table.Column<byte>(type: "smallint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleImages",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleImages_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "public",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listings_VehicleId",
                schema: "public",
                table: "Listings",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_IsPrimary",
                schema: "public",
                table: "VehicleImages",
                column: "IsPrimary");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_VehicleId",
                schema: "public",
                table: "VehicleImages",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Brand",
                schema: "public",
                table: "Vehicles",
                column: "Brand");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Model",
                schema: "public",
                table: "Vehicles",
                column: "Model");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UserId",
                schema: "public",
                table: "Vehicles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Year",
                schema: "public",
                table: "Vehicles",
                column: "Year");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Vehicles_VehicleId",
                schema: "public",
                table: "Listings",
                column: "VehicleId",
                principalSchema: "public",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }
    }
}
