using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ebillets_jo2024_API.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelsWithDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "offre",
                columns: table => new
                {
                    IdOffre = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NomOffre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NbPersonnes = table.Column<int>(type: "int", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offre", x => x.IdOffre);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "utilisateur",
                columns: table => new
                {
                    idUtilisateur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    prenom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    motDePasseHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cleUtilisateur = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilisateur", x => x.idUtilisateur);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "administrateur",
                columns: table => new
                {
                    idAdmin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    idUtilisateur = table.Column<int>(type: "int", nullable: false),
                    roleDetail = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dateAjout = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administrateur", x => x.idAdmin);
                    table.ForeignKey(
                        name: "FK_administrateur_utilisateur_idUtilisateur",
                        column: x => x.idUtilisateur,
                        principalTable: "utilisateur",
                        principalColumn: "idUtilisateur",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "panier",
                columns: table => new
                {
                    idPanier = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    idUtilisateur = table.Column<int>(type: "int", nullable: false),
                    dateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_panier", x => x.idPanier);
                    table.ForeignKey(
                        name: "FK_panier_utilisateur_idUtilisateur",
                        column: x => x.idUtilisateur,
                        principalTable: "utilisateur",
                        principalColumn: "idUtilisateur",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "reservation",
                columns: table => new
                {
                    idReservation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    idUtilisateur = table.Column<int>(type: "int", nullable: false),
                    idOffre = table.Column<int>(type: "int", nullable: false),
                    quantite = table.Column<int>(type: "int", nullable: false),
                    cleReservation = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cleFinale = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    qrcode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    statut = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dateReservation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation", x => x.idReservation);
                    table.ForeignKey(
                        name: "FK_reservation_offre_idOffre",
                        column: x => x.idOffre,
                        principalTable: "offre",
                        principalColumn: "IdOffre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservation_utilisateur_idUtilisateur",
                        column: x => x.idUtilisateur,
                        principalTable: "utilisateur",
                        principalColumn: "idUtilisateur",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "panier_offre",
                columns: table => new
                {
                    IdPanier = table.Column<int>(type: "int", nullable: false),
                    IdOffre = table.Column<int>(type: "int", nullable: false),
                    quantite = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_panier_offre", x => new { x.IdPanier, x.IdOffre });
                    table.ForeignKey(
                        name: "FK_panier_offre_offre_IdOffre",
                        column: x => x.IdOffre,
                        principalTable: "offre",
                        principalColumn: "IdOffre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_panier_offre_panier_IdPanier",
                        column: x => x.IdPanier,
                        principalTable: "panier",
                        principalColumn: "idPanier",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "billet",
                columns: table => new
                {
                    idBillet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    idReservation = table.Column<int>(type: "int", nullable: false),
                    idOffre = table.Column<int>(type: "int", nullable: false),
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false),
                    cleBillet = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cleFinale = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    qrcode = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    statut = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    titulaireNom = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dateEmission = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CleSecrete = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billet", x => x.idBillet);
                    table.ForeignKey(
                        name: "FK_billet_offre_idOffre",
                        column: x => x.idOffre,
                        principalTable: "offre",
                        principalColumn: "IdOffre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_billet_reservation_idReservation",
                        column: x => x.idReservation,
                        principalTable: "reservation",
                        principalColumn: "idReservation",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "paiement",
                columns: table => new
                {
                    IdPaiement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdReservation = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ModePaiement = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatePaiement = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Statut = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paiement", x => x.IdPaiement);
                    table.ForeignKey(
                        name: "FK_paiement_reservation_IdReservation",
                        column: x => x.IdReservation,
                        principalTable: "reservation",
                        principalColumn: "idReservation",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "scan_billet",
                columns: table => new
                {
                    idScan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    idBillet = table.Column<int>(type: "int", nullable: false),
                    idEmploye = table.Column<int>(type: "int", nullable: false),
                    dateScan = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    resultat = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scan_billet", x => x.idScan);
                    table.ForeignKey(
                        name: "FK_scan_billet_billet_idBillet",
                        column: x => x.idBillet,
                        principalTable: "billet",
                        principalColumn: "idBillet",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scan_billet_utilisateur_idEmploye",
                        column: x => x.idEmploye,
                        principalTable: "utilisateur",
                        principalColumn: "idUtilisateur",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_administrateur_idUtilisateur",
                table: "administrateur",
                column: "idUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_billet_idOffre",
                table: "billet",
                column: "idOffre");

            migrationBuilder.CreateIndex(
                name: "IX_billet_idReservation",
                table: "billet",
                column: "idReservation");

            migrationBuilder.CreateIndex(
                name: "IX_paiement_IdReservation",
                table: "paiement",
                column: "IdReservation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_panier_idUtilisateur",
                table: "panier",
                column: "idUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_panier_offre_IdOffre",
                table: "panier_offre",
                column: "IdOffre");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_idOffre",
                table: "reservation",
                column: "idOffre");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_idUtilisateur",
                table: "reservation",
                column: "idUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_scan_billet_idBillet",
                table: "scan_billet",
                column: "idBillet");

            migrationBuilder.CreateIndex(
                name: "IX_scan_billet_idEmploye",
                table: "scan_billet",
                column: "idEmploye");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "administrateur");

            migrationBuilder.DropTable(
                name: "paiement");

            migrationBuilder.DropTable(
                name: "panier_offre");

            migrationBuilder.DropTable(
                name: "scan_billet");

            migrationBuilder.DropTable(
                name: "panier");

            migrationBuilder.DropTable(
                name: "billet");

            migrationBuilder.DropTable(
                name: "reservation");

            migrationBuilder.DropTable(
                name: "offre");

            migrationBuilder.DropTable(
                name: "utilisateur");
        }
    }
}
