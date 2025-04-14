using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_project_ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentTransactionModelEsewa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_CustomerId",
                table: "PaymentTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_ProductId",
                table: "PaymentTransactions",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_tbl_customer_CustomerId",
                table: "PaymentTransactions",
                column: "CustomerId",
                principalTable: "tbl_customer",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_tbl_product_ProductId",
                table: "PaymentTransactions",
                column: "ProductId",
                principalTable: "tbl_product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_tbl_customer_CustomerId",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_tbl_product_ProductId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_CustomerId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_ProductId",
                table: "PaymentTransactions");
        }
    }
}
