using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftManager.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddroleandpasswordinAffiliate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Affiliates",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Affiliates",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Affiliates");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Affiliates");
        }
    }
}
