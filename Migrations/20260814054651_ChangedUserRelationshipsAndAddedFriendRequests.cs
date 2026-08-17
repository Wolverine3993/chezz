using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chezz.Migrations
{
    /// <inheritdoc />
    public partial class ChangedUserRelationshipsAndAddedFriendRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserFromId = table.Column<string>(type: "TEXT", nullable: false),
                    UserToId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendRequests_AspNetUsers_UserFromId",
                        column: x => x.UserFromId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendRequests_AspNetUsers_UserToId",
                        column: x => x.UserToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_UserFromId",
                table: "FriendRequests",
                column: "UserFromId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_UserToId",
                table: "FriendRequests",
                column: "UserToId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_UserToId_UserFromId",
                table: "FriendRequests",
                columns: new[] { "UserToId", "UserFromId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendRequests");
        }
    }
}
