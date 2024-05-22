using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalProject.Migrations
{
    /// <inheritdoc />
    public partial class updatetable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chemicals_Areas_AreaId",
                table: "Chemicals");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Chemicals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chemicals_Areas_AreaId",
                table: "Chemicals",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chemicals_Areas_AreaId",
                table: "Chemicals");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Chemicals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Chemicals_Areas_AreaId",
                table: "Chemicals",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id");
        }
    }
}
