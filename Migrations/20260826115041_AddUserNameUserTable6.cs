using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TawakalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameUserTable6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_T_a_partners_PartnerUserName",
                table: "T_a_partners");

            migrationBuilder.DropColumn(
                name: "PartnerUserName",
                table: "T_a_partners");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "T_a_partners");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartnerUserName",
                table: "T_a_partners",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "T_a_partners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_T_a_partners_PartnerUserName",
                table: "T_a_partners",
                column: "PartnerUserName",
                unique: true);
        }
    }
}
