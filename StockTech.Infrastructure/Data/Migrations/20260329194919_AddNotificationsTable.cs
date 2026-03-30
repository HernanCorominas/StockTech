using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "inventory_transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f3b8d1b6-0b3b-4b1a-9c1a-1a2b3c4d5e6f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 29, 19, 49, 17, 543, DateTimeKind.Utc).AddTicks(1134), new DateTime(2026, 3, 29, 19, 49, 17, 543, DateTimeKind.Utc).AddTicks(1135) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fa2f082d-72a2-b281-0081-8b9cad0e1f20"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 29, 19, 49, 17, 543, DateTimeKind.Utc).AddTicks(1099), new DateTime(2026, 3, 29, 19, 49, 17, 543, DateTimeKind.Utc).AddTicks(1100) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fb4c9e2c-71c4-5c2b-ac2b-2b3c4d5e6f7a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 29, 19, 49, 17, 543, DateTimeKind.Utc).AddTicks(1140), new DateTime(2026, 3, 29, 19, 49, 17, 543, DateTimeKind.Utc).AddTicks(1140) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fb8d4fae-7dec-11d0-a765-00a0c91e6bf6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 29, 19, 49, 17, 543, DateTimeKind.Utc).AddTicks(1062), new DateTime(2026, 3, 29, 19, 49, 17, 543, DateTimeKind.Utc).AddTicks(1067) });

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_UserId",
                table: "inventory_transactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_users_UserId",
                table: "inventory_transactions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_users_UserId",
                table: "inventory_transactions");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_UserId",
                table: "inventory_transactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "inventory_transactions");

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
    }
}
