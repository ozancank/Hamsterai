using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnSipay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WebHookString",
                table: "PaymentSipays",
                newName: "TransactionStatus");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionStatus",
                table: "PaymentSipays",
                type: "citext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 35)
                .OldAnnotation("Relational:ColumnOrder", 33);

            migrationBuilder.AddColumn<string>(
                name: "ActionDate",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 46);

            migrationBuilder.AddColumn<string>(
                name: "BankStatusCode",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 33);

            migrationBuilder.AddColumn<string>(
                name: "BankStatusDescription",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 34);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 37);

            migrationBuilder.AddColumn<string>(
                name: "NextActionDate",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 47);

            migrationBuilder.AddColumn<string>(
                name: "ProductPrice",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 40);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 38);

            migrationBuilder.AddColumn<int>(
                name: "RecurringId",
                table: "PaymentSipays",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 42);

            migrationBuilder.AddColumn<int>(
                name: "RecurringNumber",
                table: "PaymentSipays",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 45);

            migrationBuilder.AddColumn<string>(
                name: "RecurringPlanCode",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 44);

            migrationBuilder.AddColumn<string>(
                name: "RecurringStatus",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 48);

            migrationBuilder.AddColumn<string>(
                name: "RefNumber",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 43);

            migrationBuilder.AddColumn<string>(
                name: "SettlementDate",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 49);

            migrationBuilder.AddColumn<int>(
                name: "TotalRefundedAmount",
                table: "PaymentSipays",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 39);

            migrationBuilder.AddColumn<int>(
                name: "TransactionAmount",
                table: "PaymentSipays",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 41);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 36);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionDate",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "BankStatusCode",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "BankStatusDescription",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "NextActionDate",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "ProductPrice",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "RecurringId",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "RecurringNumber",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "RecurringPlanCode",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "RecurringStatus",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "RefNumber",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "SettlementDate",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "TotalRefundedAmount",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "TransactionAmount",
                table: "PaymentSipays");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "PaymentSipays");

            migrationBuilder.RenameColumn(
                name: "TransactionStatus",
                table: "PaymentSipays",
                newName: "WebHookString");

            migrationBuilder.AlterColumn<string>(
                name: "WebHookString",
                table: "PaymentSipays",
                type: "citext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 33)
                .OldAnnotation("Relational:ColumnOrder", 35);
        }
    }
}
