using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class StandardizeMultiTenancyBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_audit_logs_branches_sucursal_id",
                table: "audit_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_clients_branches_sucursal_id",
                table: "clients");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_branches_sucursal_id",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_invoices_branches_sucursal_id",
                table: "invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variants_branches_sucursal_id",
                table: "product_variants");

            migrationBuilder.DropForeignKey(
                name: "FK_products_branches_sucursal_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_branches_sucursal_id",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_suppliers_branches_sucursal_id",
                table: "suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_users_branches_sucursal_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "users",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_sucursal_id",
                table: "users",
                newName: "IX_users_branch_id");

            migrationBuilder.RenameColumn(
                name: "SucursalId",
                table: "SystemSettings",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_SystemSettings_Key_SucursalId",
                table: "SystemSettings",
                newName: "IX_SystemSettings_Key_branch_id");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "suppliers",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_suppliers_sucursal_id",
                table: "suppliers",
                newName: "IX_suppliers_branch_id");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "purchases",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_sucursal_id",
                table: "purchases",
                newName: "IX_purchases_branch_id");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "products",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_products_sucursal_id",
                table: "products",
                newName: "IX_products_branch_id");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "product_variants",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_product_variants_sucursal_id",
                table: "product_variants",
                newName: "IX_product_variants_branch_id");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "invoices",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_invoices_sucursal_id",
                table: "invoices",
                newName: "IX_invoices_branch_id");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "inventory_transactions",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_inventory_transactions_sucursal_id",
                table: "inventory_transactions",
                newName: "IX_inventory_transactions_branch_id");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "clients",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_clients_sucursal_id",
                table: "clients",
                newName: "IX_clients_branch_id");

            migrationBuilder.RenameColumn(
                name: "sucursal_id",
                table: "audit_logs",
                newName: "branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_sucursal_id",
                table: "audit_logs",
                newName: "IX_audit_logs_branch_id");

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "purchases",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxTotal",
                table: "purchases",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "purchase_items",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "purchase_items",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "activity_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    message = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    details = table.Column<string>(type: "text", nullable: true),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_activity_logs_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_activity_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_branch_id",
                table: "activity_logs",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_user_id",
                table: "activity_logs",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_audit_logs_branches_branch_id",
                table: "audit_logs",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_clients_branches_branch_id",
                table: "clients",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_branches_branch_id",
                table: "inventory_transactions",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_branches_branch_id",
                table: "invoices",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variants_branches_branch_id",
                table: "product_variants",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_branches_branch_id",
                table: "products",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_branches_branch_id",
                table: "purchases",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_suppliers_branches_branch_id",
                table: "suppliers",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_branches_branch_id",
                table: "users",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_audit_logs_branches_branch_id",
                table: "audit_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_clients_branches_branch_id",
                table: "clients");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_branches_branch_id",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_invoices_branches_branch_id",
                table: "invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variants_branches_branch_id",
                table: "product_variants");

            migrationBuilder.DropForeignKey(
                name: "FK_products_branches_branch_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_branches_branch_id",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_suppliers_branches_branch_id",
                table: "suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_users_branches_branch_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "activity_logs");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "TaxTotal",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "purchase_items");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "purchase_items");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "users",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_branch_id",
                table: "users",
                newName: "IX_users_sucursal_id");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "SystemSettings",
                newName: "SucursalId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemSettings_Key_branch_id",
                table: "SystemSettings",
                newName: "IX_SystemSettings_Key_SucursalId");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "suppliers",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_suppliers_branch_id",
                table: "suppliers",
                newName: "IX_suppliers_sucursal_id");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "purchases",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_branch_id",
                table: "purchases",
                newName: "IX_purchases_sucursal_id");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "products",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_products_branch_id",
                table: "products",
                newName: "IX_products_sucursal_id");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "product_variants",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_product_variants_branch_id",
                table: "product_variants",
                newName: "IX_product_variants_sucursal_id");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "invoices",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_invoices_branch_id",
                table: "invoices",
                newName: "IX_invoices_sucursal_id");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "inventory_transactions",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_inventory_transactions_branch_id",
                table: "inventory_transactions",
                newName: "IX_inventory_transactions_sucursal_id");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "clients",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_clients_branch_id",
                table: "clients",
                newName: "IX_clients_sucursal_id");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "audit_logs",
                newName: "sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_branch_id",
                table: "audit_logs",
                newName: "IX_audit_logs_sucursal_id");

            migrationBuilder.AddForeignKey(
                name: "FK_audit_logs_branches_sucursal_id",
                table: "audit_logs",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_clients_branches_sucursal_id",
                table: "clients",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_branches_sucursal_id",
                table: "inventory_transactions",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_branches_sucursal_id",
                table: "invoices",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variants_branches_sucursal_id",
                table: "product_variants",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_branches_sucursal_id",
                table: "products",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_branches_sucursal_id",
                table: "purchases",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_suppliers_branches_sucursal_id",
                table: "suppliers",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_branches_sucursal_id",
                table: "users",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
