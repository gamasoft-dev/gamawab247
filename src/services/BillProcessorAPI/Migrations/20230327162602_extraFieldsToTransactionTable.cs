using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class extraFieldsToTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DueDate",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "BillTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevName",
                table: "BillTransactions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "BillTransactions");

            migrationBuilder.DropColumn(
                name: "RevName",
                table: "BillTransactions");
        }
    }
}
