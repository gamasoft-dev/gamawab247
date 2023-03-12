using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateBillProcessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillPayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayerName = table.Column<string>(type: "text", nullable: true),
                    AmountDue = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CreditAccount = table.Column<string>(type: "text", nullable: true),
                    AgencyCode = table.Column<string>(type: "text", nullable: true),
                    RevenueCode = table.Column<string>(type: "text", nullable: true),
                    OraAgencyRev = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    StatusMessage = table.Column<string>(type: "text", nullable: true),
                    Pid = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    AcctCloseDate = table.Column<string>(type: "text", nullable: true),
                    ReadOnly = table.Column<string>(type: "text", nullable: true),
                    MinAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentFlag = table.Column<string>(type: "text", nullable: true),
                    CbnCode = table.Column<string>(type: "text", nullable: true),
                    AgencyName = table.Column<string>(type: "text", nullable: true),
                    RevName = table.Column<string>(type: "text", nullable: true),
                    AccountInfoResponseData = table.Column<string>(type: "text", nullable: true),
                    AccountInfoRequestData = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Charges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChannelModel = table.Column<string>(type: "text", nullable: true),
                    MaxChargeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MinChargeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PercentageCharge = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    GatewayType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    SystemReference = table.Column<string>(type: "text", nullable: true),
                    GatewayTransactionReference = table.Column<string>(type: "text", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    BillAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PrinciPalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionCharge = table.Column<decimal>(type: "numeric", nullable: false),
                    GatewayTransactionCharge = table.Column<decimal>(type: "numeric", nullable: false),
                    Narration = table.Column<string>(type: "text", nullable: true),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    ResourcePIN = table.Column<string>(type: "text", nullable: true),
                    ReceiptUrl = table.Column<string>(type: "text", nullable: true),
                    Receipt = table.Column<string>(type: "text", nullable: true),
                    StatusMessage = table.Column<string>(type: "text", nullable: true),
                    PaymentInfoRequestData = table.Column<string>(type: "text", nullable: true),
                    PaymentInfoResponseData = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillTransactions_BillPayers_UserId",
                        column: x => x.UserId,
                        principalTable: "BillPayers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillTransactions_UserId",
                table: "BillTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillTransactions");

            migrationBuilder.DropTable(
                name: "Charges");

            migrationBuilder.DropTable(
                name: "BillPayers");
        }
    }
}
