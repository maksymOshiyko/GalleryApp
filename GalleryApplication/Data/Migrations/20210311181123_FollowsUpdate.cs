using Microsoft.EntityFrameworkCore.Migrations;

namespace GalleryApplication.Data.Migrations
{
    public partial class FollowsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Followers");

            migrationBuilder.CreateTable(
                name: "Follows",
                columns: table => new
                {
                    SourceUserId = table.Column<int>(type: "int", nullable: false),
                    FollowedUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Follows", x => new { x.SourceUserId, x.FollowedUserId });
                    table.ForeignKey(
                        name: "FK_Follows_AspNetUsers_FollowedUserId",
                        column: x => x.FollowedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Follows_AspNetUsers_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowedUserId",
                table: "Follows",
                column: "FollowedUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Follows");

            migrationBuilder.CreateTable(
                name: "Followers",
                columns: table => new
                {
                    SourceUserId = table.Column<int>(type: "int", nullable: false),
                    FollowedUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => new { x.SourceUserId, x.FollowedUserId });
                    table.ForeignKey(
                        name: "FK_Followers_AspNetUsers_FollowedUserId",
                        column: x => x.FollowedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Followers_AspNetUsers_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowedUserId",
                table: "Followers",
                column: "FollowedUserId");
        }
    }
}
