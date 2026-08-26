using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TawakalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameUserTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        //     migrationBuilder.DropIndex(
        // name: "IX_T_a_partners_PartnerUserName",
        // table: "T_a_partners"
        // );

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "T_a_portal_users");

            migrationBuilder.AlterColumn<string>(
                name: "PartnerEmail",
                table: "T_a_partners",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "PartnerUserName",
                table: "T_a_partners",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_T_a_portal_users_Email",
                table: "T_a_portal_users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_a_partners_PartnerEmail",
                table: "T_a_partners",
                column: "PartnerEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_a_partners_PartnerUserName",
                table: "T_a_partners",
                column: "PartnerUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_T_a_portal_users_Email",
                table: "T_a_portal_users");

            migrationBuilder.DropIndex(
                name: "IX_T_a_partners_PartnerEmail",
                table: "T_a_partners");

            migrationBuilder.DropIndex(
                name: "IX_T_a_partners_PartnerUserName",
                table: "T_a_partners");

            migrationBuilder.DropColumn(
                name: "PartnerUserName",
                table: "T_a_partners");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "T_a_portal_users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PartnerEmail",
                table: "T_a_partners",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
