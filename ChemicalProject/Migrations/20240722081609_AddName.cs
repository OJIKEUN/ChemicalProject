using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalProject.Migrations
{
    /// <inheritdoc />
    public partial class AddName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "CC_Schema",
                table: "Records",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "CC_Schema",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "CC_Schema",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "CC_Schema",
                table: "Chemicals");
        }
    }
}
