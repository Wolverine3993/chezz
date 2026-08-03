using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chezz.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRelationships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    User1Id = table.Column<string>(type: "TEXT", nullable: true),
                    User2Id = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRelationships_AspNetUsers_User1Id",
                        column: x => x.User1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRelationships_AspNetUsers_User2Id",
                        column: x => x.User2Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRelationships_User1Id",
                table: "UserRelationships",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelationships_User2Id",
                table: "UserRelationships",
                column: "User2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRelationships");
        }
    }
}
