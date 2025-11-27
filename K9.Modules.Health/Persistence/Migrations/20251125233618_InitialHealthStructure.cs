using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9.Modules.Health.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialHealthStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "health");

            migrationBuilder.CreateTable(
                name: "DogProfiles",
                schema: "health",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Breed = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaccinationRecords",
                schema: "health",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VaccineName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DateAdministered = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VetClinicName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DogProfileId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaccinationRecords_DogProfiles_DogProfileId",
                        column: x => x.DogProfileId,
                        principalSchema: "health",
                        principalTable: "DogProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeightLogs",
                schema: "health",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MeasuredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DogProfileId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeightLogs_DogProfiles_DogProfileId",
                        column: x => x.DogProfileId,
                        principalSchema: "health",
                        principalTable: "DogProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationRecords_DogProfileId",
                schema: "health",
                table: "VaccinationRecords",
                column: "DogProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightLogs_DogProfileId",
                schema: "health",
                table: "WeightLogs",
                column: "DogProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VaccinationRecords",
                schema: "health");

            migrationBuilder.DropTable(
                name: "WeightLogs",
                schema: "health");

            migrationBuilder.DropTable(
                name: "DogProfiles",
                schema: "health");
        }
    }
}
