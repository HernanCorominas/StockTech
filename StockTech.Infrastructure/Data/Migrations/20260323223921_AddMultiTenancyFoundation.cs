using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancyFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoices_branches_branch_id",
                table: "invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_branches_branch_id",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_role_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_purchases_branch_id",
                table: "purchases");

            migrationBuilder.DropIndex(
                name: "IX_invoices_branch_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "branch_id",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "branch_id",
                table: "invoices");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "users",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_users_role_id",
                table: "users",
                newName: "IX_users_RoleId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "suppliers",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "products",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "clients",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "sucursal_id",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "sucursal_id",
                table: "suppliers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "sucursal_id",
                table: "purchases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "sucursal_id",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "sucursal_id",
                table: "product_variants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "sucursal_id",
                table: "invoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "sucursal_id",
                table: "inventory_transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "sucursal_id",
                table: "clients",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "branches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // --- CUSTOM START: Legacy Data Cleanup & Multi-tenancy Initialization ---
            
            // 1. Create Default Branch (Sucursal Principal)
            migrationBuilder.Sql(@"
                INSERT INTO branches (id, name, address, phone, manager_name, is_active, created_at, updated_at)
                VALUES ('00000000-0000-0000-0000-000000000000', 'Sucursal Principal', 'Dirección Sistema', '000-000-0000', 'System', true, now(), now())
                ON CONFLICT (id) DO NOTHING;
            ");

            // 2. Delete Legacy Business Data (Except Users)
            migrationBuilder.Sql("DELETE FROM inventory_transactions;");
            migrationBuilder.Sql("DELETE FROM invoice_items;");
            migrationBuilder.Sql("DELETE FROM invoices;");
            migrationBuilder.Sql("DELETE FROM purchase_items;");
            migrationBuilder.Sql("DELETE FROM purchases;");
            migrationBuilder.Sql("DELETE FROM product_variants;");
            migrationBuilder.Sql("DELETE FROM products;");
            migrationBuilder.Sql("DELETE FROM clients;");
            migrationBuilder.Sql("DELETE FROM suppliers;");

            // --- END CUSTOM START ---

            migrationBuilder.CreateIndex(
                name: "IX_users_sucursal_id",
                table: "users",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_sucursal_id",
                table: "suppliers",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_sucursal_id",
                table: "purchases",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_sucursal_id",
                table: "products",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_sucursal_id",
                table: "product_variants",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_sucursal_id",
                table: "invoices",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_sucursal_id",
                table: "inventory_transactions",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_clients_sucursal_id",
                table: "clients",
                column: "sucursal_id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_RoleId",
                table: "users",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_RoleId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_sucursal_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_suppliers_sucursal_id",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "IX_purchases_sucursal_id",
                table: "purchases");

            migrationBuilder.DropIndex(
                name: "IX_products_sucursal_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_product_variants_sucursal_id",
                table: "product_variants");

            migrationBuilder.DropIndex(
                name: "IX_invoices_sucursal_id",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_sucursal_id",
                table: "inventory_transactions");

            migrationBuilder.DropIndex(
                name: "IX_clients_sucursal_id",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "product_variants");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "inventory_transactions");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "branches");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "users",
                newName: "role_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_RoleId",
                table: "users",
                newName: "IX_users_role_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "suppliers",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "products",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "clients",
                newName: "created_at");

            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                table: "purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                table: "invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchases_branch_id",
                table: "purchases",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_branch_id",
                table: "invoices",
                column: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_branches_branch_id",
                table: "invoices",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_branches_branch_id",
                table: "purchases",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_role_id",
                table: "users",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
