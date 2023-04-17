using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinoTag.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LANG",
                table: "AGENT",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LANG",
                table: "AGENT");
        }
    }
}
