using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class FullTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Listings_Title_Description_VehicleBrand_VehicleModel_Locati~",
                schema: "public",
                table: "Listings",
                columns: new[] { "Title", "Description", "VehicleBrand", "VehicleModel", "Location", "SeatingCapacity", "Energy", "BodyType", "ManufacturingYear", "TransmissionType" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Listings_Title_Description_VehicleBrand_VehicleModel_Locati~",
                schema: "public",
                table: "Listings");
        }
    }
}
