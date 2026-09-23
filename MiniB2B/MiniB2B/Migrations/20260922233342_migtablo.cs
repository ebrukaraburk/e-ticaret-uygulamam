using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniB2B.Migrations
{
    /// <inheritdoc />
    public partial class migtablo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdminOnly",
                table: "GridColumnConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "GridColumnConfigs",
                columns: new[] { "Id", "AdminOnly", "ColumnTitle", "DisplayOrder", "IsVisible", "PropertyName", "RenderType" },
                values: new object[,]
                {
                    { 1, false, "Görsel", 1, true, "ImageUrl", "Image" },
                    { 2, false, "Ürün Kodu", 2, true, "ProductCode", "Text" },
                    { 3, false, "Ürün Adı", 3, true, "ProductName", "Text" },
                    { 4, false, "Marka", 4, true, "Brand", "Text" },
                    { 5, false, "Üretici Kodu", 5, true, "ManufacturerCode", "Text" },
                    { 6, true, "Özel Kod 1", 6, true, "SpecialCode1", "Text" },
                    { 7, true, "Özel Kod 2", 7, true, "SpecialCode2", "Text" },
                    { 8, false, "Stok Durumu", 8, true, "StockQuantity", "Badge" },
                    { 9, false, "Birim Fiyat", 9, true, "UnitPrice", "Currency" },
                    { 10, false, "Sipariş", 10, true, "Id", "Input" },
                    { 11, false, "İşlem", 11, true, "Id", "Action" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "GridColumnConfigs",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DropColumn(
                name: "AdminOnly",
                table: "GridColumnConfigs");
        }
    }
}
