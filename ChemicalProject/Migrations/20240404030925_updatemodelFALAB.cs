using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalProject.Migrations
{
    /// <inheritdoc />
    public partial class updatemodelFALAB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "price",
                table: "Chemicals",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Chemicals",
                newName: "StatusManager");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDateESH",
                table: "Chemicals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDateManager",
                table: "Chemicals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinimumStock",
                table: "Chemicals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RemarkESH",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemarkManager",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "Chemicals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StatusESH",
                table: "Chemicals",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalDateESH",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ApprovalDateManager",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "MinimumStock",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RemarkESH",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RemarkManager",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "StatusESH",
                table: "Chemicals");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Chemicals",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "StatusManager",
                table: "Chemicals",
                newName: "Status");
        }
    }
}
