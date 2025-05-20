using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class BetterModels4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alergeni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipAlergen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergeni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meniuri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Poza = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categorie = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meniuri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Preparate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pret = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantitatePortie = table.Column<int>(type: "int", nullable: false),
                    CantitateTotala = table.Column<int>(type: "int", nullable: false),
                    Categorie = table.Column<int>(type: "int", nullable: false),
                    Poza = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preparate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Angajat = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "MenuPreparat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreparatId = table.Column<int>(type: "int", nullable: false),
                    Cantitate = table.Column<int>(type: "int", nullable: false),
                    MenuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuPreparat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuPreparat_Meniuri_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Meniuri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuPreparat_Preparate_PreparatId",
                        column: x => x.PreparatId,
                        principalTable: "Preparate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreparatAlergen",
                columns: table => new
                {
                    AlergeniId = table.Column<int>(type: "int", nullable: false),
                    PreparateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreparatAlergen", x => new { x.AlergeniId, x.PreparateId });
                    table.ForeignKey(
                        name: "FK_PreparatAlergen_Alergeni_AlergeniId",
                        column: x => x.AlergeniId,
                        principalTable: "Alergeni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreparatAlergen_Preparate_PreparateId",
                        column: x => x.PreparateId,
                        principalTable: "Preparate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comenzi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StareComanda = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comenzi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comenzi_Users_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreparatCantitate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreparatId = table.Column<int>(type: "int", nullable: false),
                    Cantitate = table.Column<int>(type: "int", nullable: false),
                    ComandaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreparatCantitate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreparatCantitate_Comenzi_ComandaId",
                        column: x => x.ComandaId,
                        principalTable: "Comenzi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreparatCantitate_Preparate_PreparatId",
                        column: x => x.PreparatId,
                        principalTable: "Preparate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Alergeni",
                columns: new[] { "Id", "Nume", "TipAlergen" },
                values: new object[,]
                {
                    { 1, "Gluten", 0 },
                    { 2, "Oua", 1 },
                    { 3, "Lactoza", 2 },
                    { 4, "Alune", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comenzi_UserEmail",
                table: "Comenzi",
                column: "UserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPreparat_MenuId",
                table: "MenuPreparat",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPreparat_PreparatId",
                table: "MenuPreparat",
                column: "PreparatId");

            migrationBuilder.CreateIndex(
                name: "IX_PreparatAlergen_PreparateId",
                table: "PreparatAlergen",
                column: "PreparateId");

            migrationBuilder.CreateIndex(
                name: "IX_PreparatCantitate_ComandaId",
                table: "PreparatCantitate",
                column: "ComandaId");

            migrationBuilder.CreateIndex(
                name: "IX_PreparatCantitate_PreparatId",
                table: "PreparatCantitate",
                column: "PreparatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuPreparat");

            migrationBuilder.DropTable(
                name: "PreparatAlergen");

            migrationBuilder.DropTable(
                name: "PreparatCantitate");

            migrationBuilder.DropTable(
                name: "Meniuri");

            migrationBuilder.DropTable(
                name: "Alergeni");

            migrationBuilder.DropTable(
                name: "Comenzi");

            migrationBuilder.DropTable(
                name: "Preparate");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
