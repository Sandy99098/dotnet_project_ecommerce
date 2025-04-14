using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_project_ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentTransactionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTransactions",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PaymentTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "PaymentTransactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTransactions",
                table: "PaymentTransactions",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTransactions",
                table: "PaymentTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PaymentTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTransactions",
                table: "PaymentTransactions",
                column: "Id");
        }
    }
}
