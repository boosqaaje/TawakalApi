using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TawakalApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangingColumnDataType2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. DROP foreign keys pointing to T_a_partners.Id first 
            // (Replace 'FK_T_a_portal_users_T_a_partners_PartnerId' with your actual foreign key constraint name if different)
            migrationBuilder.DropForeignKey(
                name: "FK_T_a_portal_users_T_a_portal_users_PartnerId",
                table: "T_a_portal_users");

            // 2. DROP the Primary Key constraint on T_a_partners.Id
            migrationBuilder.DropPrimaryKey(
                name: "PK__T_a_part__3214EC074B2DD73F",
                table: "T_a_partners");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "T_a_partners",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "T_a_partners",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
