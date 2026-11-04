using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Assignments",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Assignments");
        }
    }
}
