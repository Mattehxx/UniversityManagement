using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityManagement.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConstranints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Students_FC",
                table: "Students",
                column: "FC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_FC",
                table: "Professors",
                column: "FC",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_FC",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Professors_FC",
                table: "Professors");
        }
    }
}
