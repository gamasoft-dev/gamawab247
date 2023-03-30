using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class nullableConstraintForInvoiceRecieptId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Receipts_ReceiptId",
                table: "Invoices");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiptId",
                table: "Invoices",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Receipts_ReceiptId",
                table: "Invoices",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Receipts_ReceiptId",
                table: "Invoices");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiptId",
                table: "Invoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Receipts_ReceiptId",
                table: "Invoices",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
