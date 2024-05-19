using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalProject.Migrations
{
    /// <inheritdoc />
    public partial class updatetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAreas_Areas_AreaId",
                table: "UserAreas");

            migrationBuilder.DropForeignKey(
                name: "FK_UserManagers_Areas_AreaId",
                table: "UserManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSuperVisors_Areas_AreaId",
                table: "UserSuperVisors");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "UserSuperVisors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "UserManagers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "UserAreas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAreas_Areas_AreaId",
                table: "UserAreas",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserManagers_Areas_AreaId",
                table: "UserManagers",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSuperVisors_Areas_AreaId",
                table: "UserSuperVisors",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAreas_Areas_AreaId",
                table: "UserAreas");

            migrationBuilder.DropForeignKey(
                name: "FK_UserManagers_Areas_AreaId",
                table: "UserManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSuperVisors_Areas_AreaId",
                table: "UserSuperVisors");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "UserSuperVisors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "UserManagers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "UserAreas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAreas_Areas_AreaId",
                table: "UserAreas",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserManagers_Areas_AreaId",
                table: "UserManagers",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSuperVisors_Areas_AreaId",
                table: "UserSuperVisors",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id");
        }
    }
}
