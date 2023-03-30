using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillTransactions_Receipts_ReceiptsId",
                table: "BillTransactions");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_BillTransactions_ReceiptsId",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "ReceiptsId",
                table: "BillTransactions");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "Receipts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts",
                column: "TransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_BillTransactions_TransactionId",
                table: "Receipts",
                column: "TransactionId",
                principalTable: "BillTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_BillTransactions_TransactionId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_TransactionId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Receipts");

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiptsId",
                table: "BillTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountInfoRequestData = table.Column<string>(type: "text", nullable: true),
                    AccountInfoResponseData = table.Column<string>(type: "text", nullable: true),
                    AcctCloseDate = table.Column<string>(type: "text", nullable: true),
                    AgencyCode = table.Column<string>(type: "text", nullable: true),
                    AgencyName = table.Column<string>(type: "text", nullable: true),
                    AmountDue = table.Column<decimal>(type: "numeric", nullable: false),
                    CbnCode = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    OraAgencyRev = table.Column<string>(type: "text", nullable: true),
                    PayerName = table.Column<string>(type: "text", nullable: true),
                    PaymentFlag = table.Column<string>(type: "text", nullable: true),
                    Pid = table.Column<string>(type: "text", nullable: true),
                    RevName = table.Column<string>(type: "text", nullable: true),
                    RevenueCode = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
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
    }
}
