using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BqsClinoTag.Migrations
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
                name: "ROLE",
                columns: table => new
                {
                    ROLE = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLE", x => x.ROLE);
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
                name: "TACHE_PLANIFIEE",
                columns: table => new
                {
                    ID_TACHE_PLANIFIEE = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TACHE_PLANIFIEE_ACTIVE = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ACTION_TACHE_PLANIFIEE = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CRONTAB = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DESCRIPTION_CRONTAB = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DH_TACHE_PLANIFIEE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DH_DERNIERE_TACHE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TACHE_ACCOMPLIE = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TACHE_PLANIFIEE", x => x.ID_TACHE_PLANIFIEE);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GEOLOC_AGENT",
                columns: table => new
                {
                    ID_GEOLOC_AGENT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ID_CONSTRUCTEUR = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ID_AGENT = table.Column<int>(type: "int", nullable: false),
                    LATI = table.Column<double>(type: "double", nullable: false),
                    LONGI = table.Column<double>(type: "double", nullable: false),
                    DH_GEOLOC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IP_GEOLOC = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GEOLOC_AGENT", x => x.ID_GEOLOC_AGENT);
                    table.ForeignKey(
                        name: "FK_GEOLOC_AGENT_UTILISATEUR",
                        column: x => x.ID_AGENT,
                        principalTable: "AGENT",
                        principalColumn: "ID_AGENT");
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
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ACTION_TYPE = table.Column<int>(type: "int", nullable: false),
                    PROGRESS = table.Column<int>(type: "int", nullable: false)
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
                name: "MATERIEL",
                columns: table => new
                {
                    ID_MATERIEL = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NOM = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    INSTRUCTION = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ID_CLIENT = table.Column<int>(type: "int", nullable: false),
                    UID_TAG = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EXPIRATION = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATERIEL", x => x.ID_MATERIEL);
                    table.ForeignKey(
                        name: "FK_MATERIEL_CLIENT",
                        column: x => x.ID_CLIENT,
                        principalTable: "CLIENT",
                        principalColumn: "ID_CLIENT");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UTILISATEUR",
                columns: table => new
                {
                    ID_UTILISATEUR = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NOM = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PRENOM = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LOGIN = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EMAIL = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MDP = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ROLE = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UTILISATEUR", x => x.ID_UTILISATEUR);
                    table.ForeignKey(
                        name: "FK_UTILISATEUR_ROLE",
                        column: x => x.ROLE,
                        principalTable: "ROLE",
                        principalColumn: "ROLE");
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
                        principalColumn: "ID_LIEU",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TACHE_LIEU_TACHE",
                        column: x => x.ID_TACHE,
                        principalTable: "TACHE",
                        principalColumn: "ID_TACHE");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UTILISATION",
                columns: table => new
                {
                    ID_UTILISATION = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DH_DEBUT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DH_FIN = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ID_MATERIEL = table.Column<int>(type: "int", nullable: false),
                    ID_AGENT = table.Column<int>(type: "int", nullable: false),
                    COMMENTAIRE = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CLOTURE = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UTILISATION", x => x.ID_UTILISATION);
                    table.ForeignKey(
                        name: "FK_UTILISATION_AGENT",
                        column: x => x.ID_AGENT,
                        principalTable: "AGENT",
                        principalColumn: "ID_AGENT");
                    table.ForeignKey(
                        name: "FK_UTILISATION_MATERIEL",
                        column: x => x.ID_MATERIEL,
                        principalTable: "MATERIEL",
                        principalColumn: "ID_MATERIEL");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UCLIENT",
                columns: table => new
                {
                    ID_UCLIENT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ID_UTILISATEUR = table.Column<int>(type: "int", nullable: false),
                    ID_CLIENT = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UCLIENT", x => x.ID_UCLIENT);
                    table.ForeignKey(
                        name: "FK_UCLIENT_CLIENT",
                        column: x => x.ID_CLIENT,
                        principalTable: "CLIENT",
                        principalColumn: "ID_CLIENT");
                    table.ForeignKey(
                        name: "FK_UCLIENT_UTILISATEUR",
                        column: x => x.ID_UTILISATEUR,
                        principalTable: "UTILISATEUR",
                        principalColumn: "ID_UTILISATEUR");
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
                        principalColumn: "ID_PASSAGE",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PASSAGE_TACHE_TACHE_LIEU",
                        column: x => x.ID_TL,
                        principalTable: "TACHE_LIEU",
                        principalColumn: "ID_TL",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NOTIFICATION",
                columns: table => new
                {
                    ID_NOTIFICATION = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ID_UTILISATION = table.Column<int>(type: "int", nullable: false),
                    DH_NOTIFICATION = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TYPE_DESTINATAIRE = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICATION", x => x.ID_NOTIFICATION);
                    table.ForeignKey(
                        name: "FK_NOTIFICATION_UTILISATION",
                        column: x => x.ID_UTILISATION,
                        principalTable: "UTILISATION",
                        principalColumn: "ID_UTILISATION");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "AgentCodeUnique",
                table: "AGENT",
                column: "CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GEOLOC_AGENT_ID_AGENT",
                table: "GEOLOC_AGENT",
                column: "ID_AGENT");

            migrationBuilder.CreateIndex(
                name: "IX_LIEU_ID_CLIENT",
                table: "LIEU",
                column: "ID_CLIENT");

            migrationBuilder.CreateIndex(
                name: "TagUniqueLieu",
                table: "LIEU",
                column: "UID_TAG",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MATERIEL_ID_CLIENT",
                table: "MATERIEL",
                column: "ID_CLIENT");

            migrationBuilder.CreateIndex(
                name: "TagUniqueObjet",
                table: "MATERIEL",
                column: "UID_TAG",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_ID_UTILISATION",
                table: "NOTIFICATION",
                column: "ID_UTILISATION");

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

            migrationBuilder.CreateIndex(
                name: "IX_UCLIENT_ID_CLIENT",
                table: "UCLIENT",
                column: "ID_CLIENT");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-UtilisateurClient",
                table: "UCLIENT",
                columns: new[] { "ID_UTILISATEUR", "ID_CLIENT" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UTILISATEUR_ROLE",
                table: "UTILISATEUR",
                column: "ROLE");

            migrationBuilder.CreateIndex(
                name: "IX_UTILISATION_ID_AGENT",
                table: "UTILISATION",
                column: "ID_AGENT");

            migrationBuilder.CreateIndex(
                name: "IX_UTILISATION_ID_MATERIEL",
                table: "UTILISATION",
                column: "ID_MATERIEL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GEOLOC_AGENT");

            migrationBuilder.DropTable(
                name: "NOTIFICATION");

            migrationBuilder.DropTable(
                name: "PASSAGE_TACHE");

            migrationBuilder.DropTable(
                name: "TACHE_PLANIFIEE");

            migrationBuilder.DropTable(
                name: "UCLIENT");

            migrationBuilder.DropTable(
                name: "UTILISATION");

            migrationBuilder.DropTable(
                name: "PASSAGE");

            migrationBuilder.DropTable(
                name: "TACHE_LIEU");

            migrationBuilder.DropTable(
                name: "UTILISATEUR");

            migrationBuilder.DropTable(
                name: "MATERIEL");

            migrationBuilder.DropTable(
                name: "AGENT");

            migrationBuilder.DropTable(
                name: "LIEU");

            migrationBuilder.DropTable(
                name: "TACHE");

            migrationBuilder.DropTable(
                name: "ROLE");

            migrationBuilder.DropTable(
                name: "CLIENT");
        }
    }
}
