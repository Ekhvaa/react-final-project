using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TourApi.Data;

#nullable disable

namespace TourApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260709123000_RemoveCountryFlagBase64")]
    public partial class RemoveCountryFlagBase64 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlagUrl",
                table: "Countries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.DropColumn(
                name: "Flag",
                table: "Countries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Flag",
                table: "Countries",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.DropColumn(
                name: "FlagUrl",
                table: "Countries");
        }
    }
}
