using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalProject.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentStockPropertyInActualModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStock",
                table: "ActualRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStock",
                table: "ActualRecords");
        }
    }
}
