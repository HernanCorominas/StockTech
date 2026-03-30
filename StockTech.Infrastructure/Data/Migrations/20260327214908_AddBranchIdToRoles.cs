using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchIdToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_activity_logs_branches_branch_id",
                table: "activity_logs");

            migrationBuilder.DropIndex(
                name: "IX_activity_logs_branch_id",
                table: "activity_logs");

            migrationBuilder.DropColumn(
                name: "branch_id",
                table: "activity_logs");

            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                table: "roles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "stock",
                table: "products",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "min_stock",
                table: "products",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "Stock",
                table: "product_variants",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinStock",
                table: "product_variants",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "VariantId",
                table: "inventory_transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "key_values",
                table: "audit_logs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_branch_id",
                table: "roles",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_VariantId",
                table: "inventory_transactions",
                column: "VariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_product_variants_VariantId",
                table: "inventory_transactions",
                column: "VariantId",
                principalTable: "product_variants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_roles_branches_branch_id",
                table: "roles",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_product_variants_VariantId",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_branches_branch_id",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "IX_roles_branch_id",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_VariantId",
                table: "inventory_transactions");

            migrationBuilder.DropColumn(
                name: "branch_id",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "VariantId",
                table: "inventory_transactions");

            migrationBuilder.AlterColumn<int>(
                name: "stock",
                table: "products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "min_stock",
                table: "products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "Stock",
                table: "product_variants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "MinStock",
                table: "product_variants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "key_values",
                table: "audit_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                table: "activity_logs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_branch_id",
                table: "activity_logs",
                column: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_activity_logs_branches_branch_id",
                table: "activity_logs",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
