using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPlatform.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LectureId",
                table: "Assignments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_LectureId",
                table: "Assignments",
                column: "LectureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Lectures_LectureId",
                table: "Assignments",
                column: "LectureId",
                principalTable: "Lectures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Lectures_LectureId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_LectureId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "LectureId",
                table: "Assignments");
        }
    }
}
