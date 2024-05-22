using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalProject.Migrations
{
    /// <inheritdoc />
    public partial class addEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailManager",
                table: "UserAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmailUser",
                table: "UserAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailManager",
                table: "UserAreas");

            migrationBuilder.DropColumn(
                name: "EmailUser",
                table: "UserAreas");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Chemicals");
        }
    }
}
