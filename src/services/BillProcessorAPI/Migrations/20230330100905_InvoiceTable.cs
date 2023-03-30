using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BillTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayerName = table.Column<string>(type: "text", nullable: true),
                    BillNumber = table.Column<string>(type: "text", nullable: true),
                    Pid = table.Column<string>(type: "text", nullable: true),
                    GatewayType = table.Column<int>(type: "integer", nullable: false),
                    TransactionReference = table.Column<string>(type: "text", nullable: true),
                    GatewayTransactionReference = table.Column<string>(type: "text", nullable: true),
                    RevName = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    DueDate = table.Column<string>(type: "text", nullable: true),
                    AmountDue = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionCharge = table.Column<decimal>(type: "numeric", nullable: false),
                    GatewayTransactionCharge = table.Column<decimal>(type: "numeric", nullable: false),
                    ReceiptUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_BillTransactions_BillTransactionId",
                        column: x => x.BillTransactionId,
                        principalTable: "BillTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BillTransactionId",
                table: "Invoices",
                column: "BillTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ReceiptId",
                table: "Invoices",
                column: "ReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");
        }
    }
}
