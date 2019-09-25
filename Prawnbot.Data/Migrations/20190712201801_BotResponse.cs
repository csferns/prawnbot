using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Prawnbot.Data.Migrations
{
    public partial class BotResponse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration",
                schema: "core");

            migrationBuilder.CreateTable(
                name: "BotResponse",
                schema: "core",
                columns: table => new
                {
                    ResponseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LookupValue = table.Column<string>(maxLength: 2000, nullable: true),
                    ReplyValue = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotResponse", x => x.ResponseId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotResponse",
                schema: "core");

            migrationBuilder.CreateTable(
                name: "Configuration",
                schema: "core",
                columns: table => new
                {
                    ConfigurationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConfigurationName = table.Column<string>(nullable: true),
                    ConfigurationValue = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ConfigurationID);
                });
        }
    }
}
