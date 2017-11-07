using Microsoft.EntityFrameworkCore.Migrations;

namespace CrmBot.DataAccess.Migrations
{
    public partial class UserTimeZoneCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneCode",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "TimeZone",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }
    }
}
