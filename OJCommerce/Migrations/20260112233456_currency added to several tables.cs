using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OJCommerce.Migrations
{
    /// <inheritdoc />
    public partial class currencyaddedtoseveraltables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FailureReason",
                table: "PaymentTransactions",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Orders",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "PaymentTransactions",
                keyColumn: "FailureReason",
                keyValue: null,
                column: "FailureReason",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "FailureReason",
                table: "PaymentTransactions",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
