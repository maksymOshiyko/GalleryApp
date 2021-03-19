using Microsoft.EntityFrameworkCore.Migrations;

namespace GalleryApplication.Data.Migrations
{
    public partial class RolesFeatureAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasComplaint",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasComplaint",
                table: "Posts");
        }
    }
}
