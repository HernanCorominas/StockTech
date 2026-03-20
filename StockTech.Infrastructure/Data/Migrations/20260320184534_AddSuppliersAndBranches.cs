using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSuppliersAndBranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "supplier",
                table: "purchases");

            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                table: "purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "supplier_id",
                table: "purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                table: "invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "branches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    manager_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_branches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    contact_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tax_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_purchases_branch_id",
                table: "purchases",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_supplier_id",
                table: "purchases",
                column: "supplier_id");

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
                name: "FK_purchases_suppliers_supplier_id",
                table: "purchases",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoices_branches_branch_id",
                table: "invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_branches_branch_id",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_suppliers_supplier_id",
                table: "purchases");

            migrationBuilder.DropTable(
                name: "branches");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropIndex(
                name: "IX_purchases_branch_id",
                table: "purchases");

            migrationBuilder.DropIndex(
                name: "IX_purchases_supplier_id",
                table: "purchases");

            migrationBuilder.DropIndex(
                name: "IX_invoices_branch_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "branch_id",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "supplier_id",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "branch_id",
                table: "invoices");

            migrationBuilder.AddColumn<string>(
                name: "supplier",
                table: "purchases",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
