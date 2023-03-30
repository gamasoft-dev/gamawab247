using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class ReceiptTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Receipt",
                table: "BillTransactions",
                newName: "PaymentReference");

            migrationBuilder.RenameColumn(
                name: "BillAmount",
                table: "BillTransactions",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "DateCompleted",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiName",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiptsId",
                table: "BillTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionDate = table.Column<string>(type: "text", nullable: true),
                    GateWay = table.Column<string>(type: "text", nullable: true),
                    PaymentRef = table.Column<string>(type: "text", nullable: true),
                    GatewayTransactionReference = table.Column<string>(type: "text", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountDue = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillTransactions_ReceiptsId",
                table: "BillTransactions",
                column: "ReceiptsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillTransactions_Receipts_ReceiptsId",
                table: "BillTransactions",
                column: "ReceiptsId",
                principalTable: "Receipts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillTransactions_Receipts_ReceiptsId",
                table: "BillTransactions");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_BillTransactions_ReceiptsId",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "DateCompleted",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "FiName",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "ReceiptsId",
                table: "BillTransactions");

            migrationBuilder.RenameColumn(
                name: "PaymentReference",
                table: "BillTransactions",
                newName: "Receipt");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "BillTransactions",
                newName: "BillAmount");
        }
    }
}
