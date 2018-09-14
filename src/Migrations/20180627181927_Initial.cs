using Microsoft.EntityFrameworkCore.Migrations;

namespace Articulate.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendees",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendees", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendees");
        }
    }
}
