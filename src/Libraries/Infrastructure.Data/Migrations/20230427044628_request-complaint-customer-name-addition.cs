using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class requestcomplaintcustomernameaddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "RequestAndComplaints",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "RequestAndComplaints",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestAndComplaints_CreatedAt",
                table: "RequestAndComplaints",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAndComplaints_CustomerId",
                table: "RequestAndComplaints",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAndComplaints_ResolutionDate",
                table: "RequestAndComplaints",
                column: "ResolutionDate");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAndComplaints_TicketId",
                table: "RequestAndComplaints",
                column: "TicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestAndComplaints_CreatedAt",
                table: "RequestAndComplaints");

            migrationBuilder.DropIndex(
                name: "IX_RequestAndComplaints_CustomerId",
                table: "RequestAndComplaints");

            migrationBuilder.DropIndex(
                name: "IX_RequestAndComplaints_ResolutionDate",
                table: "RequestAndComplaints");

            migrationBuilder.DropIndex(
                name: "IX_RequestAndComplaints_TicketId",
                table: "RequestAndComplaints");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "RequestAndComplaints");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "RequestAndComplaints",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
