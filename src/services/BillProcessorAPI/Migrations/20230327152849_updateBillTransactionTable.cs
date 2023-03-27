using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateBillTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillTransactions_BillPayers_UserId",
                table: "BillTransactions");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BillTransactions",
                newName: "BillPayerInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_BillTransactions_UserId",
                table: "BillTransactions",
                newName: "IX_BillTransactions_BillPayerInfoId");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountDue",
                table: "BillTransactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BillNumber",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayerName",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pid",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BillTransactions_BillPayers_BillPayerInfoId",
                table: "BillTransactions",
                column: "BillPayerInfoId",
                principalTable: "BillPayers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillTransactions_BillPayers_BillPayerInfoId",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "AmountDue",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "BillNumber",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "PayerName",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "Pid",
                table: "BillTransactions");

            migrationBuilder.RenameColumn(
                name: "BillPayerInfoId",
                table: "BillTransactions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BillTransactions_BillPayerInfoId",
                table: "BillTransactions",
                newName: "IX_BillTransactions_UserId");

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AmountDue = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    Gateway = table.Column<int>(type: "integer", nullable: false),
                    PaymentReference = table.Column<string>(type: "text", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BillTransactions_BillPayers_UserId",
                table: "BillTransactions",
                column: "UserId",
                principalTable: "BillPayers",
                principalColumn: "Id");
        }
    }
}
