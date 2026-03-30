using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "products",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f3b8d1b6-0b3b-4b1a-9c1a-1a2b3c4d5e6f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 23, 6, 42, 663, DateTimeKind.Utc).AddTicks(6450), new DateTime(2026, 3, 28, 23, 6, 42, 663, DateTimeKind.Utc).AddTicks(6451) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fa2f082d-72a2-b281-0081-8b9cad0e1f20"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 23, 6, 42, 663, DateTimeKind.Utc).AddTicks(6442), new DateTime(2026, 3, 28, 23, 6, 42, 663, DateTimeKind.Utc).AddTicks(6443) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fb4c9e2c-71c4-5c2b-ac2b-2b3c4d5e6f7a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 23, 6, 42, 663, DateTimeKind.Utc).AddTicks(6455), new DateTime(2026, 3, 28, 23, 6, 42, 663, DateTimeKind.Utc).AddTicks(6455) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fb8d4fae-7dec-11d0-a765-00a0c91e6bf6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 23, 6, 42, 663, DateTimeKind.Utc).AddTicks(6371), new DateTime(2026, 3, 28, 23, 6, 42, 663, DateTimeKind.Utc).AddTicks(6376) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "products");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f3b8d1b6-0b3b-4b1a-9c1a-1a2b3c4d5e6f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2398), new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2398) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fa2f082d-72a2-b281-0081-8b9cad0e1f20"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2392), new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2392) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fb4c9e2c-71c4-5c2b-ac2b-2b3c4d5e6f7a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2403), new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2403) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fb8d4fae-7dec-11d0-a765-00a0c91e6bf6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2355), new DateTime(2026, 3, 28, 22, 59, 30, 490, DateTimeKind.Utc).AddTicks(2361) });
        }
    }
}
