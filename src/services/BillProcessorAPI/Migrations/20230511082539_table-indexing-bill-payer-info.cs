using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillProcessorAPI.Migrations
{
    /// <inheritdoc />
    public partial class tableindexingbillpayerinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillTransactions_BillPayers_BillPayerInfoId",
                table: "BillTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_BillTransactions_BillNumber",
                table: "BillTransactions",
                column: "BillNumber");

       
            migrationBuilder.CreateIndex(
                name: "IX_BillTransactions_Status",
                table: "BillTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BillTransactions_UpdatedAt",
                table: "BillTransactions",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayers_billCode",
                table: "BillPayers",
                column: "billCode");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayers_UpdatedAt",
                table: "BillPayers",
                column: "UpdatedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_BillTransactions_BillPayers_BillPayerInfoId",
                table: "BillTransactions",
                column: "BillPayerInfoId",
                principalTable: "BillPayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillTransactions_BillPayers_BillPayerInfoId",
                table: "BillTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BillTransactions_BillNumber",
                table: "BillTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BillTransactions_Status",
                table: "BillTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BillTransactions_UpdatedAt",
                table: "BillTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BillPayers_billCode",
                table: "BillPayers");

            migrationBuilder.DropIndex(
                name: "IX_BillPayers_UpdatedAt",
                table: "BillPayers");


            migrationBuilder.AddForeignKey(
                name: "FK_BillTransactions_BillPayers_BillPayerInfoId",
                table: "BillTransactions",
                column: "BillPayerInfoId",
                principalTable: "BillPayers",
                principalColumn: "Id");
        }
    }
}
