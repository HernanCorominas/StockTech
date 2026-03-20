using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_purchases_suppliers_supplier_id",
                table: "purchases");

            migrationBuilder.AlterColumn<Guid>(
                name: "supplier_id",
                table: "purchases",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "inventory_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    previous_stock = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    new_stock = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    reference_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    transaction_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    purchase_id = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_transactions_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_product_id",
                table: "inventory_transactions",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_suppliers_supplier_id",
                table: "purchases",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_purchases_suppliers_supplier_id",
                table: "purchases");

            migrationBuilder.DropTable(
                name: "inventory_transactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "supplier_id",
                table: "purchases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_suppliers_supplier_id",
                table: "purchases",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
