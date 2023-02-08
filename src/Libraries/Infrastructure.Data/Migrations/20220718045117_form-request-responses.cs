using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class formrequestresponses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_formRequestResponses_BusinessForms_BusinessFormId",
                table: "formRequestResponses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_formRequestResponses",
                table: "formRequestResponses");

            migrationBuilder.RenameTable(
                name: "formRequestResponses",
                newName: "FormRequestResponses");

            migrationBuilder.RenameIndex(
                name: "IX_formRequestResponses_BusinessFormId",
                table: "FormRequestResponses",
                newName: "IX_FormRequestResponses_BusinessFormId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "FormRequestResponses",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Direction",
                table: "FormRequestResponses",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "IsSummaryMessage",
                table: "FormRequestResponses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormRequestResponses",
                table: "FormRequestResponses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                table: "FormRequestResponses",
                column: "BusinessFormId",
                principalTable: "BusinessForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                table: "FormRequestResponses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormRequestResponses",
                table: "FormRequestResponses");

            migrationBuilder.DropColumn(
                name: "IsSummaryMessage",
                table: "FormRequestResponses");

            migrationBuilder.RenameTable(
                name: "FormRequestResponses",
                newName: "formRequestResponses");

            migrationBuilder.RenameIndex(
                name: "IX_FormRequestResponses_BusinessFormId",
                table: "formRequestResponses",
                newName: "IX_formRequestResponses_BusinessFormId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "formRequestResponses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Direction",
                table: "formRequestResponses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_formRequestResponses",
                table: "formRequestResponses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_formRequestResponses_BusinessForms_BusinessFormId",
                table: "formRequestResponses",
                column: "BusinessFormId",
                principalTable: "BusinessForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
