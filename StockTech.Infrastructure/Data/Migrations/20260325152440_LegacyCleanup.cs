using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class LegacyCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "manager_name",
                table: "branches");

            // Safety check: drop manager_id if it partially exists from previous failed attempts
            migrationBuilder.Sql("ALTER TABLE branches DROP COLUMN IF EXISTS manager_id CASCADE;");

            migrationBuilder.AddColumn<Guid>(
                name: "manager_id",
                table: "branches",
                type: "uuid",
                nullable: true);

            // Cleanup legacy data (branches without a manager)
            migrationBuilder.Sql("DELETE FROM inventory_transactions WHERE sucursal_id IN (SELECT id FROM branches WHERE manager_id IS NULL);");
            migrationBuilder.Sql("DELETE FROM invoice_items WHERE invoice_id IN (SELECT id FROM invoices WHERE sucursal_id IN (SELECT id FROM branches WHERE manager_id IS NULL));");
            migrationBuilder.Sql("DELETE FROM invoices WHERE sucursal_id IN (SELECT id FROM branches WHERE manager_id IS NULL);");
            migrationBuilder.Sql("DELETE FROM purchase_items WHERE purchase_id IN (SELECT id FROM purchases WHERE sucursal_id IN (SELECT id FROM branches WHERE manager_id IS NULL));");
            migrationBuilder.Sql("DELETE FROM purchases WHERE sucursal_id IN (SELECT id FROM branches WHERE manager_id IS NULL);");
            migrationBuilder.Sql("DELETE FROM product_variants WHERE sucursal_id IN (SELECT id FROM branches WHERE manager_id IS NULL);");
            migrationBuilder.Sql("DELETE FROM audit_logs WHERE sucursal_id IN (SELECT id FROM branches WHERE manager_id IS NULL);");
            migrationBuilder.Sql("DELETE FROM branches WHERE manager_id IS NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_branches_manager_id",
                table: "branches",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_branches_name",
                table: "branches",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_branches_users_manager_id",
                table: "branches",
                column: "manager_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_branches_users_manager_id",
                table: "branches");

            migrationBuilder.DropIndex(
                name: "IX_branches_manager_id",
                table: "branches");

            migrationBuilder.DropIndex(
                name: "IX_branches_name",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "manager_id",
                table: "branches");

            migrationBuilder.AddColumn<string>(
                name: "manager_name",
                table: "branches",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
