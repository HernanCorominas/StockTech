using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeSucursalIdNullableGloballyFinalFixedFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "sucursal_id",
                table: "audit_logs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_sucursal_id",
                table: "audit_logs",
                column: "sucursal_id");

            migrationBuilder.AddForeignKey(
                name: "FK_audit_logs_branches_sucursal_id",
                table: "audit_logs",
                column: "sucursal_id",
                principalTable: "branches",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_audit_logs_branches_sucursal_id",
                table: "audit_logs");

            migrationBuilder.DropIndex(
                name: "IX_audit_logs_sucursal_id",
                table: "audit_logs");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "audit_logs");
        }
    }
}
