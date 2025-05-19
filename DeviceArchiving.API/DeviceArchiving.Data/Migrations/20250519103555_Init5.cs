using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceArchiving.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Users_UserId",
                table: "Operations");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Users_UserId",
                table: "Operations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Users_UserId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Devices");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Users_UserId",
                table: "Operations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
