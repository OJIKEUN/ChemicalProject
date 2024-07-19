using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalProject.Migrations
{
    /// <inheritdoc />
    public partial class Publish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CC_Schema");

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "CC_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wastes",
                schema: "CC_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Badge = table.Column<int>(type: "int", nullable: false),
                    WasteType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WasteQuantity = table.Column<int>(type: "int", nullable: false),
                    WasteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wastes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chemicals",
                schema: "CC_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Badge = table.Column<int>(type: "int", nullable: false),
                    ChemicalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Packaging = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumStock = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    CostCentre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Justify = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    StatusManager = table.Column<bool>(type: "bit", nullable: true),
                    RemarkManager = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalDateManager = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusESH = table.Column<bool>(type: "bit", nullable: true),
                    RemarkESH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalDateESH = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chemicals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chemicals_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "CC_Schema",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAdmins",
                schema: "CC_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAdmins_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "CC_Schema",
                        principalTable: "Areas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserAreas",
                schema: "CC_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailManager = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAreas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "CC_Schema",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserManagers",
                schema: "CC_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserManagers_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "CC_Schema",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSuperVisors",
                schema: "CC_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSuperVisors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSuperVisors_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "CC_Schema",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActualRecords",
                schema: "CC_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Badge = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChemicalId = table.Column<int>(type: "int", nullable: false),
                    CurrentStock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActualRecords_Chemicals_ChemicalId",
                        column: x => x.ChemicalId,
                        principalSchema: "CC_Schema",
                        principalTable: "Chemicals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                schema: "CC_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChemicalId = table.Column<int>(type: "int", nullable: false),
                    Badge = table.Column<int>(type: "int", nullable: false),
                    ReceivedQuantity = table.Column<int>(type: "int", nullable: false),
                    Consumption = table.Column<int>(type: "int", nullable: false),
                    Justify = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WasteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Records_Chemicals_ChemicalId",
                        column: x => x.ChemicalId,
                        principalSchema: "CC_Schema",
                        principalTable: "Chemicals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Records_Wastes_WasteId",
                        column: x => x.WasteId,
                        principalSchema: "CC_Schema",
                        principalTable: "Wastes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActualRecords_ChemicalId",
                schema: "CC_Schema",
                table: "ActualRecords",
                column: "ChemicalId");

            migrationBuilder.CreateIndex(
                name: "IX_Chemicals_AreaId",
                schema: "CC_Schema",
                table: "Chemicals",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_ChemicalId",
                schema: "CC_Schema",
                table: "Records",
                column: "ChemicalId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_WasteId",
                schema: "CC_Schema",
                table: "Records",
                column: "WasteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAdmins_AreaId",
                schema: "CC_Schema",
                table: "UserAdmins",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAreas_AreaId",
                schema: "CC_Schema",
                table: "UserAreas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserManagers_AreaId",
                schema: "CC_Schema",
                table: "UserManagers",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSuperVisors_AreaId",
                schema: "CC_Schema",
                table: "UserSuperVisors",
                column: "AreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActualRecords",
                schema: "CC_Schema");

            migrationBuilder.DropTable(
                name: "Records",
                schema: "CC_Schema");

            migrationBuilder.DropTable(
                name: "UserAdmins",
                schema: "CC_Schema");

            migrationBuilder.DropTable(
                name: "UserAreas",
                schema: "CC_Schema");

            migrationBuilder.DropTable(
                name: "UserManagers",
                schema: "CC_Schema");

            migrationBuilder.DropTable(
                name: "UserSuperVisors",
                schema: "CC_Schema");

            migrationBuilder.DropTable(
                name: "Chemicals",
                schema: "CC_Schema");

            migrationBuilder.DropTable(
                name: "Wastes",
                schema: "CC_Schema");

            migrationBuilder.DropTable(
                name: "Areas",
                schema: "CC_Schema");
        }
    }
}
