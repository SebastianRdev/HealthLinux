using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftManager.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEnumDocumentTypeinAffiliate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentType",
                table: "Affiliates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "Affiliates");
        }
    }
}
