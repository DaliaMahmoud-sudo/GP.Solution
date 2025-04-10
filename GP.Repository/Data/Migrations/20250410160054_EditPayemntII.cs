using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GP.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditPayemntII : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "Payments");
        }
    }
}
