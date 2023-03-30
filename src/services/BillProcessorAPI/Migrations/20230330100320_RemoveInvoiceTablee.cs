using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInvoiceTablee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiptUrl",
                table: "Receipts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptUrl",
                table: "Receipts");
        }
    }
}
