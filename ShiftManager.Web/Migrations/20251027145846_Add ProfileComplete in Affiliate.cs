using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftManager.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileCompleteinAffiliate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProfileComplete",
                table: "Affiliates",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileComplete",
                table: "Affiliates");
        }
    }
}
