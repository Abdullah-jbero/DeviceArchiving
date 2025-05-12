using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceArchiving.Data.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceOperations");

            migrationBuilder.DropIndex(
                name: "IX_Devices_SerialNumber",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OperationDate",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Operations");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Operations",
                newName: "OperationName");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Devices",
                newName: "WindowsPassword");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Devices",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Devices",
                newName: "SystemPassword");

            migrationBuilder.RenameColumn(
                name: "EncryptionKey",
                table: "Devices",
                newName: "Source");

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Operations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "newValue",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oldValue",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "BrotherName",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Card",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FreezePassword",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HardDrivePassword",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LaptopName",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OperationsTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationsTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_DeviceId",
                table: "Operations",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Devices_DeviceId",
                table: "Operations",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Devices_DeviceId",
                table: "Operations");

            migrationBuilder.DropTable(
                name: "OperationsTypes");

            migrationBuilder.DropIndex(
                name: "IX_Operations_DeviceId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "newValue",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "oldValue",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "BrotherName",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Card",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "FreezePassword",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "HardDrivePassword",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "LaptopName",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "OperationName",
                table: "Operations",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "WindowsPassword",
                table: "Devices",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Devices",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "SystemPassword",
                table: "Devices",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Devices",
                newName: "EncryptionKey");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OperationDate",
                table: "Operations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Operations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "Devices",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "DeviceOperations",
                columns: table => new
                {
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    OperationId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceOperations", x => new { x.DeviceId, x.OperationId });
                    table.ForeignKey(
                        name: "FK_DeviceOperations_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceOperations_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SerialNumber",
                table: "Devices",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceOperations_OperationId",
                table: "DeviceOperations",
                column: "OperationId");
        }
    }
}
