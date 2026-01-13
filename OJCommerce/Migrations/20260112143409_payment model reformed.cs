using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OJCommerce.Migrations
{
    /// <inheritdoc />
    public partial class paymentmodelreformed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "PaymentTransactions",
                newName: "FailureReason");

            migrationBuilder.RenameColumn(
                name: "PublicPaymentTransactionId",
                table: "PaymentTransactions",
                newName: "PublicPaymentId");

            migrationBuilder.RenameColumn(
                name: "PaymentGateway",
                table: "PaymentTransactions",
                newName: "CustomerEmail");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "PaymentTransactions",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AuthorizationUrl",
                table: "PaymentTransactions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "PaymentTransactions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PaymentTransactions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "PaymentTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "PaymentTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProviderTransactionReference",
                table: "PaymentTransactions",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PaymentTransactions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Payload = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Processed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentWebhookEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Payload = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Processed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentWebhookEvents", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Provider_ProviderTransactionReference",
                table: "PaymentTransactions",
                columns: new[] { "Provider", "ProviderTransactionReference" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "PaymentWebhookEvents");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_Provider_ProviderTransactionReference",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "AuthorizationUrl",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ProviderTransactionReference",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "PublicPaymentId",
                table: "PaymentTransactions",
                newName: "PublicPaymentTransactionId");

            migrationBuilder.RenameColumn(
                name: "FailureReason",
                table: "PaymentTransactions",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "CustomerEmail",
                table: "PaymentTransactions",
                newName: "PaymentGateway");
        }
    }
}
