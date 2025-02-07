using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentSipay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SipayMerchantKey",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SipayPlanCode",
                table: "Payments");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentSipayId",
                table: "Payments",
                type: "uuid",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 11);

            migrationBuilder.CreateTable(
                name: "PaymentSipays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "citext", nullable: true),
                    OrderNo = table.Column<string>(type: "citext", nullable: true),
                    OrderId = table.Column<string>(type: "citext", nullable: true),
                    InvoiceId = table.Column<string>(type: "citext", nullable: true),
                    StatusCode = table.Column<string>(type: "citext", nullable: true),
                    StatusDescription = table.Column<string>(type: "citext", nullable: true),
                    PaymentMethod = table.Column<string>(type: "citext", nullable: true),
                    CreditCardNo = table.Column<string>(type: "citext", nullable: true),
                    TransactionType = table.Column<string>(type: "citext", nullable: true),
                    PaymentStatus = table.Column<string>(type: "citext", nullable: true),
                    PaymentMethodCode = table.Column<string>(type: "citext", nullable: true),
                    ErrorCode = table.Column<string>(type: "citext", nullable: true),
                    Error = table.Column<string>(type: "citext", nullable: true),
                    AuthCode = table.Column<string>(type: "citext", nullable: true),
                    MerchantCommission = table.Column<string>(type: "citext", nullable: true),
                    UserCommission = table.Column<string>(type: "citext", nullable: true),
                    MerchantCommissionPercentage = table.Column<string>(type: "citext", nullable: true),
                    MerchantCommissionFixed = table.Column<string>(type: "citext", nullable: true),
                    Installment = table.Column<string>(type: "citext", nullable: true),
                    Amount = table.Column<string>(type: "citext", nullable: true),
                    PaymentReasonCode = table.Column<string>(type: "citext", nullable: true),
                    PaymentReasonCodeDetail = table.Column<string>(type: "citext", nullable: true),
                    HashKey = table.Column<string>(type: "citext", nullable: true),
                    MdStatus = table.Column<string>(type: "citext", nullable: true),
                    OriginalBankErrorCode = table.Column<string>(type: "citext", nullable: true),
                    OriginalBankErrorDescription = table.Column<string>(type: "citext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSipays", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentSipayId",
                table: "Payments",
                column: "PaymentSipayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentSipays_PaymentSipayId",
                table: "Payments",
                column: "PaymentSipayId",
                principalTable: "PaymentSipays",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentSipays_PaymentSipayId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "PaymentSipays");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentSipayId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentSipayId",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "SipayMerchantKey",
                table: "Payments",
                type: "citext",
                maxLength: 100,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 11);

            migrationBuilder.AddColumn<string>(
                name: "SipayPlanCode",
                table: "Payments",
                type: "citext",
                maxLength: 100,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 12);
        }
    }
}
