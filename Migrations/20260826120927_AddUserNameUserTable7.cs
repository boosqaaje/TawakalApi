using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TawakalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameUserTable7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.DropIndex(
                name: "IX_T_a_portal_users_PartnerId",
                table: "T_a_portal_users");

            migrationBuilder.DropIndex(
                name: "IX_T_a_portal_users_UserName",
                table: "T_a_portal_users");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "T_a_portal_users");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "T_a_portal_users");

            migrationBuilder.AddColumn<string>(
                name: "PartnerUserName",
                table: "T_a_partners",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

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
                name: "IX_T_a_partners_PartnerUserName",
                table: "T_a_partners");

            migrationBuilder.DropColumn(
                name: "PartnerUserName",
                table: "T_a_partners");

            migrationBuilder.AddColumn<long>(
                name: "PartnerId",
                table: "T_a_portal_users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "T_a_portal_users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_T_a_portal_users_PartnerId",
                table: "T_a_portal_users",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_T_a_portal_users_UserName",
                table: "T_a_portal_users",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_T_a_portal_users_T_a_portal_users_PartnerId",
                table: "T_a_portal_users",
                column: "PartnerId",
                principalTable: "T_a_portal_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
