using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniB2B.Migrations
{
    /// <inheritdoc />
    public partial class eklentiler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alignment",
                table: "GridColumnConfigs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ColumnWidth",
                table: "GridColumnConfigs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VisibleOnDesktop",
                table: "GridColumnConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VisibleOnMobile",
                table: "GridColumnConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VisibleOnTablet",
                table: "GridColumnConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Center", "70px", true, true, true });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Left", "110px", true, false, true });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Left", "25%", true, true, true });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Left", "120px", true, false, true });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Left", "120px", true, false, false });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Left", "100px", true, false, false });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Left", "100px", true, false, false });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Center", "110px", true, true, true });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Right", "110px", true, true, true });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Center", "130px", true, true, true });

            migrationBuilder.UpdateData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Alignment", "ColumnWidth", "VisibleOnDesktop", "VisibleOnMobile", "VisibleOnTablet" },
                values: new object[] { "Right", "160px", true, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alignment",
                table: "GridColumnConfigs");

            migrationBuilder.DropColumn(
                name: "ColumnWidth",
                table: "GridColumnConfigs");

            migrationBuilder.DropColumn(
                name: "VisibleOnDesktop",
                table: "GridColumnConfigs");

            migrationBuilder.DropColumn(
                name: "VisibleOnMobile",
                table: "GridColumnConfigs");

            migrationBuilder.DropColumn(
                name: "VisibleOnTablet",
                table: "GridColumnConfigs");
        }
    }
}
