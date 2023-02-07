using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                table: "BillTransactions");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "BillTransactions",
                newName: "PrinciPay");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "BillTransactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BillAmount",
                table: "BillTransactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayTransactionReference",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Narration",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptUrl",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourcePIN",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemReference",
                table: "BillTransactions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PropertyNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "BillAmount",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "GatewayTransactionReference",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "Narration",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "ReceiptUrl",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "ResourcePIN",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "SystemReference",
                table: "BillTransactions");

            migrationBuilder.RenameColumn(
                name: "PrinciPay",
                table: "BillTransactions",
                newName: "Amount");

            migrationBuilder.AddColumn<Guid>(
                name: "Reference",
                table: "BillTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
