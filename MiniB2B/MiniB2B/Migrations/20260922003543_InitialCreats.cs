using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniB2B.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SavedCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CardHolder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Last4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryMonth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedCards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedCards_UserId",
                table: "SavedCards",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedCards");

            migrationBuilder.DropColumn(
                name: "AddressLine",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Users");
        }
    }
}
