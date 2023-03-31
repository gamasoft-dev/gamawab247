using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class billInvoiceRefactorUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Receipts_ReceiptId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ReceiptId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "Invoices");

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceId",
                table: "Receipts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_InvoiceId",
                table: "Receipts",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Invoices_InvoiceId",
                table: "Receipts",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Invoices_InvoiceId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_InvoiceId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Receipts");

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiptId",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ReceiptId",
                table: "Invoices",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Receipts_ReceiptId",
                table: "Invoices",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id");
        }
    }
}
