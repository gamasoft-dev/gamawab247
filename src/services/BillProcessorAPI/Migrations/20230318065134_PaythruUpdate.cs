using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class PaythruUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SystemReference",
                table: "BillTransactions",
                newName: "TransactionReference");

            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuccessIndicator",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "billCode",
                table: "BillPayers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "SuccessIndicator",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "billCode",
                table: "BillPayers");

            migrationBuilder.RenameColumn(
                name: "TransactionReference",
                table: "BillTransactions",
                newName: "SystemReference");
        }
    }
}
