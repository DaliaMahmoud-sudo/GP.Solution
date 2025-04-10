using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GP.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditPayemnt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Payments");
        }
    }
}
