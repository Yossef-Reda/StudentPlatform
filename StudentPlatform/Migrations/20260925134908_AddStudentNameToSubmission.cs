using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentNameToSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubmissions_AspNetUsers_StudentId",
                table: "StudentSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubmissions_StudentId",
                table: "StudentSubmissions");

            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "StudentSubmissions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "StudentSubmissions");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubmissions_StudentId",
                table: "StudentSubmissions",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubmissions_AspNetUsers_StudentId",
                table: "StudentSubmissions",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
