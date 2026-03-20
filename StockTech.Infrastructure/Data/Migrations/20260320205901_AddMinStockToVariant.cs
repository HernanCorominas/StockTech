using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMinStockToVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinStock",
                table: "product_variants",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinStock",
                table: "product_variants");
        }
    }
}
