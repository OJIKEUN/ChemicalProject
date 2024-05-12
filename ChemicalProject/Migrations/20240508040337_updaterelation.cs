using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalProject.Migrations
{
    /// <inheritdoc />
    public partial class updaterelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wastes_Records_RecordId",
                table: "Wastes");

            migrationBuilder.DropIndex(
                name: "IX_Wastes_RecordId",
                table: "Wastes");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "Wastes");

            migrationBuilder.AddColumn<int>(
                name: "WasteId",
                table: "Records",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Records_WasteId",
                table: "Records",
                column: "WasteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Wastes_WasteId",
                table: "Records",
                column: "WasteId",
                principalTable: "Wastes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Wastes_WasteId",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_WasteId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "WasteId",
                table: "Records");

            migrationBuilder.AddColumn<int>(
                name: "RecordId",
                table: "Wastes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Wastes_RecordId",
                table: "Wastes",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wastes_Records_RecordId",
                table: "Wastes",
                column: "RecordId",
                principalTable: "Records",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
