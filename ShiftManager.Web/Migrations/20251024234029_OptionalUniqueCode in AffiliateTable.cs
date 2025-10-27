using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftManager.Web.Migrations
{
    /// <inheritdoc />
    public partial class OptionalUniqueCodeinAffiliateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UniqueCode",
                table: "Affiliates",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Affiliates",
                keyColumn: "UniqueCode",
                keyValue: null,
                column: "UniqueCode",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "UniqueCode",
                table: "Affiliates",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
