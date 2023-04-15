using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinoTag.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AGENT",
                columns: table => new
                {
                    ID_AGENT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NOM = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CODE = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AGENT", x => x.ID_AGENT);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CLIENT",
                columns: table => new
                {
                    ID_CLIENT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NOM = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENT", x => x.ID_CLIENT);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TACHE",
                columns: table => new
                {
                    ID_TACHE = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NOM = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DESCRIPTION = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TACHE", x => x.ID_TACHE);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LIEU",
                columns: table => new
                {
                    ID_LIEU = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NOM = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ID_CLIENT = table.Column<int>(type: "int", nullable: false),
                    UID_TAG = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LIEU", x => x.ID_LIEU);
                    table.ForeignKey(
                        name: "FK_LIEU_CLIENT",
                        column: x => x.ID_CLIENT,
                        principalTable: "CLIENT",
                        principalColumn: "ID_CLIENT");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PASSAGE",
                columns: table => new
                {
                    ID_PASSAGE = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ID_LIEU = table.Column<int>(type: "int", nullable: false),
                    ID_AGENT = table.Column<int>(type: "int", nullable: false),
                    DH_DEBUT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DH_FIN = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PHOTO = table.Column<byte[]>(type: "longblob", nullable: true),
                    COMMENTAIRE = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PASSAGE", x => x.ID_PASSAGE);
                    table.ForeignKey(
                        name: "FK_PASSAGE_LIEU",
                        column: x => x.ID_LIEU,
                        principalTable: "LIEU",
                        principalColumn: "ID_LIEU");
                    table.ForeignKey(
                        name: "FK_PASSAGE_PASSAGE",
                        column: x => x.ID_AGENT,
                        principalTable: "AGENT",
                        principalColumn: "ID_AGENT");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TACHE_LIEU",
                columns: table => new
                {
                    ID_TL = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ID_TACHE = table.Column<int>(type: "int", nullable: false),
                    ID_LIEU = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TACHE_LIEU", x => x.ID_TL);
                    table.ForeignKey(
                        name: "FK_TACHE_LIEU_LIEU",
                        column: x => x.ID_LIEU,
                        principalTable: "LIEU",
                        principalColumn: "ID_LIEU");
                    table.ForeignKey(
                        name: "FK_TACHE_LIEU_TACHE",
                        column: x => x.ID_TACHE,
                        principalTable: "TACHE",
                        principalColumn: "ID_TACHE");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PASSAGE_TACHE",
                columns: table => new
                {
                    ID_PT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ID_PASSAGE = table.Column<int>(type: "int", nullable: false),
                    ID_TL = table.Column<int>(type: "int", nullable: false),
                    FAIT = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PASSAGE_TACHE", x => x.ID_PT);
                    table.ForeignKey(
                        name: "FK_PASSAGE_TACHE_PASSAGE",
                        column: x => x.ID_PASSAGE,
                        principalTable: "PASSAGE",
                        principalColumn: "ID_PASSAGE");
                    table.ForeignKey(
                        name: "FK_PASSAGE_TACHE_TACHE_LIEU",
                        column: x => x.ID_TL,
                        principalTable: "TACHE_LIEU",
                        principalColumn: "ID_TL");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "AgentCodeUnique",
                table: "AGENT",
                column: "CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LIEU_ID_CLIENT",
                table: "LIEU",
                column: "ID_CLIENT");

            migrationBuilder.CreateIndex(
                name: "IX_PASSAGE_ID_AGENT",
                table: "PASSAGE",
                column: "ID_AGENT");

            migrationBuilder.CreateIndex(
                name: "IX_PASSAGE_ID_LIEU",
                table: "PASSAGE",
                column: "ID_LIEU");

            migrationBuilder.CreateIndex(
                name: "IX_PASSAGE_TACHE_ID_PASSAGE",
                table: "PASSAGE_TACHE",
                column: "ID_PASSAGE");

            migrationBuilder.CreateIndex(
                name: "IX_PASSAGE_TACHE_ID_TL",
                table: "PASSAGE_TACHE",
                column: "ID_TL");

            migrationBuilder.CreateIndex(
                name: "IX_TACHE_LIEU",
                table: "TACHE_LIEU",
                columns: new[] { "ID_LIEU", "ID_TACHE" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TACHE_LIEU_ID_TACHE",
                table: "TACHE_LIEU",
                column: "ID_TACHE");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PASSAGE_TACHE");

            migrationBuilder.DropTable(
                name: "PASSAGE");

            migrationBuilder.DropTable(
                name: "TACHE_LIEU");

            migrationBuilder.DropTable(
                name: "AGENT");

            migrationBuilder.DropTable(
                name: "LIEU");

            migrationBuilder.DropTable(
                name: "TACHE");

            migrationBuilder.DropTable(
                name: "CLIENT");
        }
    }
}
