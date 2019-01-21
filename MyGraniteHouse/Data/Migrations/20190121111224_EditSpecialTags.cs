using Microsoft.EntityFrameworkCore.Migrations;

namespace MyGraniteHouse.Data.Migrations
{
    public partial class EditSpecialTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SpecialTags");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SpecialTags",
                nullable: true);
        }
    }
}
