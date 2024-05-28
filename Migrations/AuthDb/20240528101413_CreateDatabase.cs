using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bloggie.Web.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1d4c38a7-4e58-4b2a-8a8e-9d9728e0c1e3",
                column: "NormalizedName",
                value: "ADMIN");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b8e62e84-2e7b-47b2-9fdf-d7b0b4b2a8b2",
                column: "NormalizedName",
                value: "USER");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c5f9cfe8-3c3a-4c9a-a5f5-9f8e349a2d1b",
                column: "NormalizedName",
                value: "SUPERADMIN");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e02f72d1-620d-4b88-bb73-dcd490b6e1d4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f734cfe-4a77-4dd8-b7d6-d532c58af5e5", "AQAAAAIAAYagAAAAEOGhvMmBQvb7Dcr1OMf8Nm+yvhBS76H+qp0sgHLA98+swHD5ImH1V6WGsFtbQa5FTg==", "1c97d802-c951-4319-bd02-3373389614d6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1d4c38a7-4e58-4b2a-8a8e-9d9728e0c1e3",
                column: "NormalizedName",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b8e62e84-2e7b-47b2-9fdf-d7b0b4b2a8b2",
                column: "NormalizedName",
                value: "User");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c5f9cfe8-3c3a-4c9a-a5f5-9f8e349a2d1b",
                column: "NormalizedName",
                value: "SuperAdmin");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e02f72d1-620d-4b88-bb73-dcd490b6e1d4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "19f765ac-e160-4272-9e2e-991df8d09a9b", "AQAAAAIAAYagAAAAEJeZ1rkhAV+1OlRVnU180IGUsqofq/W2RV9r+ZZY4Jyywm+g4/ovW5PiPp/eBFLevw==", "7c849b0d-53ef-44c7-81cf-4127911df302" });
        }
    }
}
