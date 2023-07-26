using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketKhoone.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class V2023_06_04_1311 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Variants_ColorCode",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_Variants_Value",
                table: "Variants");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_Value_ColorCode",
                table: "Variants",
                columns: new[] { "Value", "ColorCode" },
                unique: true,
                filter: "[ColorCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Variants_Value_ColorCode",
                table: "Variants");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_ColorCode",
                table: "Variants",
                column: "ColorCode",
                unique: true,
                filter: "[ColorCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_Value",
                table: "Variants",
                column: "Value",
                unique: true);
        }
    }
}
