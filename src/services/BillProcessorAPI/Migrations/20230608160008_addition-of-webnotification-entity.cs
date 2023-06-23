using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class additionofwebnotificationentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillTransactions_BillPayers_BillPayerInfoId",
                table: "BillTransactions");


            migrationBuilder.CreateTable(
                name: "Webhooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WebGuid = table.Column<string>(type: "text", nullable: true),
                    ResponseCode = table.Column<string>(type: "text", nullable: true),
                    ResponseDesc = table.Column<string>(type: "text", nullable: true),
                    ReceiptNumber = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    TransID = table.Column<string>(type: "text", nullable: true),
                    TransCode = table.Column<string>(type: "text", nullable: true),
                    StatusMessage = table.Column<string>(type: "text", nullable: true),
                    PropertyAddress = table.Column<string>(type: "text", nullable: true),
                    TransactionReference = table.Column<string>(type: "text", nullable: true),
                    PaymentRef = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true),
                    GatewayType = table.Column<string>(type: "text", nullable: true),
                    Remark = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Webhooks", x => x.Id);
                });

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

            migrationBuilder.DropTable(
                name: "Webhooks");

            migrationBuilder.AddColumn<Guid>(
                name: "BillPayerInfoId1",
                table: "BillTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillTransactions_BillPayerInfoId1",
                table: "BillTransactions",
                column: "BillPayerInfoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BillTransactions_BillPayers_BillPayerInfoId",
                table: "BillTransactions",
                column: "BillPayerInfoId",
                principalTable: "BillPayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BillTransactions_BillPayers_BillPayerInfoId1",
                table: "BillTransactions",
                column: "BillPayerInfoId1",
                principalTable: "BillPayers",
                principalColumn: "Id");
        }
    }
}
