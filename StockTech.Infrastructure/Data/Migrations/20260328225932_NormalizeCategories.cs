using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "category",
                table: "products");

            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("f3b8d1b6-0b3b-4b1a-9c1a-1a2b3c4d5e6f"), new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2398), "Complementos y accesorios varios", true, "Accesorios", new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2398) },
                    { new Guid("fa2f082d-72a2-b281-0081-8b9cad0e1f20"), new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2392), "Equipos electrónicos y accesorios", true, "Tecnología", new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2392) },
                    { new Guid("fb4c9e2c-71c4-5c2b-ac2b-2b3c4d5e6f7a"), new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2403), "Prendas de vestir y calzado", true, "Ropa", new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2403) },
                    { new Guid("fb8d4fae-7dec-11d0-a765-00a0c91e6bf6"), new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2355), "Productos de cuidado personal y belleza", true, "Belleza", new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2361) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_Categories_category_id",
                table: "products",
                column: "category_id",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_Categories_category_id",
                table: "products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_products_category_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
