using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BqsClinoTag.Migrations
{
    public partial class chat_history : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UTILISATEUR",
                table: "UTILISATEUR");

            migrationBuilder.RenameTable(
                name: "UTILISATEUR",
                newName: "utilisateur");

            migrationBuilder.RenameIndex(
                name: "IX_UTILISATEUR_ROLE",
                table: "utilisateur",
                newName: "IX_utilisateur_ROLE");

            migrationBuilder.AddColumn<int>(
                name: "SATISFACTION",
                table: "PASSAGE",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ASK",
                table: "LIEU",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CONTACT",
                table: "LIEU",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "LIEU",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "INVENTORY",
                table: "LIEU",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PUBLIC_LINK",
                table: "LIEU",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "QTY",
                table: "LIEU",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "QTY_DATE",
                table: "LIEU",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SATISFACTION",
                table: "LIEU",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "STOCK",
                table: "LIEU",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_utilisateur",
                table: "utilisateur",
                column: "ID_UTILISATEUR");

            migrationBuilder.CreateTable(
                name: "acknowledge",
                columns: table => new
                {
                    ID_ACKNOWLEDGE = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NOTIFICATION_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LIEU = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NOTIFICATION = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NAME = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CONTACT = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acknowledge", x => x.ID_ACKNOWLEDGE);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "acknowledge_log",
                columns: table => new
                {
                    ID_ACKNOWLEDGELOG = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ACKNOWLEDGE_ID = table.Column<int>(type: "int", nullable: false),
                    ACKNOWLEDGE_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    USER_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acknowledge_log", x => x.ID_ACKNOWLEDGELOG);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SATISFACTION_LOG",
                columns: table => new
                {
                    ID_SATISFACTION = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LIEU_NAME = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CONTACT = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NAME = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Satisfaction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SATISFACTION_LOG", x => x.ID_SATISFACTION);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SETTINGS",
                columns: table => new
                {
                    Language = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Task = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LanguageOne = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LanguageTwo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LanguageThird = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailAPI = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Logo = table.Column<byte[]>(type: "longblob", nullable: true),
                    ResetTime = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acknowledge");

            migrationBuilder.DropTable(
                name: "acknowledge_log");

            migrationBuilder.DropTable(
                name: "SATISFACTION_LOG");

            migrationBuilder.DropTable(
                name: "SETTINGS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_utilisateur",
                table: "utilisateur");

            migrationBuilder.DropColumn(
                name: "SATISFACTION",
                table: "PASSAGE");

            migrationBuilder.DropColumn(
                name: "ASK",
                table: "LIEU");

            migrationBuilder.DropColumn(
                name: "CONTACT",
                table: "LIEU");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "LIEU");

            migrationBuilder.DropColumn(
                name: "INVENTORY",
                table: "LIEU");

            migrationBuilder.DropColumn(
                name: "PUBLIC_LINK",
                table: "LIEU");

            migrationBuilder.DropColumn(
                name: "QTY",
                table: "LIEU");

            migrationBuilder.DropColumn(
                name: "QTY_DATE",
                table: "LIEU");

            migrationBuilder.DropColumn(
                name: "SATISFACTION",
                table: "LIEU");

            migrationBuilder.DropColumn(
                name: "STOCK",
                table: "LIEU");

            migrationBuilder.RenameTable(
                name: "utilisateur",
                newName: "UTILISATEUR");

            migrationBuilder.RenameIndex(
                name: "IX_utilisateur_ROLE",
                table: "UTILISATEUR",
                newName: "IX_UTILISATEUR_ROLE");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UTILISATEUR",
                table: "UTILISATEUR",
                column: "ID_UTILISATEUR");
        }
    }
}
