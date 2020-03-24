using Microsoft.EntityFrameworkCore.Migrations;

namespace XRD.LibCat.Migrations
{
    public partial class addIsTeacher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTeacher",
                table: "tblStaffMembers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_tblStaffMembers_IsTeacher",
                table: "tblStaffMembers",
                column: "IsTeacher");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tblStaffMembers_IsTeacher",
                table: "tblStaffMembers");

            migrationBuilder.DropColumn(
                name: "IsTeacher",
                table: "tblStaffMembers");
        }
    }
}
