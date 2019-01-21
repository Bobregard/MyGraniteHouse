using Microsoft.EntityFrameworkCore.Migrations;

namespace MyGraniteHouse.Data.Migrations
{
    public partial class AddNameColumnToSpecialTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SpecialTags",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SpecialTags");
        }
    }
}
