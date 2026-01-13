using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OJCommerce.Migrations
{
    /// <inheritdoc />
    public partial class webhookeventextended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizationCode",
                table: "PaymentWebhookEvents",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CardBrand",
                table: "PaymentWebhookEvents",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CardLast4",
                table: "PaymentWebhookEvents",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "CardReusable",
                table: "PaymentWebhookEvents",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "PaymentWebhookEvents",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizationCode",
                table: "PaymentWebhookEvents");

            migrationBuilder.DropColumn(
                name: "CardBrand",
                table: "PaymentWebhookEvents");

            migrationBuilder.DropColumn(
                name: "CardLast4",
                table: "PaymentWebhookEvents");

            migrationBuilder.DropColumn(
                name: "CardReusable",
                table: "PaymentWebhookEvents");

            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "PaymentWebhookEvents");
        }
    }
}
