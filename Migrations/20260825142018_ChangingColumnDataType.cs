using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TawakalApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangingColumnDataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_a_portal_users_T_a_partners_PartnerId",
                table: "T_a_portal_users");

            migrationBuilder.AlterColumn<long>(
                name: "PartnerId",
                table: "T_a_portal_users",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_T_a_portal_users_T_a_portal_users_PartnerId",
                table: "T_a_portal_users",
                column: "PartnerId",
                principalTable: "T_a_portal_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_a_portal_users_T_a_portal_users_PartnerId",
                table: "T_a_portal_users");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "T_a_portal_users",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_T_a_portal_users_T_a_partners_PartnerId",
                table: "T_a_portal_users",
                column: "PartnerId",
                principalTable: "T_a_partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
