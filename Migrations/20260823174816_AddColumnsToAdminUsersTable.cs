using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TawakalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsToAdminUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "T_a_portal_users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "T_a_portal_users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "T_a_portal_users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "T_a_portal_users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "T_a_portal_users");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "T_a_portal_users");
        }
    }
}
