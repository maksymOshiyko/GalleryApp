using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GalleryApplication.Data.Migrations
{
    public partial class PostUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Posts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoPublicId",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPublicId",
                table: "Posts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Posts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "Posts",
                type: "int",
                nullable: true);
        }
    }
}
