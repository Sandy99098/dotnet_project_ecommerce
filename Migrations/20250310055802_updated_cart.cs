using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_project_ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class updated_cart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tbl_cart_cust_id",
                table: "tbl_cart",
                column: "cust_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_cart_prod_id",
                table: "tbl_cart",
                column: "prod_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_cart_tbl_customer_cust_id",
                table: "tbl_cart",
                column: "cust_id",
                principalTable: "tbl_customer",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_cart_tbl_product_prod_id",
                table: "tbl_cart",
                column: "prod_id",
                principalTable: "tbl_product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_cart_tbl_customer_cust_id",
                table: "tbl_cart");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_cart_tbl_product_prod_id",
                table: "tbl_cart");

            migrationBuilder.DropIndex(
                name: "IX_tbl_cart_cust_id",
                table: "tbl_cart");

            migrationBuilder.DropIndex(
                name: "IX_tbl_cart_prod_id",
                table: "tbl_cart");
        }
    }
}
