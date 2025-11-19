using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixConversionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                schema: "public",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "TransmissionType",
                schema: "public",
                table: "Listings",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "public",
                table: "Listings",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "Energy",
                schema: "public",
                table: "Listings",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "BodyType",
                schema: "public",
                table: "Listings",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Role",
                schema: "public",
                table: "Users",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<byte>(
                name: "TransmissionType",
                schema: "public",
                table: "Listings",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                schema: "public",
                table: "Listings",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<byte>(
                name: "Energy",
                schema: "public",
                table: "Listings",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<byte>(
                name: "BodyType",
                schema: "public",
                table: "Listings",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
