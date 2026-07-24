using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bankyer.Infrastructure.Migrations;

[DbContext(typeof(Bankyer.Infrastructure.Database.AppDbContext))]
[Migration("20260724123000_AddAccountUser")]
public partial class AddAccountUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Accounts_New",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Balance = table.Column<decimal>(type: "TEXT", nullable: false),
                CurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                UserId = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Accounts", account => account.Id);
                table.ForeignKey(
                    name: "FK_Accounts_Currencies_CurrencyCode",
                    column: account => account.CurrencyCode,
                    principalTable: "Currencies",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Accounts_AspNetUsers_UserId",
                    column: account => account.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.Sql(
            """
            INSERT INTO "Accounts_New" ("Id", "Status", "Balance", "CurrencyCode", "UserId")
            SELECT "Id", "Status", "Balance", "CurrencyCode", NULL
            FROM "Accounts";
            """);

        migrationBuilder.DropTable(name: "Accounts");

        migrationBuilder.RenameTable(
            name: "Accounts_New",
            newName: "Accounts");

        migrationBuilder.CreateIndex(
            name: "IX_Accounts_CurrencyCode",
            table: "Accounts",
            column: "CurrencyCode");

        migrationBuilder.CreateIndex(
            name: "IX_Accounts_UserId",
            table: "Accounts",
            column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Accounts_Old",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Balance = table.Column<decimal>(type: "TEXT", nullable: false),
                CurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Accounts", account => account.Id);
                table.ForeignKey(
                    name: "FK_Accounts_Currencies_CurrencyCode",
                    column: account => account.CurrencyCode,
                    principalTable: "Currencies",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.Sql(
            """
            INSERT INTO "Accounts_Old" ("Id", "Status", "Balance", "CurrencyCode")
            SELECT "Id", "Status", "Balance", "CurrencyCode"
            FROM "Accounts";
            """);

        migrationBuilder.DropTable(name: "Accounts");

        migrationBuilder.RenameTable(
            name: "Accounts_Old",
            newName: "Accounts");

        migrationBuilder.CreateIndex(
            name: "IX_Accounts_CurrencyCode",
            table: "Accounts",
            column: "CurrencyCode");
    }
}
